using System;
using System.Collections.Generic;

namespace CryptoAdapter
{
    public enum CipherAlgorithm
    {
        AES,
        RC4
    }
    
    
    public enum BlockMode
    {
        ECB,
        CBC,
        GCM,
        OFB,
        CTS
    };
    
//    [Flags]
//    public enum Padding
//    {
//        NoPadding = 1,
//        ZeroBytePadding = 1 << 1,
//        Pkcs7 = 1 << 2
//    }
    
    public enum Padding
    {
        NoPadding,
        ZeroBytePadding,
        Pkcs7
    }

    public class DecryptionKeyInfos
    {

    }
    

    public class CryptoConfig
    {
//        public static int[] KeySize = {128, 192, 256, 40};
        public CipherAlgorithm Algorithm { get; set; }
//        public int[] KeySize { get; set; }
        public BlockMode BlockMode { get; set; }
        public Padding Padding { get; set; }
        
        public Dictionary<string, string> OldConfig;

        
        public override string ToString()
        {
            return $"CipherAlgo: {Algorithm},\nBlockMode: {BlockMode},\nPadding: {Padding},\nOldConfig: {OldConfig}";
        }
    }
}