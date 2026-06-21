using System.Text;

namespace TuringsLastCipher.Core;

/// <summary>
/// The deterministic solution oracle. This is the only authority on whether a player's
/// answer is correct — never the LLM. Comparison is case-, whitespace-, and
/// punctuation-insensitive so players are not punished for formatting.
/// </summary>
public static class CipherOracle
{
    public static bool Solved(string playerInput, string truth)
        => Normalize(playerInput) == Normalize(truth);

    /// <summary>Uppercase and strip everything that is not a letter or digit.</summary>
    public static string Normalize(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
            if (char.IsLetterOrDigit(c))
                sb.Append(char.ToUpperInvariant(c));
        return sb.ToString();
    }
}
