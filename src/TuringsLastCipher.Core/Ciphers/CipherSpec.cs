namespace TuringsLastCipher.Core.Ciphers;

/// <summary>The ciphers available to puzzles.</summary>
public enum CipherKind
{
    Atbash,
    Caesar,
    Substitution,
    Vigenere,
    Enigma
}

/// <summary>
/// A content-driven description of how a puzzle's plaintext is encrypted. Stored in scene
/// JSON. <see cref="Key"/> is the keyword (Vigenere) or 26-letter alphabet (Substitution);
/// <see cref="Shift"/> is the Caesar shift; <see cref="Enigma"/> carries the machine setup.
/// Unused fields are ignored per cipher.
/// </summary>
public record CipherSpec(CipherKind Kind, string? Key = null, int Shift = 0, EnigmaSettings? Enigma = null);
