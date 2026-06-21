using TuringsLastCipher.Core.Ciphers;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

/// <summary>Known-answer vectors and edge cases for individual ciphers.</summary>
public class CipherVectorTests
{
    [Fact]
    public void Atbash_maps_a_to_z_and_preserves_case()
    {
        Assert.Equal("Zyx", Atbash.Transform("Abc"));
        // Reciprocal: applying twice is the identity.
        Assert.Equal("Hello, World!", Atbash.Transform(Atbash.Transform("Hello, World!")));
    }

    [Fact]
    public void Caesar_shifts_by_three_with_wraparound()
        => Assert.Equal("Khoor, Zruog!", Caesar.Encrypt("Hello, World!", 3));

    [Fact]
    public void Caesar_thirteen_is_self_inverse()
        => Assert.Equal("Turing", Caesar.Encrypt(Caesar.Encrypt("Turing", 13), 13));

    [Fact]
    public void Vigenere_classic_vector()
        // ATTACKATDAWN with key LEMON -> LXFOPVEFRNHR
        => Assert.Equal("LXFOPVEFRNHR", Vigenere.Encrypt("ATTACKATDAWN", "LEMON"));

    [Fact]
    public void Vigenere_keyword_wraps_when_shorter_than_text()
    {
        // Key "A" applies a zero shift everywhere, so the text is unchanged.
        Assert.Equal("UNCHANGED", Vigenere.Encrypt("UNCHANGED", "A"));
        // A repeating key must wrap: longer text than keyword still decrypts cleanly.
        const string longText = "MEET ME AT THE BLETCHLEY PARK BENCH AT NOON";
        Assert.Equal(longText, Vigenere.Decrypt(Vigenere.Encrypt(longText, "KEY"), "KEY"));
    }

    [Fact]
    public void Vigenere_skips_non_letters_in_text_without_consuming_key()
    {
        // Spaces must not advance the keyword: "AB CD" with key "BB" shifts every letter by 1.
        Assert.Equal("BC DE", Vigenere.Encrypt("AB CD", "BB"));
    }
}
