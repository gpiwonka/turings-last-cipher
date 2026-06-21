using TuringsLastCipher.Core.Ciphers;

namespace TuringsLastCipher.Core.Scenes;

/// <summary>
/// A cipher puzzle attached to a scene. <see cref="Plaintext"/> is the stored truth: it is
/// encrypted in C# to produce the ciphertext the player sees and is the only thing a player's
/// answer is checked against. It must never be sent raw to the client.
/// </summary>
public record Puzzle(
    string Id,
    CipherSpec Cipher,
    string Plaintext,
    string Next,
    string? Brief = null)
{
    /// <summary>The ciphertext shown to the player, derived deterministically from the plaintext.</summary>
    public string Ciphertext => CipherEngine.Encrypt(Cipher, Plaintext);

    /// <summary>True when the player's decryption matches the stored plaintext under normalization.</summary>
    public bool IsSolvedBy(string playerInput) => CipherOracle.Solved(playerInput, Plaintext);

    /// <summary>Shippable only if the puzzle's own ciphertext decrypts back to its plaintext.</summary>
    public bool IsSolvable() => CipherEngine.RoundTrips(Cipher, Plaintext);
}
