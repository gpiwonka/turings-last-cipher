namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Monoalphabetic substitution. The key is a 26-letter permutation of A-Z: the letter at
/// position i is the ciphertext for the i-th plaintext letter (A maps to key[0], etc.).
/// </summary>
public static class Substitution
{
    public static string Encrypt(string text, string key) => Apply(text, BuildKey(key));

    public static string Decrypt(string text, string key) => Apply(text, Invert(BuildKey(key)));

    /// <summary>Validate and normalise a key into a 26-char uppercase mapping array.</summary>
    private static char[] BuildKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        string k = key.Trim().ToUpperInvariant();
        if (k.Length != Letters.AlphabetSize)
            throw new ArgumentException("Substitution key must be exactly 26 letters.", nameof(key));

        var seen = new bool[Letters.AlphabetSize];
        foreach (char c in k)
        {
            if (c < 'A' || c > 'Z')
                throw new ArgumentException("Substitution key must contain only letters A-Z.", nameof(key));
            int idx = c - 'A';
            if (seen[idx])
                throw new ArgumentException("Substitution key must be a permutation of A-Z (no repeats).", nameof(key));
            seen[idx] = true;
        }
        return k.ToCharArray();
    }

    private static char[] Invert(char[] forward)
    {
        var inverse = new char[Letters.AlphabetSize];
        for (int i = 0; i < forward.Length; i++)
            inverse[forward[i] - 'A'] = (char)('A' + i);
        return inverse;
    }

    private static string Apply(string text, char[] map)
    {
        ArgumentNullException.ThrowIfNull(text);
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (!Letters.IsAsciiLetter(c)) continue;
            char mapped = map[Letters.IndexOf(c)];
            chars[i] = Letters.IsUpper(c) ? mapped : char.ToLowerInvariant(mapped);
        }
        return new string(chars);
    }
}
