namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Atbash: maps A&lt;-&gt;Z, B&lt;-&gt;Y, ... The cipher is its own inverse (reciprocal),
/// so encrypting twice returns the original text.
/// </summary>
public static class Atbash
{
    public static string Transform(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (!Letters.IsAsciiLetter(c)) continue;
            int mirrored = Letters.AlphabetSize - 1 - Letters.IndexOf(c);
            chars[i] = Letters.FromIndex(mirrored, Letters.IsUpper(c));
        }
        return new string(chars);
    }

    public static string Encrypt(string text) => Transform(text);

    public static string Decrypt(string text) => Transform(text);
}
