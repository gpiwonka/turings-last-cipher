namespace TuringsLastCipher.Client;

// Client-side mirrors of the server DTOs. Deserialized from the /api/* responses.
public record ChoiceView(string Text, string Next);

public record EnigmaView(IReadOnlyList<string> Rotors, string Reflector, string? Plugboard, int RotorCount);

public record PuzzleView(string Id, string Cipher, string Ciphertext, string? Brief, string HintPolicy, EnigmaView? Enigma);

public record SceneView(
    string Id,
    string Text,
    int Chapter,
    IReadOnlyList<ChoiceView> Choices,
    PuzzleView? Puzzle,
    bool IsEnding);

public record SolveResult(bool Solved, string? Next);
public record HintResult(string Text, string Policy);
public record DialogueResult(string Text);
public record LetterCount(string Letter, int Count);
public record EnigmaToolResult(string Text);
