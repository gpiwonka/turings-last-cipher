namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Letter-frequency helper exposed to the player as a solving aid. It is a tool, not a
/// cipher: it only counts letters and never transforms text.
/// </summary>
public static class FrequencyAnalysis
{
    /// <summary>Case-insensitive count for every letter A-Z (zero when absent).</summary>
    public static IReadOnlyDictionary<char, int> Counts(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var counts = new Dictionary<char, int>(Letters.AlphabetSize);
        for (char c = 'A'; c <= 'Z'; c++) counts[c] = 0;
        foreach (char c in text)
            if (Letters.IsAsciiLetter(c))
                counts[char.ToUpperInvariant(c)]++;
        return counts;
    }

    /// <summary>Letters that actually appear, ordered by descending count then alphabetically.</summary>
    public static IReadOnlyList<(char Letter, int Count)> Ranked(string text)
        => Counts(text)
            .Where(kv => kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
}
