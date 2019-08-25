using System.Collections.Generic;

namespace CryptoAdapter
{
    public abstract class CryptoFactory {
        public CustomCipherFactory GetCipher(Dictionary<string, Dictionary<string, string>> cryptoConfig) { 
            CustomCipherFactory ccf = CreateCryptor(cryptoConfig);
            return ccf; 
        }
        public DigestFactory GetDigest(Dictionary<string, Dictionary<string, string>> digestParams) { 
            DigestFactory df = CreateDigestor(digestParams);
            return df; 
        }
        public CertificateFactory GetCert(Dictionary<string, Dictionary<string, string>> certParams) { 
            CertificateFactory cf = CreateCertifier(certParams);
            return cf; 
        }
        
        protected abstract CustomCipherFactory CreateCryptor(Dictionary<string, Dictionary<string, string>> cryptoConfig); 
        protected abstract DigestFactory CreateDigestor(Dictionary<string, Dictionary<string, string>> cryptoConfig); 
        protected abstract CertificateFactory CreateCertifier(Dictionary<string, Dictionary<string, string>> cryptoConfig);
    }
        
    public abstract class CustomCipherFactory {
        public abstract byte[] EncryptTextToBytes(string plainText);
        public abstract string DecryptBytesToText(byte[] cipherBytes);
        public abstract Dictionary<string, string> Result();
    }
    
    public abstract class DigestFactory {
        public abstract void CreateDigest(string config);
    }
    
    public abstract class CertificateFactory {
        public abstract void CreateCert(string config);
        public abstract void GetSign(string config);
        public abstract bool VerifySign(string sign, byte[] input);
    }
}