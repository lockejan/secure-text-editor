using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAdapter
{
    public class BouncyCastleFactory : CryptoFactory{
        protected override CustomCipherFactory CreateCryptor(Dictionary<string, Dictionary<string, string>> config) {
            CustomCipherFactory customCipherFactory = null;

//            var boundary = cryptoTask.IndexOf('/');
//            var type = cryptoTask.Substring(0, boundary);
            var type = config.Keys.First();
//            var config = cryptoTask.Substring(boundary + 1);
            
            switch (type)
            {
                case "Cipher":
                    customCipherFactory = new BcCipher(config["Cipher"]);
                    break;
                case "PBE":
                    customCipherFactory = new BcPbe();
                    break;
                default:
                    Console.WriteLine($"{type} Cipher is not available in the factory");
                    break;
            }
            return customCipherFactory; 
        }

        protected override DigestFactory CreateDigestor(string cryptoTask)
        {
            DigestFactory digestFactory = null;

            switch (cryptoTask)
            {
                case "Digest":
                    digestFactory = new BcDigest();
                    break;
                default:
                    Console.WriteLine($"{cryptoTask} Digest is not available in the factory");
                    break;
            }
            return digestFactory; 
        }

        protected override CertificateFactory CreateCertifier(string cryptoTask)
        {
            CertificateFactory certificateFactory = null;

            switch (cryptoTask)
            {
                case "Cert":
                    certificateFactory = new BcCertificate();
                    break;
                default:
                    Console.WriteLine($"{cryptoTask} Cert-Type is not available in the factory");
                    break;
            }
            return certificateFactory; 
        }
    }
    
}