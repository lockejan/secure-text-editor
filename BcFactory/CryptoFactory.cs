using System;
using BcFactory.Factories;
using BcFactory.Resources;

namespace BcFactory
{

    public static class CryptoFactory
    {

        public static ICipher CreateCipher(CryptoConfig config)
        {
            if(config.IsEncryptActive)
                return new CipherBuilder(config);

            throw new ArgumentException("Invalid Configuration. Encryption not activated.'");
        }

        public static IPbe CreatePbe(CryptoConfig config)
        {
            if(!config.IsPbeActive) 
                throw new ArgumentException("Invalid Configuration. Pbe not activated."); 
            
            if(config.PbePassword == null)
                throw new ArgumentException("Pbe not properly configured. Empty password is not allowed.");
            
            return new PbeBuilder(config);
        }

        public static IDigest CreateDigest(CryptoConfig config)
        {
            if (!config.IsIntegrityActive) 
                throw new ArgumentException("Integrity not activated!");
            
            if (config.Integrity == Integrity.Digest)
                return new DigestBuilder(config);
            
            throw new ArgumentException("Unsupported digest mode!");
        }

        public static ICert CreateCert(CryptoConfig config)
        {
            if (!config.IsIntegrityActive)
                throw new ArgumentException("Integrity not activated!");
            
            if (config.Integrity == Integrity.Dsa)
                return new CertificateBuilder(config);
            
            throw new ArgumentException("Unsupported certificate mode!");
        }
    }
    
    public interface ICipher
    {
        CryptoConfig EncryptTextToBytes(string content);
        
        string DecryptBytesToText(byte[] cipherBytes);
    }

    public interface IPbe
    {
        CryptoConfig GenerateKeyBytes();
    }
    
    public interface IDigest
    {
        CryptoConfig SignInput(string message);
        
        bool VerifySign(string sign, string message);
    }

    public interface ICert
    {
        void GenerateCerts();
        CryptoConfig SignInput(string message);
        
        bool VerifySign(string sign, string message);
    }

}