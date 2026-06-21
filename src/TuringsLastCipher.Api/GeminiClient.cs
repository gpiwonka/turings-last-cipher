using System.Net.Http.Json;
using System.Text.Json;

namespace TuringsLastCipher.Api;

/// <summary>
/// Thin server-side proxy to the Gemini API. The API key lives only here (env / Secret
/// Manager) and never reaches the client. Every call is best-effort: any failure — missing
/// key, network error, bad response — returns null so callers fall back to static text and
/// the game stays fully playable offline.
/// </summary>
public sealed class GeminiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<GeminiClient> _log;
    private readonly string? _apiKey;
    private readonly string _model;

    public GeminiClient(HttpClient http, IConfiguration config, ILogger<GeminiClient> log)
    {
        _http = http;
        _log = log;
        _apiKey = config["GEMINI_API_KEY"];
        _model = config["GEMINI_MODEL"] ?? "gemini-2.5-flash";
    }

    public bool Enabled => !string.IsNullOrWhiteSpace(_apiKey);

    /// <summary>Render text from a prompt, or null if Gemini is unavailable or errors out.</summary>
    public async Task<string?> GenerateAsync(string prompt, CancellationToken ct)
    {
        if (!Enabled) return null;

        try
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
            var body = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                // gemini-2.5-flash is a thinking model: reasoning tokens count against
                // maxOutputTokens. These are 1-2 sentence flavor lines that need no reasoning,
                // so disable thinking (thinkingBudget = 0) — otherwise the visible hint gets
                // truncated mid-sentence. Keep a comfortable cap for the actual text.
                generationConfig = new
                {
                    temperature = 0.9,
                    maxOutputTokens = 256,
                    thinkingConfig = new { thinkingBudget = 0 }
                }
            };

            using var resp = await _http.PostAsJsonAsync(url, body, ct);
            if (!resp.IsSuccessStatusCode)
            {
                _log.LogWarning("Gemini returned {Status}; falling back to static text.", resp.StatusCode);
                return null;
            }

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
            string? text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Gemini call failed; falling back to static text.");
            return null;
        }
    }
}
