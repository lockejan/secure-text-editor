using System;

namespace BcFactory
{
    public class CertificateBuilder : IIntegrity
    {
        public CertificateBuilder(CryptoConfig config)
        {
            
        }
        
        public byte[] SignBytes(string content)
        {
            Console.WriteLine("Signing process started...");
            return new byte[1];
        }

        public string VerifySign(string sign, string input)
        {
            Console.WriteLine("Verification process started");
            return "verification...?";
        }
        
        public void CreateCert(string config)
        {
            Console.WriteLine("Certs are being generated");
        }

    }
}