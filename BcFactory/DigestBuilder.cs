using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory
{
    public class DigestBuilder : IIntegrity
    {
        private readonly CryptoConfig _config;
        private byte[] _myKey;
        private readonly AesEngine _myAes;
        private const string AesAlgo= "AES"; 
        private const string AesKeySize = "256"; 
        public DigestBuilder(CryptoConfig config)
        {
            _config = config;
            _myAes = new AesEngine();
            // TODO: check which combo of aes is usually used
            GenerateKey(AesAlgo + AesKeySize);
        }

        private void GenerateKey(string cipher)
        {
            var gen = GeneratorUtilities.GetKeyGenerator(cipher);
            _myKey = gen.GenerateKey();
        }

        public byte[] SignBytes(string content)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(content);
            byte[] digestBytes;
            
            switch (_config.IntegrityOptions)
            {
                case IntegrityOptions.Sha256:
                    Console.WriteLine("Sha256 digest:");
                    digestBytes = Sha256(textBytes);
                    break;
                case IntegrityOptions.AesCmac:
                    Console.WriteLine("AESCMAC digest:");
                    digestBytes = AesCMac(textBytes);
                    break;
                default:
                    Console.WriteLine("HMacSha256 digest:");
                    digestBytes = HMacSha256(textBytes);
                    break;
            }
            Console.WriteLine(Convert.ToBase64String(digestBytes));
            return digestBytes;
        }

        public string VerifySign(string sign, string input)
        {
            Console.WriteLine("Digest-Verifikation startet");
            bool result = sign != Convert.ToBase64String(SignBytes(input));
            return $"Integrity tampered: {result}";
        }

        private byte[] Sha256(byte[] data)
        {
            Sha256Digest sha256 = new Sha256Digest();
            
            sha256.BlockUpdate(data, 0, data.Length);
            byte[] hash = new byte[sha256.GetDigestSize()];
            sha256.DoFinal(hash, 0);
            
            return hash;
        }

        private byte[] AesCMac(byte[] data)
        {
            CMac mac = new CMac(_myAes);
            KeyParameter keyParam = new KeyParameter(_myKey);
            
            mac.Init(keyParam);
            mac.BlockUpdate(data, 0, data.Length);
            byte[] hash = new byte[mac.GetMacSize()];
            mac.DoFinal(hash,0);

            return hash;
        }

        private byte[] HMacSha256(byte[] data)
        {
            Sha256Digest sha256 = new Sha256Digest();
            HMac hMac = new HMac(sha256);
            KeyParameter keyParam = new KeyParameter(_myKey);
            
            hMac.Init(keyParam);
            hMac.BlockUpdate(data, 0, data.Length);
            byte[] hash = new byte[hMac.GetMacSize()];
            hMac.DoFinal(hash,0);

            return hash;
        }
        
    }
}