using System;
using System.Text;

namespace BcFactory
{
    public static class CryptoFactory
    {
        public static ICrypto Create(CryptoConfig settings)
        {
            if (settings.IsEncryptActive)
                return new CipherBuilder(settings);

            return new NoopCrypto();
        }
    }

    public static class IntegrityFactory
    {
        public static IIntegrity Create(CryptoConfig settings)
        {
            if (settings.IsIntegrityActive)
            {
                if (settings.Integrity == Integrity.Digest)
                    return new DigestBuilder(settings);

                return new CertificateBuilder(settings);
            }
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICrypto
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        byte[] EncryptTextToBytes(string content);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        string DecryptBytesToText(byte[] cipherBytes);
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoopCrypto : ICrypto
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] EncryptTextToBytes(string content)
        {
            return Encoding.Default.GetBytes(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public string DecryptBytesToText(byte[] cipherBytes)
        {
            return Encoding.Default.GetString(cipherBytes);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IIntegrity
    {
        void CreateCertificate();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        byte[] SignBytes(string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        bool VerifySign(string sign, string input);
    }

}