using TuringsLastCipher.Api;
using TuringsLastCipher.Core.Ciphers;

var builder = WebApplication.CreateBuilder(args);

// Cloud Run injects PORT and expects the server to listen on 0.0.0.0. Honour it when present.
string? port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddSingleton<StoryProvider>();
builder.Services.AddSingleton<AssistantService>();
builder.Services.AddHttpClient<GeminiClient>();

var app = builder.Build();

// Fail fast: load and validate story content before serving anything.
var story = app.Services.GetRequiredService<StoryProvider>().Story;

// Host the Blazor WebAssembly client from this same container.
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api");

api.MapGet("/start", () => story.Get(story.StartId).ToView());

api.MapGet("/scene/{id}", (string id) =>
    story.TryGet(id, out var scene) ? Results.Ok(scene.ToView()) : Results.NotFound());

// The solution oracle lives entirely server-side; the plaintext never leaves this process.
api.MapPost("/solve", (SolveRequest req) =>
{
    if (!story.TryGet(req.SceneId, out var scene) || scene.Puzzle is not { } puzzle)
        return Results.NotFound();
    bool solved = puzzle.IsSolvedBy(req.Answer ?? string.Empty);
    return Results.Ok(new SolveResult(solved, solved ? puzzle.Next : null));
});

api.MapPost("/hint", async (HintRequest req, AssistantService assistant, CancellationToken ct) =>
    story.TryGet(req.SceneId, out var scene)
        ? Results.Ok(await assistant.HintAsync(scene, ct))
        : Results.NotFound());

api.MapPost("/dialogue", async (DialogueRequest req, AssistantService assistant, CancellationToken ct) =>
    story.TryGet(req.SceneId, out var scene)
        ? Results.Ok(await assistant.DialogueAsync(scene, ct))
        : Results.NotFound());

// Frequency analysis is a player tool, computed by Core (never the LLM).
api.MapPost("/frequency", (FrequencyRequest req) =>
    Results.Ok(FrequencyAnalysis.Ranked(req.Text ?? string.Empty)
        .Select(e => new { Letter = e.Letter.ToString(), e.Count })));

// Enigma decode tool: run the puzzle's ciphertext through the machine with the player's chosen
// rotor positions. The secret rings stay server-side; readable output means the dials are right.
api.MapPost("/enigma", (EnigmaToolRequest req) =>
{
    if (!story.TryGet(req.SceneId, out var scene) || scene.Puzzle?.Cipher.Enigma is not { } settings)
        return Results.NotFound();
    if (req.Positions is null || req.Positions.Count != settings.Rotors.Count)
        return Results.BadRequest($"Expected {settings.Rotors.Count} rotor positions.");

    string decoded = Enigma.Transform(scene.Puzzle.Ciphertext, settings.WithPositions(req.Positions));
    return Results.Ok(new EnigmaToolResult(decoded));
});

app.MapFallbackToFile("index.html");

app.Run();
