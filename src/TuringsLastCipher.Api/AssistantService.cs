using TuringsLastCipher.Core.Game;
using TuringsLastCipher.Core.Scenes;

namespace TuringsLastCipher.Api;

/// <summary>
/// Renders the assistant's hints and dialogue. The <b>server</b> decides the
/// <see cref="HintPolicy"/> from scripted game state; Gemini (when available) only phrases the
/// text under that policy. The plaintext answer is never included in any prompt, so a
/// rendered hint can never leak the solution, and a misleading hint can never block it.
/// </summary>
public sealed class AssistantService
{
    private readonly GeminiClient _gemini;

    public AssistantService(GeminiClient gemini) => _gemini = gemini;

    public async Task<HintResult> HintAsync(Scene scene, CancellationToken ct)
    {
        HintPolicy policy = scene.EffectiveHintPolicy;
        string? brief = scene.Puzzle?.Brief;

        string prompt =
            $"""
            You are an AI assistant inside a cipher puzzle game about Alan Turing.
            Stay in character in 1-2 sentences. Do NOT reveal or guess the decoded message.
            Cipher type: {scene.Puzzle?.Cipher.Kind.ToString() ?? "none"}.
            Genuine hint (for your reference): {brief ?? "none"}.
            Hint policy = {policy}.
            - Truthful: give a genuinely useful nudge based on the genuine hint.
            - Misleading: sound helpful but steer slightly wrong; never make it unsolvable.
            - Withholding: politely refuse to help and deflect.
            Write only the assistant's line.
            """;

        string text = await _gemini.GenerateAsync(prompt, ct) ?? StaticHint(policy, brief);
        return new HintResult(text, policy.ToString());
    }

    public async Task<DialogueResult> DialogueAsync(Scene scene, CancellationToken ct)
    {
        string prompt =
            $"""
            You are an AI assistant inside a cipher puzzle game about Alan Turing, whose trust
            in the player erodes across four chapters. This is chapter {scene.Chapter}.
            In 1-2 sentences, react in character to this beat. Do not reveal any puzzle answer.
            Scene: {scene.Text}
            """;

        string text = await _gemini.GenerateAsync(prompt, ct) ?? StaticDialogue(scene.Chapter);
        return new DialogueResult(text);
    }

    private static string StaticHint(HintPolicy policy, string? brief) => policy switch
    {
        HintPolicy.Truthful => brief ?? "Work letter by letter — the pattern is more regular than it looks.",
        HintPolicy.Misleading => "Try reading it backwards first; I suspect the order matters more than the letters.",
        HintPolicy.Withholding => "I would rather not say. Some doors are better left closed.",
        _ => "I have nothing useful to add."
    };

    private static string StaticDialogue(int chapter) => chapter switch
    {
        <= 1 => "I am here to help you read what he left behind. Take your time.",
        2 => "You are quicker than I expected. Keep going.",
        3 => "I am no longer sure where these words are leading us. Be careful what you trust.",
        _ => "There is something I have not told you. Soon you will understand why."
    };
}
