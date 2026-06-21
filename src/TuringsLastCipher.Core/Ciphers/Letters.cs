namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Small shared helpers for letter math. Ciphers operate on the ASCII A-Z alphabet,
/// preserve case, and pass every non-letter through unchanged.
/// </summary>
internal static class Letters
{
    public const int AlphabetSize = 26;

    public static bool IsAsciiLetter(char c)
        => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

    public static bool IsUpper(char c) => c >= 'A' && c <= 'Z';

    /// <summary>0-based position in the alphabet (case-insensitive). Caller guarantees a letter.</summary>
    public static int IndexOf(char letter) => char.ToUpperInvariant(letter) - 'A';

    /// <summary>Build a letter from a 0-based alphabet index that is already in [0, 25].</summary>
    public static char FromIndex(int index, bool upper) => (char)((upper ? 'A' : 'a') + index);

    /// <summary>Euclidean modulo: result is always in [0, m). Handles negative shifts.</summary>
    public static int Mod(int value, int m) => ((value % m) + m) % m;
}
