using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;

namespace BcFactory
{
    public class CertificateBuilder : IIntegrity
    {
        private byte[] tmpSource = Encoding.UTF8.GetBytes("Hallo cipher");
        public CertificateBuilder(CryptoConfig config)
        {
            
        }
        
        public byte[] SignBytes(string content)
        {
            Console.WriteLine("Signing process started...");
//            KeyPairGen();
            try
            {
				DsaKeyPairGenerator dsaKeyPairGen = new DsaKeyPairGenerator();
                DsaParametersGenerator dsaParamGen = new DsaParametersGenerator();
                dsaParamGen.Init(512,12, new SecureRandom());
                dsaKeyPairGen.Init(new DsaKeyGenerationParameters(new SecureRandom(),dsaParamGen.GenerateParameters()));
//                AsymmetricCipherKeyPair keyPair = dsaKeyPairGen.GenerateKeyPair();
                AsymmetricCipherKeyPair keyPair = dsaKeyPairGen.GenerateKeyPair();

//                DsaKeyParameters privateKey = (DsaKeyParameters)keyPair.Private;
//                DsaKeyParameters publicKey = (DsaKeyParameters)keyPair.Public;
                AsymmetricKeyParameter privateKey = keyPair.Private;
                AsymmetricKeyParameter publicKey = keyPair.Public;
                
                //To print the public key in pem format
                TextWriter textWriter = new StringWriter();
                var pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(publicKey);
                pemWriter.Writer.Flush();

                Console.WriteLine($"Private key is: {privateKey.GetHashCode()}");
                Console.WriteLine($"Public key is: {publicKey}");


                // Generation of digital signature
//                ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.sa)
                ISigner sign = SignerUtilities.GetSigner("SHA256withDSA");
                sign.Init(true, privateKey);
                sign.BlockUpdate(tmpSource, 0, tmpSource.Length);
                byte[] signature = sign.GenerateSignature();
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

            return new byte[1];
        }

        public string VerifySign(string sign, string input)
        {
            Console.WriteLine("Verification process started");
            return "verification...?";
        }
        
        public void CreateCert(string config)
        {
            Console.WriteLine("Certs are being generated");
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
    }
}