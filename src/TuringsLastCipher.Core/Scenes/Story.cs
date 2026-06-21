namespace TuringsLastCipher.Core.Scenes;

/// <summary>
/// An immutable graph of scenes keyed by id, with a designated start. Construction is cheap;
/// call <see cref="Validate"/> once at load time to catch broken links and unsolvable puzzles
/// before any content reaches a player.
/// </summary>
public sealed class Story
{
    private readonly IReadOnlyDictionary<string, Scene> _scenes;

    public string StartId { get; }

    public Story(IEnumerable<Scene> scenes, string startId)
    {
        ArgumentNullException.ThrowIfNull(scenes);
        ArgumentException.ThrowIfNullOrWhiteSpace(startId);
        var map = new Dictionary<string, Scene>(StringComparer.Ordinal);
        foreach (var scene in scenes)
        {
            if (!map.TryAdd(scene.Id, scene))
                throw new ArgumentException($"Duplicate scene id '{scene.Id}'.", nameof(scenes));
        }
        _scenes = map;
        StartId = startId;
    }

    public bool TryGet(string id, out Scene scene) => _scenes.TryGetValue(id, out scene!);

    public Scene Get(string id)
        => _scenes.TryGetValue(id, out var scene)
            ? scene
            : throw new KeyNotFoundException($"No scene with id '{id}'.");

    /// <summary>
    /// Returns a list of content problems (empty when the story is sound): a missing start,
    /// dangling scene references, or puzzles that do not round-trip. This is the solvability
    /// gate the brief requires before puzzles ship to the player.
    /// </summary>
    public IReadOnlyList<string> Validate()
    {
        var problems = new List<string>();

        if (!_scenes.ContainsKey(StartId))
            problems.Add($"Start scene '{StartId}' does not exist.");

        foreach (var scene in _scenes.Values)
        {
            foreach (var choice in scene.Options)
                if (!_scenes.ContainsKey(choice.Next))
                    problems.Add($"Scene '{scene.Id}' choice points to missing scene '{choice.Next}'.");

            if (scene.Puzzle is { } puzzle)
            {
                if (!_scenes.ContainsKey(puzzle.Next))
                    problems.Add($"Puzzle '{puzzle.Id}' in scene '{scene.Id}' advances to missing scene '{puzzle.Next}'.");
                if (!puzzle.IsSolvable())
                    problems.Add($"Puzzle '{puzzle.Id}' in scene '{scene.Id}' does not round-trip and is unsolvable.");
            }
        }

        return problems;
    }
}
