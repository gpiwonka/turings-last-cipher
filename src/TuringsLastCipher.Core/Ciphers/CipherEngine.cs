using TuringsLastCipher.Core;

namespace TuringsLastCipher.Core.Ciphers;

/// <summary>
/// Dispatches a <see cref="CipherSpec"/> to the matching cipher. This is the single entry
/// point the game uses to turn a stored plaintext into the ciphertext the player sees, and
/// to check a player's decryption against the stored plaintext.
/// </summary>
public static class CipherEngine
{
    public static string Encrypt(CipherSpec spec, string plaintext) => spec.Kind switch
    {
        CipherKind.Atbash => Atbash.Encrypt(plaintext),
        CipherKind.Caesar => Caesar.Encrypt(plaintext, spec.Shift),
        CipherKind.Substitution => Substitution.Encrypt(plaintext, RequireKey(spec)),
        CipherKind.Vigenere => Vigenere.Encrypt(plaintext, RequireKey(spec)),
        CipherKind.Enigma => Enigma.Encrypt(plaintext, RequireEnigma(spec)),
        _ => throw new ArgumentOutOfRangeException(nameof(spec), spec.Kind, "Unknown cipher.")
    };

    public static string Decrypt(CipherSpec spec, string ciphertext) => spec.Kind switch
    {
        CipherKind.Atbash => Atbash.Decrypt(ciphertext),
        CipherKind.Caesar => Caesar.Decrypt(ciphertext, spec.Shift),
        CipherKind.Substitution => Substitution.Decrypt(ciphertext, RequireKey(spec)),
        CipherKind.Vigenere => Vigenere.Decrypt(ciphertext, RequireKey(spec)),
        CipherKind.Enigma => Enigma.Decrypt(ciphertext, RequireEnigma(spec)),
        _ => throw new ArgumentOutOfRangeException(nameof(spec), spec.Kind, "Unknown cipher.")
    };

    /// <summary>
    /// Solvability guard: a puzzle is shippable only if decrypting its own ciphertext yields
    /// the canonical plaintext under the oracle's normalization. Run before content ships.
    /// </summary>
    public static bool RoundTrips(CipherSpec spec, string plaintext)
        => CipherOracle.Solved(Decrypt(spec, Encrypt(spec, plaintext)), plaintext);

    private static string RequireKey(CipherSpec spec)
        => spec.Key ?? throw new ArgumentException($"Cipher {spec.Kind} requires a Key.", nameof(spec));

    private static EnigmaSettings RequireEnigma(CipherSpec spec)
        => spec.Enigma ?? throw new ArgumentException("Cipher Enigma requires Enigma settings.", nameof(spec));
}
