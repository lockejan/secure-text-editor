using System.Collections.Generic;

namespace CryptoAdapter
{
    public abstract class CryptoFactory {
        public CustomCipherFactory GetCipher(Dictionary<string, Dictionary<string, string>> cryptoConfig) { 
            CustomCipherFactory cf = CreateCryptor(cryptoConfig);
            return cf; 
        }
        public DigestFactory GetDigest(string digestParams) { 
            DigestFactory df = CreateDigestor(digestParams);
            return df; 
        }
        public CertificateFactory GetCert(string certParams) { 
            CertificateFactory cf = CreateCertifier(certParams);
            return cf; 
        }
        
        //Definition der Factory Method 
        protected abstract CustomCipherFactory CreateCryptor(Dictionary<string, Dictionary<string, string>> cryptoConfig); 
        protected abstract DigestFactory CreateDigestor(string cryptoTask); 
        protected abstract CertificateFactory CreateCertifier(string cryptoTask);
    }
        
    public abstract class CustomCipherFactory {
        public abstract byte[] EncryptTextToBytes(string plainText);
        public abstract string DecryptBytesToText(byte[] cipherBytes);
    }
    public abstract class DigestFactory {
        public abstract void CreateDigest(string config);
    }
    public abstract class CertificateFactory {
        public abstract void CreateCert(string config);
        public abstract void GetSign(string config);
    }
}