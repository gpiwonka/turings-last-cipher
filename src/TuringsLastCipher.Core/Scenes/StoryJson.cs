using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringsLastCipher.Core.Scenes;

/// <summary>
/// Parses content-driven story JSON into a <see cref="Story"/>. Operates on strings only —
/// file I/O stays in the hosting layer so Core remains free of I/O dependencies. Cipher kinds
/// and hint policies are written as their names (e.g. "Caesar", "Truthful").
/// </summary>
public static class StoryJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private sealed record StoryDocument(string Start, IReadOnlyList<Scene> Scenes);

    public static Story Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var doc = JsonSerializer.Deserialize<StoryDocument>(json, Options)
                  ?? throw new JsonException("Story JSON deserialized to null.");
        if (doc.Scenes is null || doc.Scenes.Count == 0)
            throw new JsonException("Story JSON contains no scenes.");
        return new Story(doc.Scenes, doc.Start);
    }
}
