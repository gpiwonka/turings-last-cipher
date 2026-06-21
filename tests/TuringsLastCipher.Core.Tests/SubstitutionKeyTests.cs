using TuringsLastCipher.Core.Ciphers;
using Xunit;

namespace TuringsLastCipher.Core.Tests;

public class SubstitutionKeyTests
{
    [Theory]
    [InlineData("TOOSHORT")]                          // wrong length
    [InlineData("QWERTYUIOPASDFGHJKLZXCVBNMA")]       // 27 chars
    [InlineData("AABCDEFGHIJKLMNOPQRSTUVWXY")]        // repeated letter, missing one
    [InlineData("QWERTYUIOPASDFGHJKLZXCVBN5")]        // non-letter
    public void Invalid_keys_are_rejected(string key)
        => Assert.Throws<ArgumentException>(() => Substitution.Encrypt("HELLO", key));

    [Fact]
    public void Identity_key_leaves_letters_unchanged()
    {
        const string identity = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Assert.Equal("Hello, World!", Substitution.Encrypt("Hello, World!", identity));
    }

    [Fact]
    public void Key_is_case_insensitive_and_trimmed()
    {
        const string upper = "QWERTYUIOPASDFGHJKLZXCVBNM";
        const string messy = "  qwertyuiopasdfghjklzxcvbnm  ";
        Assert.Equal(Substitution.Encrypt("TURING", upper), Substitution.Encrypt("TURING", messy));
    }
}
