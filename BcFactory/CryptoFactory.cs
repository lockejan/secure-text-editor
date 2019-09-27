using System.Text;
using BcFactory.Factories;

namespace BcFactory
{
    /// <summary>
    /// Factory class for Cipher Processing.
    /// </summary>
    public static class CryptoFactory
    {
        /// <summary>
        /// Gets a CryptoProcessor-Object from some factory.
        /// </summary>
        /// <param name="settings">CryptoConfig Object which holds all parameters needed for processing</param>
        /// <returns>Object of some cryptoProcessing class.</returns>
        public static ICrypto Create(CryptoConfig settings)
        {
            if (settings.IsEncryptActive)
                return new CipherBuilder(settings);

            return new NoopCrypto();
        }
    }

    /// <summary>
    /// Factory class for Digests and Certificates.
    /// </summary>
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
    /// Interface which defines essential functions to.
    /// </summary>
    public interface IIntegrity
    {
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