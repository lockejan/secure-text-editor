using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    public class CertificateBuilder : ICert
    {
        private CryptoConfig _config;
        
        //private byte[] toBeSealedInput = Encoding.UTF8.GetBytes("Hallo cipher");
        public CertificateBuilder(CryptoConfig config)
        {
            _config = config;
        }

        public CryptoConfig CreateCertificate(string content)
        {
            Console.WriteLine("Signing process started...");
            var cipherBytes = Convert.FromBase64String(_config.Cipher);

            try
            {
				DsaKeyPairGenerator dsaKeyPairGen = new DsaKeyPairGenerator();
                DsaParametersGenerator dsaParamGen = new DsaParametersGenerator();
                dsaParamGen.Init(512,12, new SecureRandom());
                dsaKeyPairGen.Init(new DsaKeyGenerationParameters(new SecureRandom(),dsaParamGen.GenerateParameters()));
                AsymmetricCipherKeyPair keyPair = dsaKeyPairGen.GenerateKeyPair();
                
                AsymmetricKeyParameter privateKey = keyPair.Private;
                AsymmetricKeyParameter publicKey = keyPair.Public;
                
                //To print the public key in pem format
/*                TextWriter textWriter = new StringWriter();
                var pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(publicKey);
                pemWriter.Writer.Flush();

                Console.WriteLine($"Private key is: {privateKey.GetHashCode()}");
                Console.WriteLine($"Public key is: {publicKey}");
*/

                // Generation of digital signature
//                ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.sa)
                ISigner sign = SignerUtilities.GetSigner("SHA256withDSA");
                sign.Init(true, privateKey);
                sign.BlockUpdate(cipherBytes, 0, cipherBytes.Length);
                
                byte[] signature = sign.GenerateSignature();
                _config.Signature = Convert.ToBase64String(signature);
                _config.SignaturePrivateKey = privateKey.ToString();
                _config.SignaturePublicKey = publicKey.ToString();
                
                Console.WriteLine($"Public key is: {Convert.ToBase64String(signature)}");

//                DsaParametersGenerator dParamGen = new DsaParametersGenerator();
//                dParamGen.Init(512, 25, new SecureRandom());
//                IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("DSA");
//                keyPairGenerator.Init(new DsaKeyGenerationParameters(new SecureRandom(), dParamGen.GenerateParameters()));
//                AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
//
//                AsymmetricKeyParameter privKey = keyPair.Private;
//                AsymmetricKeyParameter pubKey = keyPair.Public;
            }
            catch (Exception e)
            {
                throw new ArgumentException("error setting up keys - " + e.ToString());
            }

            //return new byte[1];
            return _config;
        }
        
//
//        private AsymmetricCipherKeyPair GetKeyPair()
//        {
//            DsaKeyPairGenerator dsaKey = new DsaKeyPairGenerator();
//            
//            KeyGenerationParameters keyParam = new KeyGenerationParameters(new SecureRandom(),1024);
//            
//            dsaKey.Init(keyParam);
//            Console.WriteLine(dsaKey.GenerateKeyPair());
//        }
        
//        private AsymmetricCipherKeyPair GetKeyPair()
//        {
//            DsaParametersGenerator parametersGenerator = new DsaParametersGenerator(new Sha256Digest());
//            parametersGenerator.Init(new DsaParameterGenerationParameters(2048,GetParameterN(2048),192, new SecureRandom()));
//            var parameters = parametersGenerator.GenerateParameters();
//            
//            DsaKeyPairGenerator dsaKeyPairGenerator = new DsaKeyPairGenerator();
//            dsaKeyPairGenerator.Init(new DsaKeyGenerationParameters(new SecureRandom(),parameters));
//            return dsaKeyPairGenerator.GenerateKeyPair();
//        }
//
//        private SignatureKeyPair GenerateKeyPair()
//        {
//            AsymmetricCipherKeyPair keyPair = GetKeyPair();
//
//            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Public);
//            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
//            byte[] privateEncoded = privateKeyInfo.GetEncoded();
//            byte[] publicEncoded = publicKeyInfo.GetEncoded();
//
//            return new SignatureKeyPair(privateEncoded, publicEncoded, keyPair);
//        }
//
//        private void dummy()
//        {
////            PrivateKeyFactory.CreateKey();
////            PublicKeyFactory.CreateKey();
//            
//            Signature signature = Signature.GetInstance("SHA256withDSA");
//            signature.
//        }
        public void GenerateCerts()
        {
            throw new NotImplementedException();
        }

        public CryptoConfig SignBytes(string content)
        {
            throw new NotImplementedException();
        }

        public bool VerifySign(string sign)
        {
            throw new NotImplementedException();
        }
    }
}