using System.Text;
using CryptoEngine.Resources;

namespace CryptoEngine.Tests;

public class DigestTests
{

    [Fact]
    public void TestInvalidDigestCreateCall()
    {
        var config = new CryptoConfig {};

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateDigest(config));
    }

    [Fact]
    public void TestInvalidCertCreateConfig()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Dsa
        };

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateDigest(config));
    }

    //##############
    //Sha256 Tests #
    //##############
    [Fact]
    public void TestValidSha256Digest()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.Sha256
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.True(result);
    }

    [Fact]
    public void TestInvalidSha256Digest()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.Sha256
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);
        config.Signature = config.Signature.ToUpper();

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.False(result);
    }

    //###############
    //AesCmac Tests #
    //###############
    [Fact]
    public void TestValidAesCmac()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.AesCmac
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.True(result);
    }

    [Fact]
    public void TestInvalidAesCmac()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.AesCmac
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);
        config.Signature = config.Signature.ToUpper();

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.False(result);
    }

    //#################
    //HmacSha256 Tests#
    //#################
    [Fact]
    public void TestValidHmacSha256()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.HmacSha256
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.True(result);
    }

    [Fact]
    public void TestInvalidHmacSha256()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest,
            IntegrityOptions = IntegrityOptions.HmacSha256
        };

        var cipherBuilder = CryptoFactory.CreateDigest(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        config = cipherBuilder.SignInput(base64String);
        config.Signature = config.Signature.ToUpper();

        var result = cipherBuilder.VerifySign(config.Signature, base64String);
        Assert.False(result);
    }

}
