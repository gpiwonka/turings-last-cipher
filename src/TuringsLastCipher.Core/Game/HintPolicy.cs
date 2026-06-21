namespace TuringsLastCipher.Core.Game;

/// <summary>
/// What the in-game assistant is allowed to do when the player asks for a hint at a given
/// story beat. The <b>server</b> owns this decision; the LLM only renders the chosen text.
/// A non-truthful policy must never block solving — the cipher and the frequency tool always
/// provide a deterministic path to the answer regardless of what the assistant says.
/// </summary>
public enum HintPolicy
{
    /// <summary>A genuinely useful nudge toward the solution.</summary>
    Truthful,

    /// <summary>A plausible but wrong steer. Flavour for the trust-erosion arc, never a blocker.</summary>
    Misleading,

    /// <summary>The assistant refuses or deflects, offering no real help.</summary>
    Withholding
}
