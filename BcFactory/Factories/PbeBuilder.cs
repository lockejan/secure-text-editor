using System;
using BcFactory.Resources;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    /// <inheritdoc />
    internal class PbeBuilder : IPbe
    {
        private readonly CryptoConfig _config;

        /// <inheritdoc />
        public PbeBuilder(CryptoConfig config)
        {
            _config = config;

            if (_config.IvOrSalt == null)
                _config.IvOrSalt = GenerateSalt();
        }
        
        private byte[] GenerateSalt()
        {
            SecureRandom random = new SecureRandom();
            var salt = new byte[_config.PbePassword.Length];
            random.NextBytes(salt);
            return salt;
        }
        
        CryptoConfig IPbe.GenerateKeyBytes()
        {
            var keyBytes = _config.PbeAlgorithm switch
            {
                PbeAlgorithm.SCRYPT => BcScrypt(_config.PbePassword, _config.IvOrSalt, 8, 128, 8),
                PbeAlgorithm.PBKDF2 => BcPkcs5Scheme(_config.PbePassword, _config.IvOrSalt, 128),
                _ => throw new ArgumentException("Algorithm not supported.")
            };
            _config.Key = keyBytes;
            return _config;
        }

        private byte[] BcScrypt(char[] password, byte[] salt,
            int costParameter, int blocksize, int parallelizationParam)
        {
            return SCrypt.Generate(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password),
                salt, costParameter, blocksize, parallelizationParam,
                256 / 8);
        }


        private byte[] BcPkcs5Scheme(char[] password, byte[] salt,
            int iterationCount)
        {
            var generator = new Pkcs5S1ParametersGenerator(
                GetDigest());

            generator.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password),
                salt,
                iterationCount);
            
            return ((KeyParameter)generator.GenerateDerivedParameters(_config.CipherAlgorithm.ToString(),GetKeySize())).GetKey();
        }
        
        private Org.BouncyCastle.Crypto.IDigest GetDigest()
        {
            return _config.CipherAlgorithm == CipherAlgorithm.RC4
                ? new Sha1Digest()
                : (Org.BouncyCastle.Crypto.IDigest)new Sha256Digest();
        }

        private int GetKeySize()
        {
            return _config.CipherAlgorithm == CipherAlgorithm.RC4
                ? 160
                : 256;
        }

    }
}