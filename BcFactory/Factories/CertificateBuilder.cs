using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace BcFactory.Factories
{
    public class CertificateBuilder : ICert
    {
        private readonly CryptoConfig _config;

        private AsymmetricKeyParameter _privateKey;
        private AsymmetricKeyParameter _publicKey;
        
        public CertificateBuilder(CryptoConfig config)
        {
            _config = config;

            if (_config.SignaturePublicKey == null) return;
            var publicKeyDerRestored = Convert.FromBase64String(_config.SignaturePublicKey);
            _publicKey = PublicKeyFactory.CreateKey(publicKeyDerRestored);

        }

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

        public CryptoConfig SignInput(string input)
        {
            var cipherBytes = Convert.FromBase64String(_config.Cipher);

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
        
        public bool VerifySign(string sign)
        {
            var messageBytes = Convert.FromBase64String(_config.Cipher);
            var signer = SignerUtilities.GetSigner("SHA256withDSA");

            signer.Init(false, _publicKey);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            
            var decodedBytes = Convert.FromBase64String(_config.Signature);
            
            return signer.VerifySignature(decodedBytes);
        }

    }
}