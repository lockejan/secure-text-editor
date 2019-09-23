using System;

namespace BcFactory
{
    public static class KeySize
    {
        public static readonly int[] AES = {128, 192, 256};
        public static readonly int[] RC4 = {40};
//        public static readonly int[] RC4 = {40, 128, 256, 512, 1024, 2048};
    }
    
    public enum PbeAlgorithm{PBKDF2, SCRYPT}
    
    public enum PbeDigest{GCM, SHA1, SHA256}
    
    public enum CipherAlgorithm{AES,RC4}
    public enum BlockMode{None,ECB,CBC,GCM,OFB,CTS}
    
    public enum Padding
    {
        None = 0,
        Pkcs7 = 1,
        ZeroByte = 2
    }
    
    public enum Integrity{Digest,Dsa};
    
    public enum IntegrityOptions
    {
        Sha256 = 0,
        AesCmac = 1,
        HmacSha256 = 2,
    };

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