using System.Net.Http.Json;

namespace TuringsLastCipher.Client;

/// <summary>Typed wrapper over the game's server endpoints. All puzzle truth stays on the server.</summary>
public sealed class GameApi
{
    private readonly HttpClient _http;

    public GameApi(HttpClient http) => _http = http;

    public Task<SceneView?> StartAsync()
        => _http.GetFromJsonAsync<SceneView>("api/start");

    public Task<SceneView?> SceneAsync(string id)
        => _http.GetFromJsonAsync<SceneView>($"api/scene/{id}");

    public async Task<SolveResult> SolveAsync(string sceneId, string answer)
    {
        var resp = await _http.PostAsJsonAsync("api/solve", new { sceneId, answer });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<SolveResult>())!;
    }

    public async Task<HintResult> HintAsync(string sceneId)
    {
        var resp = await _http.PostAsJsonAsync("api/hint", new { sceneId });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<HintResult>())!;
    }

    public async Task<IReadOnlyList<LetterCount>> FrequencyAsync(string text)
    {
        var resp = await _http.PostAsJsonAsync("api/frequency", new { text });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<List<LetterCount>>())!;
    }

    public async Task<string> EnigmaDecodeAsync(string sceneId, IReadOnlyList<int> positions)
    {
        var resp = await _http.PostAsJsonAsync("api/enigma", new { sceneId, positions });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<EnigmaToolResult>())!.Text;
    }
}
