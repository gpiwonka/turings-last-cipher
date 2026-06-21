using TuringsLastCipher.Core.Ciphers;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

/// <summary>Decrypt(Encrypt(x, k), k) == x must hold for every cipher.</summary>
public class CipherRoundTripTests
{
    private const string Plain = "The quick brown fox jumps over the lazy dog! 1936.";
    private const string SubKey = "QWERTYUIOPASDFGHJKLZXCVBNM"; // a 26-letter permutation

    [Fact]
    public void Atbash_round_trips()
        => Assert.Equal(Plain, Atbash.Decrypt(Atbash.Encrypt(Plain)));

    [Theory]
    [InlineData(3)]
    [InlineData(13)]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(29)]
    public void Caesar_round_trips(int shift)
        => Assert.Equal(Plain, Caesar.Decrypt(Caesar.Encrypt(Plain, shift), shift));

    [Fact]
    public void Substitution_round_trips()
        => Assert.Equal(Plain, Substitution.Decrypt(Substitution.Encrypt(Plain, SubKey), SubKey));

    [Theory]
    [InlineData("ENIGMA")]
    [InlineData("A")]
    [InlineData("turing")]
    public void Vigenere_round_trips(string keyword)
        => Assert.Equal(Plain, Vigenere.Decrypt(Vigenere.Encrypt(Plain, keyword), keyword));

    [Fact]
    public void Ciphers_preserve_non_letters_and_length()
    {
        string cipher = Vigenere.Encrypt(Plain, "ENIGMA");
        Assert.Equal(Plain.Length, cipher.Length);
        // Spaces, punctuation and digits pass through untouched.
        for (int i = 0; i < Plain.Length; i++)
            if (!char.IsLetter(Plain[i]))
                Assert.Equal(Plain[i], cipher[i]);
    }
}
