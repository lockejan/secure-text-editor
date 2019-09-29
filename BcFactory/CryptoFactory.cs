using System;
using BcFactory.Factories;
using BcFactory.Resources;

namespace BcFactory
{

    public static class CryptoFactory
    {

        public static ICipher CreateCipher(CryptoConfig settings)
        {
            if(settings.IsEncryptActive)
                return new CipherBuilder(settings);

            throw new ArgumentException("Invalid Configuration. Encryption not activated.'");
        }

        public static IPbe CreatePbe(CryptoConfig settings, char[] password)
        {
            if(!settings.IsPbeActive) 
                throw new ArgumentException("Invalid Configuration. Pbe not activated."); 
            
            if(settings.PbePassword == null)
                throw new ArgumentException("Pbe not properly configured. Empty password is not allowed.");
            
            return new PbeBuilder(settings);
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
        CryptoConfig GenerateKeyBytes(char[] password);
    }
    
    public interface IDigest
    {
        CryptoConfig SignInput(string input);
        
        bool VerifySign(string sign);
    }

    public interface ICert
    {
        void GenerateCerts();
        CryptoConfig SignInput(string input);
        
        bool VerifySign(string sign);
    }

}