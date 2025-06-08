using System;
using CryptoEngine.Factories;
using CryptoEngine.Resources;

namespace CryptoEngine
{

    /// <summary>
    /// Factory class which returns objects from all supported engines.
    /// Also holds interface implementation for builder/engine classes.
    /// </summary>
    public static class CryptoFactory
    {

        /// <summary>
        /// Checks for valid call and then returns cipher engine object.
        /// </summary>
        /// <param name="config">CryptoConfig Object</param>
        /// <returns>CipherEngine object implementing ICipher interface.</returns>
        /// <exception cref="ArgumentException">To handle false calls in frontend exception gets thrown.</exception>
        public static ICipher CreateCipher(CryptoConfig config)
        {
            if(config.IsEncryptActive)
                return new CipherBuilder(config);

            throw new ArgumentException("Invalid Configuration. Encryption not activated.'");
        }

        /// <summary>
        /// Checks for valid call and then returns pbe engine object.
        /// </summary>
        /// <param name="config">CryptoConfig Object</param>
        /// <returns>PbeEngine object implementing IPbe interface.</returns>
        /// <exception cref="ArgumentException">To handle false calls in frontend exception gets thrown.</exception>
        public static IPbe CreatePbe(CryptoConfig config)
        {
            if(!config.IsPbeActive) 
                throw new ArgumentException("Invalid Configuration. Pbe not activated."); 
            
            if(config.PbePassword == null)
                throw new ArgumentException("Pbe not properly configured. Empty password is not allowed.");
            
            return new PbeBuilder(config);
        }

        /// <summary>
        /// Checks for valid call and then returns digest engine object.
        /// </summary>
        /// <param name="config">CryptoConfig Object</param>
        /// <returns>DigestEngine object implementing IDigest interface.</returns>
        /// <exception cref="ArgumentException">To handle false calls in frontend exception gets thrown.</exception>
        public static IDigest CreateDigest(CryptoConfig config)
        {
            if (!config.IsIntegrityActive) 
                throw new ArgumentException("Integrity not activated!");
            
            if (config.Integrity == Integrity.Digest)
                return new DigestBuilder(config);
            
            throw new ArgumentException("Unsupported digest mode!");
        }

        /// <summary>
        /// Checks for valid call and then returns certificate engine object.
        /// </summary>
        /// <param name="config">CryptoConfig Object</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ICert CreateCert(CryptoConfig config)
        {
            if (!config.IsIntegrityActive)
                throw new ArgumentException("Integrity not activated!");
            
            if (config.Integrity == Integrity.Dsa)
                return new CertificateBuilder(config);
            
            throw new ArgumentException("Unsupported certificate mode!");
        }
    }
    
    /// <summary>
    /// Cipher engine interface which declares basic methods which has to be exposed.
    /// </summary>
    public interface ICipher
    {
        /// <summary>
        /// expects message to encrypted and writes resulting bytes to config object.
        /// </summary>
        /// <param name="content">text coming from editor</param>
        /// <returns>updated config state.</returns>
        CryptoConfig EncryptTextToBytes(string content);
        
        /// <summary>
        /// expects cipher bytes to decrypt them.
        /// returns decrypted message.
        /// </summary>
        /// <param name="cipherBytes">encrypted original message.</param>
        /// <returns>original message</returns>
        string DecryptBytesToText(byte[] cipherBytes);
    }

    /// <summary>
    /// Password based encryption engine interface which declares basic methods which have to be exposed and implemented.
    /// </summary>
    public interface IPbe
    {
        /// <summary>
        /// Generates key bytes which are needed for cipher engine.
        /// resulting bytes array gets updated on CryptoConfig object.
        /// </summary>
        /// <returns>updated config state.</returns>
        CryptoConfig GenerateKeyBytes();
    }

    /// <summary>
    /// Main abstraction for certification and digest/mac engines.
    /// Declares basic methods which have to be exposed and implemented.
    /// </summary>
    public interface IIntegrity
    {
        /// <summary>
        /// Signing message and returning it via updated config object.
        /// </summary>
        /// <param name="message">text or cipher text.</param>
        /// <returns>updated config object.</returns>
        CryptoConfig SignInput(string message);
        
        /// <summary>
        /// Verification of signature or digest.
        /// creates another sign to compare it
        /// </summary>
        /// <param name="sign">string representation of signature base64 encoded.</param>
        /// <param name="message">cipher or text in string representation.</param>
        /// <returns>bool representing outcome of verification.</returns>
        bool VerifySign(string sign, string message);   
    }

    /// <inheritdoc />
    public interface IDigest : IIntegrity {}

    /// <inheritdoc />
    public interface ICert : IIntegrity
    {
        /// <summary>
        /// triggers creation of certificates.
        /// needs to be called before signing.
        /// </summary>
        void GenerateCerts();
    }

}