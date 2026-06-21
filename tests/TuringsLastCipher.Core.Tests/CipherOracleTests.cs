using TuringsLastCipher.Core;
using TuringsLastCipher.Core.Ciphers;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

public class CipherOracleTests
{
    [Theory]
    [InlineData("MEET AT NOON", "meet at noon")]
    [InlineData("Meet at noon.", "MEETATNOON")]
    [InlineData("  meet,  at... noon!  ", "MEET AT NOON")]
    public void Solved_accepts_canonical_plaintext_under_normalization(string playerInput, string truth)
        => Assert.True(CipherOracle.Solved(playerInput, truth));

    [Theory]
    [InlineData("MEET AT NOON", "MEET AT MIDNIGHT")]
    [InlineData("", "SOMETHING")]
    [InlineData("almost", "almostt")]
    public void Solved_rejects_clear_misses(string playerInput, string truth)
        => Assert.False(CipherOracle.Solved(playerInput, truth));

    [Fact]
    public void Normalize_keeps_digits_and_drops_punctuation_and_whitespace()
        => Assert.Equal("TURING1936", CipherOracle.Normalize(" Turing, 1936! "));

    [Theory]
    [InlineData(CipherKind.Atbash, null, 0)]
    [InlineData(CipherKind.Caesar, null, 7)]
    [InlineData(CipherKind.Substitution, "QWERTYUIOPASDFGHJKLZXCVBNM", 0)]
    [InlineData(CipherKind.Vigenere, "ENIGMA", 0)]
    public void Every_spec_round_trips_through_the_oracle(CipherKind kind, string? key, int shift)
    {
        var spec = new CipherSpec(kind, key, shift);
        const string plaintext = "If a machine can think, it might think more cleverly than we do.";
        string ciphertext = CipherEngine.Encrypt(spec, plaintext);

        Assert.NotEqual(CipherOracle.Normalize(plaintext), CipherOracle.Normalize(ciphertext));
        Assert.True(CipherEngine.RoundTrips(spec, plaintext));
        Assert.True(CipherOracle.Solved(CipherEngine.Decrypt(spec, ciphertext), plaintext));
    }

    [Fact]
    public void Engine_requires_a_key_for_keyed_ciphers()
    {
        var spec = new CipherSpec(CipherKind.Vigenere);
        Assert.Throws<ArgumentException>(() => CipherEngine.Encrypt(spec, "HELLO"));
    }
}
