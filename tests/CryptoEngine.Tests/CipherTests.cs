using CryptoEngine.Resources;
using Org.BouncyCastle.Crypto;

namespace CryptoEngine.Tests;

public class CipherTests
{

    [Fact]
    public void TestInvalidCipherCreateCall()
    {
        var config = new CryptoConfig();

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateCipher(config));
    }

    [Fact]
    public void TestInvalidCtsInput()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            BlockMode = BlockMode.CTS,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        Assert.Throws<DataLengthException>(
                () => cipherBuilder.EncryptTextToBytes("Hallo Welt"));
    }

    //RC4 Combinations

    [Fact]
    public void TestValidRc4Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.RC4,
            KeySize = 2048
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    //#############################
    //ECB Combinations without Pbe
    //############################
    [Fact]
    public void TestValidAes128EcbZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.ECB,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192EcbZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.ECB,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256EcbZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.ECB,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes128EcbPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.ECB,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192EcbPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.ECB,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256EcbPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.ECB,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    //#############################
    //CBC Combinations without Pbe
    //############################
    [Fact]
    public void TestValidAes128CbcZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.CBC,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192CbcZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.CBC,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256CbcZeroByteCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.CBC,
            Padding = Padding.ZeroByte
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes128CbcPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.CBC,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192CbcPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.CBC,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256CbcPkcs7Cipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.CBC,
            Padding = Padding.Pkcs7
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    //#############################
    //CTS Combinations without Pbe
    //############################

    [Fact]
    public void TestValidAes128CtsCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.CTS,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt, Du bist schön.");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt, Du bist schön.",result);
    }

    [Fact]
    public void TestValidAes192CtsCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.CTS,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt, Du bist schön.");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt, Du bist schön.",result);
    }

    [Fact]
    public void TestValidAes256CtsCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.CTS,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt, Du bist schön.");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt, Du bist schön.",result);
    }

    //#############################
    //OFB Combinations without Pbe
    //############################

    [Fact]
    public void TestValidAes128OfbCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.OFB,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192OfbCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.OFB,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256OfbCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.OFB,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    //#############################
    //GCM Combinations without Pbe
    //############################
    [Fact]
    public void TestValidAes128GcmCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 128,
            BlockMode = BlockMode.GCM,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes192GcmCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 192,
            BlockMode = BlockMode.GCM,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }

    [Fact]
    public void TestValidAes256GcmCipher()
    {
        var config = new CryptoConfig
        {
            IsEncryptActive = true,
            CipherAlgorithm = CipherAlgorithm.AES,
            KeySize = 256,
            BlockMode = BlockMode.GCM,
            Padding = Padding.None
        };

        var cipherBuilder = CryptoFactory.CreateCipher(config);
        config = cipherBuilder.EncryptTextToBytes("Hallo Welt");
        var decodedCipher = Convert.FromBase64String(config.Cipher);

        var result = cipherBuilder.DecryptBytesToText(decodedCipher);
        Assert.Equal("Hallo Welt",result);
    }
}
