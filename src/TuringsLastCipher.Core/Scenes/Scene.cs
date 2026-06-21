using TuringsLastCipher.Core.Game;

namespace TuringsLastCipher.Core.Scenes;

/// <summary>
/// A single story beat. A scene either offers <see cref="Choices"/> to navigate, carries a
/// <see cref="Puzzle"/> that must be solved to advance, or is a terminal ending (no puzzle,
/// no choices). Transitions are deterministic game state and are never decided by the LLM.
/// </summary>
public record Scene(
    string Id,
    string Text,
    Puzzle? Puzzle = null,
    IReadOnlyList<Choice>? Choices = null,
    int Chapter = 1,
    HintPolicy? HintPolicy = null)
{
    public IReadOnlyList<Choice> Options => Choices ?? Array.Empty<Choice>();

    /// <summary>A scene with neither a puzzle nor choices is an ending.</summary>
    public bool IsEnding => Puzzle is null && Options.Count == 0;

    /// <summary>
    /// The hint policy in force for this beat: an explicit per-scene override if present,
    /// otherwise the chapter default from the <see cref="DeceptionDirector"/>.
    /// </summary>
    public HintPolicy EffectiveHintPolicy => HintPolicy ?? DeceptionDirector.PolicyForChapter(Chapter);
}
