using System;

namespace CryptoAdapter
{

    public class BcPbe : CustomCipherFactory
    {

        public override byte[] EncryptTextToBytes(string config)
        {
            var configs = config.Split('/');
            Console.WriteLine("PBE-Encryption startet");
            foreach (var entries in configs)
            {
                Console.WriteLine($"- {entries}");
            }
            return new byte[16];
        }

        public override string DecryptBytesToText(byte[] cipherBytes)
        {
            throw new NotImplementedException();
        }
    }
}