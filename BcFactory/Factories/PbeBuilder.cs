using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    internal class PbeBuilder
    {
        private readonly CryptoConfig _config;
        private readonly byte[] _salt;
        
        public PbeBuilder(CryptoConfig config)
        {
            _config = config;
            _salt = GenerateSalt();
        }
        
        private byte[] GenerateSalt()
        {
            SecureRandom random = new SecureRandom();
            var salt = new byte[_config.PbePassword.Length];
            random.NextBytes(salt);
            return salt;
        }
        
        public byte[] EncryptTextToBytes(string content)
        {
            var passwordChars = content.ToCharArray();
            byte[] keyBytes;
            
            switch (_config.PbeAlgorithm)
            {
                case PbeAlgorithm.SCRYPT:
                    Console.WriteLine("SCrypt key:");
                    keyBytes = BcScrypt(passwordChars, _salt, 8, 128, 8);
                    break;
                default:
                    Console.WriteLine("PBKDF2 key:");
                    keyBytes = _config.CipherAlgorithm == CipherAlgorithm.RC4 ? BcPKCS5Scheme(passwordChars, _salt,128) : BcPKCS5Scheme2(passwordChars, _salt,128);
                    break;
            }
            Console.WriteLine(Convert.ToBase64String(keyBytes));
            return keyBytes;
        }

        public string DecryptBytesToText(byte[] cipherBytes)
        {
            Console.WriteLine("Decrypting...");
            return "";
        }

        private byte[] BcScrypt(char[] password, byte[] salt,
            int costParameter, int blocksize,
            int parallelizationParam)
        {
            return SCrypt.Generate(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password),
                salt, costParameter, blocksize, parallelizationParam,
                256 / 8);
        }

        private byte[] BcPKCS5Scheme2(char[] password, byte[] salt,
            int iterationCount)
        {
            Pkcs5S1ParametersGenerator generator = new Pkcs5S1ParametersGenerator(
                new Sha256Digest());

            generator.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password),
                salt,
                iterationCount);
            
            return ((KeyParameter)generator.GenerateDerivedParameters(_config.CipherAlgorithm.ToString(),256)).GetKey();
        }

        private byte[] BcPKCS5Scheme(char[] password, byte[] salt,
            int iterationCount)
        {
            Pkcs5S1ParametersGenerator generator = new Pkcs5S1ParametersGenerator(
                new Sha1Digest());

            generator.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password),
                salt,
                iterationCount);
            return ((KeyParameter)generator.GenerateDerivedParameters(_config.CipherAlgorithm.ToString(),160)).GetKey();
        }

    }
}