namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Configuration for the Enigma-lite machine. <see cref="Rotors"/> are named (e.g. "I",
/// "II", "III") and listed left-to-right; the rightmost rotor is the fast one. Each list
/// indexed per rotor must match <see cref="Rotors"/> in length. <see cref="Positions"/> are
/// the start positions (0-25); <see cref="Rings"/> the ring settings (0-25, default all 0);
/// <see cref="Plugboard"/> is space-separated letter pairs, e.g. "AT BE".
/// </summary>
public record EnigmaSettings(
    IReadOnlyList<string> Rotors,
    IReadOnlyList<int> Positions,
    string Reflector = "B",
    IReadOnlyList<int>? Rings = null,
    string? Plugboard = null)
{
    /// <summary>A copy of these settings with different start positions (used by the in-game tool).</summary>
    public EnigmaSettings WithPositions(IReadOnlyList<int> positions) => this with { Positions = positions };
}
