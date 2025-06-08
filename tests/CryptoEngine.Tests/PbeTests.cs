using CryptoEngine.Resources;

namespace CryptoEngine.Tests;

public class PbeTests
{

    [Fact]
    public void TestInvalidPbeCreateCall()
    {
        var config = new CryptoConfig {PbePassword = "12345".ToCharArray()};

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreatePbe(config));
    }

    [Fact]
    public void TestEmptyPasswordConfig()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            PbeDigest = PbeDigest.GCM,
            BlockMode = BlockMode.GCM,
            IsPbeActive = true
        };

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreatePbe(config));
    }

    //####################
    //SCRYPT Combinations#
    //####################

    [Fact]
    public void TestValidScryptKey()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            PbeAlgorithm = PbeAlgorithm.SCRYPT,
            PbeDigest = PbeDigest.GCM,
            PbePassword = "secret".ToCharArray(),
            BlockMode = BlockMode.GCM,
            IsPbeActive = true
        };

        var pbeBuilder = CryptoFactory.CreatePbe(config);
        config = pbeBuilder.GenerateKeyBytes();

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    //####################
    //PBKDF Combinations#
    //####################

    [Fact]
    public void TestValidPbkdf2Sha256Key()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            IsPbeActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            PbeAlgorithm = PbeAlgorithm.PBKDF2,
            PbeDigest = PbeDigest.SHA256,
            PbePassword = "secret".ToCharArray(),
            BlockMode = BlockMode.CBC,
            Padding = Padding.Pkcs7,
        };

        var pbeBuilder = CryptoFactory.CreatePbe(config);
        config = pbeBuilder.GenerateKeyBytes();

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidPbkdf2Sha1Key()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.RC4,
            KeySize = 40,
            PbeAlgorithm = PbeAlgorithm.PBKDF2,
            PbeDigest = PbeDigest.SHA1,
            PbePassword = "secret".ToCharArray(),
            BlockMode = BlockMode.None,
            IsPbeActive = true
        };

        var pbeBuilder = CryptoFactory.CreatePbe(config);
        config = pbeBuilder.GenerateKeyBytes();

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }
}
