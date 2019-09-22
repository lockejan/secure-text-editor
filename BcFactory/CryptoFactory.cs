using System;

namespace BcFactory
{

//        public static class CryptoFactory
//        {
//            public static Crypt Create(CryptoConfig settings)
//            {
//              if (settings.IsEncryptActive && settings.Algorithm == CipherAlgorithm.AES)
//                  return new AesCipher(settings);
//              throw new NotSupportedException("The given settings combination is not supported");
//            }
//        }
//
//        abstract class Cipher
//        {
//            protected readonly CryptoConfig _settings;
//
//            public Cipher(CryptoConfig settings)
//            {   _settings = settings ?? throw new ArgumentNullException(nameof(settings));
//            }
//
//            public abstract string Encrypt(string content);
//            public abstract string Decrypt(string content);
//        }
//
//        class AesCipher : Cipher
//        {
//            public AesCipher(CryptoConfig settings) : base(settings)
//            {
//            }
//
//            public override string Encrypt(string content)
//            {
//                return $"encrypting...{content}";
//            }
//
//            public override string Decrypt(string content)
//            {
//                return $"decrypting...{content}";
//            }
//        }

    public static class CryptoFactory
    {
        public static ICrypto Create(CryptoConfig settings)
        {
            if (settings.IsEncryptActive)
            {
                if (settings.IsPbeActive)
                    return new PbeCipherBuilder(settings);
                return new CipherBuilder(settings);
            }
//            throw new NotSupportedException("The given settings combination is not supported");
            return null;
        }
    }
    
    public static class IntegrityFactory
    {
        public static IIntegrity Create(CryptoConfig settings)
        {
            if(settings.IsIntegrityActive)
            {
                if(settings.Integrity == Integrity.Digest)
                    return new DigestBuilder(settings);
                return new CertificateBuilder(settings);
            }
            return null;
        }
    }

    public interface ICrypto
    {
        byte[] EncryptTextToBytes(string content);
        string DecryptBytesToText(byte[] cipherBytes);
    }

    public interface IIntegrity
    {
        byte[] SignBytes(string content);
        string VerifySign(string sign, string input);
    }
 
}