using System;

namespace BcFactory
{
    public static class KeySize
    {
        public static readonly int[] AES = {128, 192, 256};
        public static readonly int[] RC4 = {40};
//        public static readonly int[] RC4 = {40, 128, 256, 512, 1024, 2048};
    }
    
    /// <summary>
    /// Holds all available PBE algorithm which can be used by the editor. 
    /// </summary>
    public enum PbeAlgorithm{PBKDF2, SCRYPT}
    
    /// <summary>
    /// Holds all PBE possible forms of hashes or other algorithms which provide any sort of integrity. 
    /// </summary>
    public enum PbeDigest{GCM, SHA1, SHA256}
    
    /// <summary>
    /// Holds all available Cipher algorithm which can be used by the editor.
    /// Just a handful of symmetric and stream ciphers are currently available. 
    /// </summary>
    public enum CipherAlgorithm{AES,RC4}
    
    /// <summary>
    /// Holds all available Blockmodes which can be used with symmetric cipher algorithms.
    /// </summary>
    public enum BlockMode{None,ECB,CBC,GCM,OFB,CTS}
    
    /// <summary>
    /// Holds all sort of paddings which are available for cipher processing. 
    /// </summary>
    public enum Padding
    {
        None = 0,
        Pkcs7 = 1,
        ZeroByte = 2
    }
    
    /// <summary>
    /// Holds all available forms of integrity related functions.
    /// For e.g. Digests and PublicKeyCryptography like DSA. 
    /// </summary>
    public enum Integrity{Digest,Dsa};
    
    /// <summary>
    /// All available Digests and Algorithms which are available to be used with DSA or just as Digest.
    /// Not all combinations are possible. The related business logic is implemented somewhere else. 
    /// </summary>
    public enum IntegrityOptions
    {
        Sha256 = 0,
        AesCmac = 1,
        HmacSha256 = 2,
    };

    /// <summary>
    /// Configuration class which holds all relevant attributes for further processing.
    /// Properties are just having basic getter and setter.
    /// ToString-method is customized to display state of attributes for debugging.
    /// </summary>
    public class CryptoConfig
    {
        public bool IsEncryptActive { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public int KeySize { get; set; }
        public BlockMode BlockMode { get; set; }
        public Padding Padding { get; set; }
        public bool IsPbeActive { get; set; }
        public PbeAlgorithm PbeAlgorithm { get; set; }
        public PbeDigest PbeDigest { get; set; }
        public char[] PbePassword { get; set; }
        public bool IsIntegrityActive { get; set; }
        public Integrity Integrity { get; set; }
        public IntegrityOptions IntegrityOptions { get; set; }
        
        public override string ToString()
        {
            return $"IsEncrypActive: {IsEncryptActive},\n" +
                   $"IsPbeActive: {IsPbeActive},\n" +
                   $"PbePassword: {PbePassword},\n" +
                   $"PbeAlgo: {PbeAlgorithm},\n" +
                   $"PbeDigest: {PbeDigest},\n" +
                   $"CipherAlgo: {CipherAlgorithm},\n" +
                   $"KeySize: {KeySize},\n" +
                   $"BlockMode: {BlockMode},\n" +
                   $"Padding: {Padding},\n" +
                   $"IsIntegrityActive: {IsIntegrityActive},\n" +
                   $"Integrity: {Integrity},\n" +
                   $"IntegrityOptions: {IntegrityOptions},\n";
        }
    }
}