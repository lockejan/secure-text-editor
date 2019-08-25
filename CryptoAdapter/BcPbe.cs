using System;
using System.Collections.Generic;

namespace CryptoAdapter
{

    public class BcPbe : CustomCipherFactory
    {
        private Dictionary<string, string> _config;
        public BcPbe(Dictionary<string, string> config)
        {
            _config = config;
        }

        public override byte[] EncryptTextToBytes(string config)
        {
            Console.WriteLine("PBE-Encryption started");
            foreach (var entries in config)
            {
                Console.WriteLine($"- {entries}");
            }
            return new byte[16];
        }

        public override string DecryptBytesToText(byte[] cipherBytes)
        {
            Console.WriteLine("Decrypted Cipher Text...");
            return "Decrypted Cipher Text...";
        }

        public override Dictionary<string, string> Result()
        {
            throw new NotImplementedException();
        }
    }
}