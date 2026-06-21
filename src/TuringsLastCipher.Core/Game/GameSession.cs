using TuringsLastCipher.Core.Scenes;

namespace TuringsLastCipher.Core.Game;

/// <summary>
/// A single player's deterministic progress through a <see cref="Story"/>. All state lives
/// here on the server; the LLM never advances it. Navigation happens only through declared
/// choices, and a puzzle scene advances only when <see cref="CipherOracle"/> accepts the answer.
/// </summary>
public sealed class GameSession
{
    private readonly Story _story;
    private readonly HashSet<string> _solved = new(StringComparer.Ordinal);

    public string CurrentSceneId { get; private set; }

    public GameSession(Story story, string? startId = null)
    {
        _story = story ?? throw new ArgumentNullException(nameof(story));
        CurrentSceneId = startId ?? story.StartId;
        if (!_story.TryGet(CurrentSceneId, out _))
            throw new ArgumentException($"Start scene '{CurrentSceneId}' does not exist.", nameof(startId));
    }

    public Scene Current => _story.Get(CurrentSceneId);

    public IReadOnlyCollection<string> SolvedPuzzleIds => _solved;

    /// <summary>
    /// Follow a choice on the current scene. Returns false if the target is not one of the
    /// current scene's declared choices (guards against forged navigation).
    /// </summary>
    public bool Choose(string nextSceneId)
    {
        foreach (var choice in Current.Options)
        {
            if (choice.Next == nextSceneId)
            {
                CurrentSceneId = nextSceneId;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Attempt the current scene's puzzle. On a correct answer the puzzle is recorded as
    /// solved and the session advances; otherwise nothing changes.
    /// </summary>
    public bool TrySolve(string playerInput)
    {
        if (Current.Puzzle is not { } puzzle || !puzzle.IsSolvedBy(playerInput))
            return false;
        _solved.Add(puzzle.Id);
        CurrentSceneId = puzzle.Next;
        return true;
    }
}
