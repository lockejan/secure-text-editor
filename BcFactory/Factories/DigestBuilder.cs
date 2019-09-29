using System;
using System.Text;
using BcFactory.Resources;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    public class DigestBuilder : IIntegrity
    {
        private readonly CryptoConfig _config;
        
        private readonly AesEngine _myAes;
        private const string AesAlgo= "AES"; 
        private const string AesKeySize = "256"; 
        public DigestBuilder(CryptoConfig config)
        {
            _config = config;
            _myAes = new AesEngine();
            
            if (_config.DigestKey == null)
                GenerateKey(AesAlgo + AesKeySize);
        }

        private void GenerateKey(string cipher)
        {
            var gen = GeneratorUtilities.GetKeyGenerator(cipher);
            _config.DigestKey = gen.GenerateKey();
        }
        
        public CryptoConfig SignBytes(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            var digestBytes = _config.IntegrityOptions switch
            {
                IntegrityOptions.Sha256 => Sha256(inputBytes),
                IntegrityOptions.AesCmac => AesCMac(inputBytes),
                IntegrityOptions.HmacSha256 => HMacSha256(inputBytes),
                _ => throw new ArgumentException("Unsupported digest.")
            };
            
            _config.Signature = Convert.ToBase64String(digestBytes);
            return _config;
        }

        public bool VerifySign(string sign)
        {
            var result = sign != SignBytes(_config.Cipher).Signature;
            return result;
        }

        private byte[] Sha256(byte[] inputBytes)
        {
            Sha256Digest sha256 = new Sha256Digest();
            
            sha256.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] hash = new byte[sha256.GetDigestSize()];
            sha256.DoFinal(hash, 0);
            
            return hash;
        }

        private byte[] AesCMac(byte[] inputBytes)
        {
            CMac mac = new CMac(_myAes);
            KeyParameter keyParam = new KeyParameter(_config.DigestKey);
            
            mac.Init(keyParam);
            mac.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] hash = new byte[mac.GetMacSize()];
            mac.DoFinal(hash,0);

            return hash;
        }

        private byte[] HMacSha256(byte[] inputBytes)
        {
            Sha256Digest sha256 = new Sha256Digest();
            HMac hMac = new HMac(sha256);
            KeyParameter keyParam = new KeyParameter(_config.DigestKey);
            
            hMac.Init(keyParam);
            hMac.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] hash = new byte[hMac.GetMacSize()];
            hMac.DoFinal(hash,0);

            return hash;
        }
        
    }
}