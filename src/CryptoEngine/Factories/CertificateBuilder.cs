using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace CryptoEngine.Factories
{
    /// <inheritdoc />
    internal class CertificateBuilder : ICert
    {
        private readonly CryptoConfig _config;

        private AsymmetricKeyParameter _privateKey;
        private AsymmetricKeyParameter _publicKey;

        /// <inheritdoc />
        public CertificateBuilder(CryptoConfig config)
        {
            _config = config;

            if (_config.SignaturePublicKey == null) return;
            var publicKeyDerRestored = Convert.FromBase64String(_config.SignaturePublicKey);
            _publicKey = PublicKeyFactory.CreateKey(publicKeyDerRestored);

        }

        /// <inheritdoc />
        public void GenerateCerts()
        {
            var dsaKeyPairGen = new DsaKeyPairGenerator();
            var dsaParamGen = new DsaParametersGenerator();
            
            dsaParamGen.Init(512,12, new SecureRandom());
            dsaKeyPairGen.
                Init(new DsaKeyGenerationParameters(new SecureRandom(),
                dsaParamGen.GenerateParameters()));
            
            var keyPair = dsaKeyPairGen.GenerateKeyPair();
                
            _privateKey = keyPair.Private;
            _publicKey = keyPair.Public;
            
            var publicKeyDer = SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(_publicKey)
                .GetDerEncoded();
            
            var publicKeyDerBase64 = Convert.ToBase64String(publicKeyDer);
            
            _config.SignaturePublicKey = publicKeyDerBase64;
        }

        /// <inheritdoc />
        public CryptoConfig SignInput(string message)
        {
            var cipherBytes = Convert.FromBase64String(message);

            try
            {
                // Generation of digital signature
                var signer = SignerUtilities.GetSigner("SHA256withDSA");
                signer.Init(true, _privateKey);
                signer.BlockUpdate(cipherBytes, 0, cipherBytes.Length);
            
                var signature = signer.GenerateSignature();
                _config.Signature = Convert.ToBase64String(signature);
            }
            catch (Exception e)
            {
                throw new ArgumentException("error generating signature - " + e);
            }
            
            return _config;
        }

        /// <inheritdoc />
        public bool VerifySign(string sign, string message)
        {
            var messageBytes = Convert.FromBase64String(message);
            var signer = SignerUtilities.GetSigner("SHA256withDSA");

            signer.Init(false, _publicKey);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            
            var decodedBytes = Convert.FromBase64String(sign);
            
            return signer.VerifySignature(decodedBytes);
        }

    }
}