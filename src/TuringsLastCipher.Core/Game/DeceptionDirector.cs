namespace TuringsLastCipher.Core.Game;

/// <summary>
/// Deterministic rules for how the assistant's trustworthiness erodes across the four
/// chapters. This is scripted game logic, not an LLM decision: the same beat always yields
/// the same policy, so the narrative arc is reproducible and testable.
/// </summary>
public static class DeceptionDirector
{
    /// <summary>
    /// Default hint policy for a chapter. Chapters 1-2 the assistant is reliable; by Chapter 3
    /// it starts to mislead; by Chapter 4 it largely withholds. Individual beats may override
    /// this via <see cref="Scenes.Scene.HintPolicy"/>.
    /// </summary>
    public static HintPolicy PolicyForChapter(int chapter) => chapter switch
    {
        <= 2 => HintPolicy.Truthful,
        3 => HintPolicy.Misleading,
        _ => HintPolicy.Withholding
    };
}
