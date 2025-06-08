using System.Text;
using CryptoEngine.Resources;

namespace CryptoEngine.Tests;

public class CertTests
{

    [Fact]
    public void TestInvalidCertCreateCall()
    {
        var config = new CryptoConfig();

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateCert(config));
    }

    [Fact]
    public void TestInvalidCertCreateConfig()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Digest
        };

        Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateCert(config));
    }

    //######################
    //Dsa Cert Combinations
    //######################

    [Fact]
    public void TestValidDsa256Cert()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Dsa,
            IntegrityOptions = IntegrityOptions.Sha256
        };

        var certBuilder = CryptoFactory.CreateCert(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        certBuilder.GenerateCerts();
        config = certBuilder.SignInput(base64String);

        var result = certBuilder.VerifySign(config.Signature, base64String);
        Assert.True(result);
    }

    [Fact]
    public void TestInvalidDsa256Signature()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Dsa,
            IntegrityOptions = IntegrityOptions.Sha256
        };

        var certBuilder = CryptoFactory.CreateCert(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        certBuilder.GenerateCerts();
        config = certBuilder.SignInput(base64String);
        config.Signature = config.Signature.ToUpper();

        var result = certBuilder.VerifySign(config.Signature, base64String);
        Assert.False(result);
    }

    [Fact]
    public void TestMissingPrivateKeyDsa256()
    {
        var config = new CryptoConfig
        {
            IsIntegrityActive = true,
            Integrity = Integrity.Dsa,
            IntegrityOptions = IntegrityOptions.Sha256
        };

        var certBuilder = CryptoFactory.CreateCert(config);
        var bytes = Encoding.UTF8.GetBytes("Hallo Welt");
        var base64String = Convert.ToBase64String(bytes);

        Assert.Throws<ArgumentException>(
                () => certBuilder.SignInput(base64String));
    }

}
