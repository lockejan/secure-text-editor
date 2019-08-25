using System;

namespace CryptoAdapter
{
    public class BcDigest : DigestFactory
    {
        public override void CreateDigest(string config)
        {
            var configs = config.Split('/');
            Console.WriteLine("Digest-Generierung startet");
            foreach (var entries in configs)
            {
                Console.WriteLine($"- {entries}");
            }
        }

        
        //using Org.BouncyCastle.Crypto.Digests;
//using Org.BouncyCastle.Crypto.Macs;
//using Org.BouncyCastle.Crypto.Parameters;
//
//namespace SecureTextEditor
//{
//    public class SteDigestFactory
//    {
//        public byte[] SHA256(byte[] data)
//        {
//            Sha256Digest sha256 = new Sha256Digest();
//            sha256.BlockUpdate(data, 0, data.Length);
//            byte[] hash = new byte[sha256.GetDigestSize()];
//            sha256.DoFinal(hash, 0);
//            return hash;
//        }
//        
//        public byte[] HMacSha256(byte[] data)
//        {
//            Sha256Digest sha256 = new Sha256Digest();
//            HMac hMac = new HMac(sha256);
//            
//            KeyParameter keyParam = new KeyParameter(_myKey);
//            
//            hMac.Init(keyParam);
//            
//            hMac.BlockUpdate(data, 0, data.Length);
//            
//            byte[] hash = new byte[hMac.GetMacSize()];
//
//            hMac.DoFinal(hash,0);
//
//            return hash;
//        }
//        
//        public byte[] AesCMac(byte[] data)
//        {
//            CMac mac = new CMac(_myAes);
//            
//            KeyParameter keyParam = new KeyParameter(_myKey);
//            
//            mac.Init(keyParam);
//            
//            mac.BlockUpdate(data, 0, data.Length);
//            
//            byte[] hash = new byte[mac.GetMacSize()];
//            
//            mac.DoFinal(hash,0);
//
//            return hash;
//        }
//    }
//}
        
    }  
}