namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Caesar: shift every letter by a fixed amount around the alphabet. Decryption is the
/// inverse shift. Any integer shift is accepted and reduced modulo 26.
/// </summary>
public static class Caesar
{
    public static string Encrypt(string text, int shift) => Shift(text, shift);

    public static string Decrypt(string text, int shift) => Shift(text, -shift);

    private static string Shift(string text, int shift)
    {
        ArgumentNullException.ThrowIfNull(text);
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (!Letters.IsAsciiLetter(c)) continue;
            int idx = Letters.Mod(Letters.IndexOf(c) + shift, Letters.AlphabetSize);
            chars[i] = Letters.FromIndex(idx, Letters.IsUpper(c));
        }
        return new string(chars);
    }
}
