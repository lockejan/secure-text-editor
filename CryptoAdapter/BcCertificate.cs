using System;
using System.Collections.Generic;

namespace CryptoAdapter
{
    
    public class BcCertificate : CertificateFactory
    {
        private Dictionary<string, string> _config;
        public BcCertificate(Dictionary<string, string> config)
        {
            _config = config;
        }
        public override void CreateCert(string config)
        {
            Console.WriteLine("Certs are being generated");
            foreach (var entries in config)
            {
                Console.WriteLine($"- {entries}");
            }
        }

        public override void GetSign(string config)
        {
            Console.WriteLine("Signing process started...");
        }

        public override bool VerifySign(string sign, byte[] input)
        {
            Console.WriteLine("Signature validation failed.");
            return false;
        }
    }
}