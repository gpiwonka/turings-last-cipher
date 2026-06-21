using TuringsLastCipher.Core.Scenes;

namespace TuringsLastCipher.Api;

/// <summary>
/// Client-facing DTOs. These deliberately omit the puzzle plaintext: the client only ever
/// sees ciphertext and framing, never the answer it is checked against.
/// </summary>
public record ChoiceView(string Text, string Next);

/// <summary>Enigma machine metadata for the in-game decode tool. Deliberately omits the secret
/// start positions and ring settings — the player supplies positions and the server applies the
/// hidden rings.</summary>
public record EnigmaView(IReadOnlyList<string> Rotors, string Reflector, string? Plugboard, int RotorCount);

public record PuzzleView(string Id, string Cipher, string Ciphertext, string? Brief, string HintPolicy, EnigmaView? Enigma);

public record SceneView(
    string Id,
    string Text,
    int Chapter,
    IReadOnlyList<ChoiceView> Choices,
    PuzzleView? Puzzle,
    bool IsEnding);

public record SolveRequest(string SceneId, string Answer);
public record SolveResult(bool Solved, string? Next);

public record HintRequest(string SceneId);
public record HintResult(string Text, string Policy);

public record DialogueRequest(string SceneId);
public record DialogueResult(string Text);

public record FrequencyRequest(string Text);

public record EnigmaToolRequest(string SceneId, IReadOnlyList<int> Positions);
public record EnigmaToolResult(string Text);

public static class SceneMapping
{
    /// <summary>Project a domain <see cref="Scene"/> to its safe client view (strips plaintext).</summary>
    public static SceneView ToView(this Scene scene)
    {
        var choices = scene.Options.Select(c => new ChoiceView(c.Text, c.Next)).ToList();
        PuzzleView? puzzle = null;
        if (scene.Puzzle is { } p)
        {
            EnigmaView? enigma = p.Cipher.Enigma is { } e
                ? new EnigmaView(e.Rotors, e.Reflector, e.Plugboard, e.Rotors.Count)
                : null;
            puzzle = new PuzzleView(p.Id, p.Cipher.Kind.ToString(), p.Ciphertext, p.Brief,
                scene.EffectiveHintPolicy.ToString(), enigma);
        }
        return new SceneView(scene.Id, scene.Text, scene.Chapter, choices, puzzle, scene.IsEnding);
    }
}
