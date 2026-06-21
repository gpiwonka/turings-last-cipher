using TuringsLastCipher.Core;
using TuringsLastCipher.Core.Ciphers;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

public class EnigmaTests
{
    private static EnigmaSettings Settings(string? plugboard = null, int[]? positions = null, int[]? rings = null)
        => new(
            Rotors: new[] { "I", "II", "III" },
            Positions: positions ?? new[] { 0, 0, 0 },
            Reflector: "B",
            Rings: rings,
            Plugboard: plugboard);

    [Theory]
    [InlineData(null, null)]
    [InlineData("AT BE QX", null)]
    [InlineData("AT BE", new[] { 1, 11, 4 })]
    public void Encrypting_twice_with_same_settings_returns_plaintext(string? plug, int[]? positions)
    {
        const string plain = "The machine kept our secrets better than they kept me! 1936.";
        var settings = Settings(plug, positions, rings: new[] { 2, 0, 5 });

        string cipher = Enigma.Encrypt(plain, settings);
        string back = Enigma.Decrypt(cipher, settings);

        Assert.NotEqual(CipherOracle.Normalize(plain), CipherOracle.Normalize(cipher));
        Assert.Equal(plain, back);
    }

    [Fact]
    public void No_letter_ever_encrypts_to_itself()
    {
        const string plain = "AAAAAAAAAAAAAAAAAAAAAAAAAAZZZZZ THE QUICK BROWN FOX";
        string cipher = Enigma.Encrypt(plain, Settings(plugboard: "AT BE"));
        for (int i = 0; i < plain.Length; i++)
            if (char.IsLetter(plain[i]))
                Assert.NotEqual(char.ToUpperInvariant(plain[i]), char.ToUpperInvariant(cipher[i]));
    }

    [Fact]
    public void Different_start_positions_produce_different_ciphertext()
    {
        const string plain = "MEET AT THE PARK";
        Assert.NotEqual(
            Enigma.Encrypt(plain, Settings(positions: new[] { 0, 0, 0 })),
            Enigma.Encrypt(plain, Settings(positions: new[] { 0, 0, 1 })));
    }

    [Fact]
    public void Non_letters_pass_through_and_do_not_step_rotors()
    {
        // The two halves are identical letter streams; the spaces/punctuation between must not
        // shift the rotor schedule, so the encoded letters line up.
        string a = Enigma.Encrypt("ABCDEFGHIJ", Settings());
        string b = Enigma.Encrypt("ABCDEFGHIJ", Settings());
        Assert.Equal(a, b);

        string spaced = Enigma.Encrypt("AB CD", Settings());
        string tight = Enigma.Encrypt("ABCD", Settings());
        // Remove the space from the spaced ciphertext: it must equal the tight ciphertext.
        Assert.Equal(tight, spaced.Replace(" ", ""));
    }

    [Theory]
    [InlineData("ABC")]      // odd-length pair
    [InlineData("AA")]       // letter paired with itself
    [InlineData("AB BC")]    // B reused
    [InlineData("A5")]       // non-letter
    public void Invalid_plugboard_is_rejected(string plugboard)
        => Assert.Throws<ArgumentException>(() => Enigma.Encrypt("HELLO", Settings(plugboard: plugboard)));

    [Fact]
    public void Mismatched_position_count_is_rejected()
    {
        var bad = new EnigmaSettings(new[] { "I", "II", "III" }, new[] { 0, 0 });
        Assert.Throws<ArgumentException>(() => Enigma.Encrypt("HELLO", bad));
    }

    [Fact]
    public void Round_trips_through_the_engine_and_oracle()
    {
        var spec = new CipherSpec(CipherKind.Enigma, Enigma: Settings(plugboard: "AT BE", positions: new[] { 1, 11, 4 }));
        const string plaintext = "I BUILT A MACHINE THAT COULD THINK";

        string ciphertext = CipherEngine.Encrypt(spec, plaintext);
        Assert.True(CipherEngine.RoundTrips(spec, plaintext));
        Assert.True(CipherOracle.Solved(CipherEngine.Decrypt(spec, ciphertext), plaintext));
    }
}
