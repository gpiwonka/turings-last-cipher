namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Vigenere: a keyword supplies a repeating sequence of Caesar shifts. The keyword index
/// only advances on letters, so spaces and punctuation in the text do not consume the key.
/// A keyword shorter than the text wraps around.
/// </summary>
public static class Vigenere
{
    public static string Encrypt(string text, string keyword) => Crypt(text, keyword, encrypt: true);

    public static string Decrypt(string text, string keyword) => Crypt(text, keyword, encrypt: false);

    private static string Crypt(string text, string keyword, bool encrypt)
    {
        ArgumentNullException.ThrowIfNull(text);
        int[] shifts = BuildShifts(keyword);
        var chars = text.ToCharArray();
        int k = 0;
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (!Letters.IsAsciiLetter(c)) continue;
            int shift = shifts[k % shifts.Length];
            if (!encrypt) shift = -shift;
            int idx = Letters.Mod(Letters.IndexOf(c) + shift, Letters.AlphabetSize);
            chars[i] = Letters.FromIndex(idx, Letters.IsUpper(c));
            k++;
        }
        return new string(chars);
    }

    private static int[] BuildShifts(string keyword)
    {
        ArgumentNullException.ThrowIfNull(keyword);
        var shifts = new List<int>(keyword.Length);
        foreach (char c in keyword)
            if (Letters.IsAsciiLetter(c))
                shifts.Add(Letters.IndexOf(c));
        if (shifts.Count == 0)
            throw new ArgumentException("Vigenere keyword must contain at least one letter.", nameof(keyword));
        return shifts.ToArray();
    }
}
