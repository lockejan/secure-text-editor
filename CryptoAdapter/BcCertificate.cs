using System;

namespace CryptoAdapter
{
    
    public class BcCertificate : CertificateFactory
    {
        public override void CreateCert(string config)
        {
            var configs = config.Split('/');
            Console.WriteLine("Zertifikate werden erzeugt");
            foreach (var entries in configs)
            {
                Console.WriteLine($"- {entries}");
            }
        }

        public override void GetSign(string config)
        {
            Console.WriteLine("INHALT wird signiert....");
        }
    }
}