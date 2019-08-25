using System.Collections.Generic;

namespace CryptoAdapter
{
    public class BouncyCastleFactory : CryptoFactory{
        protected override CustomCipherFactory CreateCryptor(Dictionary<string, Dictionary<string, string>> config) {
            CustomCipherFactory customCipherFactory = null;
            
            if (config.ContainsKey("Cipher"))
                customCipherFactory = new BcCipher(config["Cipher"]);
            else
                customCipherFactory = new BcPbe(config["PBE"]);
            
            return customCipherFactory; 
        }

        protected override DigestFactory CreateDigestor(Dictionary<string, Dictionary<string, string>> config)
        {
            DigestFactory digestFactory = new BcDigest(config["Integrity"]);
            
            return digestFactory; 
        }

        protected override CertificateFactory CreateCertifier(Dictionary<string, Dictionary<string, string>> config)
        {
            CertificateFactory certificateFactory = new BcCertificate(config["Integrity"]);

            return certificateFactory; 
        }
    }
    
}