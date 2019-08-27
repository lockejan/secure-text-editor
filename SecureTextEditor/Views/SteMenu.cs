using System;
using System.Collections.Generic;

namespace SecureTextEditor.Views
{

    /// <summary>
    /// SteMenu properties for SaveDialog.
    /// These dictionaries are representing all possible configurations.
    /// They are used to fill the CLI or GUI menus or combos with entries.
    /// If additional options are being implemented these dictionaries
    /// have to be updated to make the menu recognize the internal updates.
    /// </summary>
    public class SteMenu
    {
        int[] KeySize = {128, 192, 256, 40};
        enum Cipher {AES, RC4};
        enum BlockMode {ECB, CBC, GCM, OFB, CTS};
        enum PBE {Yes, No};
        enum PBECipher {PBKDF2, SCRYPT};
        enum PBEDigest {SHA1,SHA256};

        [Flags]
        enum Padding{
            NoPadding = 1,
            ZeroBytePadding = 1 << 1,
            PKCS7 = 1 << 2
        }
        
        public static void enumTester()
        {
            var BlockModeDict = new Dictionary<BlockMode, Padding>()
            {
                {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
                {BlockMode.CBC, Padding.ZeroBytePadding | Padding.PKCS7},
                {BlockMode.GCM, Padding.NoPadding},
                {BlockMode.CTS, Padding.NoPadding},
                {BlockMode.OFB, Padding.NoPadding}
            };

            /*
            var BlockCipherDict = new Dictionary<CipherType, Dictionary<Cipher,Dictionary<BlockMode, Padding>>>()
            {
                {
                    CipherType.CIPHER,
                    new Dictionary<BlockMode, Padding>()
                    {
                        {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
                        {BlockMode.CBC, Padding.ZeroBytePadding | Padding.PKCS7},
                        {BlockMode.GCM, Padding.NoPadding},
                        {BlockMode.CTS, Padding.NoPadding},
                        {BlockMode.OFB, Padding.NoPadding}}
                },
                {
                    PBE.SCRYPT,
                    new Dictionary<BlockMode, Padding>()
                    {
                        {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
                        {BlockMode.CBC, Padding.ZeroBytePadding | Padding.PKCS7},
                        {BlockMode.GCM, Padding.NoPadding},
                        {BlockMode.CTS, Padding.NoPadding},
                        {BlockMode.OFB, Padding.NoPadding}}
                },
                
            };
            var StreamCipherDict = new Dictionary<Cipher, Padding>()
            {
                {Cipher.RC4, Padding.NoPadding}
            };
*/

            //var value = BlockCipherDict[Cipher.AES];
            
            //if (value.HasFlag(Padding.NoPadding))
            {
                Console.WriteLine("Treffer A");    
            }
            
        }
        
        /// <summary>
        /// SteMenu entries for supported ciphers and their key lengths.
        /// </summary>
        public static readonly IDictionary<string, string[]> CipherMenuTree = new Dictionary<string, string[]>()
        {
            {"AES", new[] {"128", "192","256"}}
        };
        
        /// <summary>
        /// SteMenu entries for supported block modes and padding options.
        /// Holds only valid combinations to prevent the user from bad decisions. 
        /// </summary>
        public static readonly IDictionary<string, string[]> CipherOptionsMenuTree = new Dictionary<string, string[]>()
        {
            {"ECB", new[] {"ZeroBytePadding", "PKCS7"}},
            {"CBC", new[] {"ZeroBytePadding", "PKCS7"}},
            {"GCM", new[] {"NoPadding"}},
            {"OFB", new[] {"NoPadding"}},
            {"CTS", new[] {"NoPadding"}}
        };
        
        
        /// <summary>
        /// Holds menu part for password based encryption options.
        /// Furthermore the dependent config parts for further processing are also defined here.
        /// </summary>
        public static readonly IDictionary<string, string[]> PBEMenuTree = new Dictionary<string, string[]>()
        {
            {"WithAES256-GCM-SCRYPT", new[] {"GCM","NoPadding"}},
            {"WithSHA256And128Bit-AES-CBC-BC", new[] {"CBC", "PKCS7"}},
            {"WithSHAAnd40BitRC4", new[] {"-","NoPadding"}}
        };

        /// <summary>
        /// Holds menu part for integrity related options of the text editor.
        /// For instance all digest modes and digital signature possibilities.
        /// </summary>
        public static readonly IDictionary<string, string[]> IntegrityMenuTree = new Dictionary<string, string[]>()
        {
            {"Digest", new[] {"SHA-256","AESCMAC","HMAC-SHA256"}},
            {"Digital Signature", new[] {"SHA256withDSA"}},
        };
        
//        public static readonly IDictionary<string, Dictionary<string, string[]>> EncryptMenuTree = new Dictionary<string, Dictionary<string, string[]>>()
//        {
//            {
//                "AES",
//                new Dictionary<string, string[]>()
//                {
//                    {"ECB", new[] {"ZeroBytePadding", "PKCS7"}},
//                    {"CBC", new[] {"ZeroBytePadding", "PKCS7"}},
//                    {"GCM", new[] {"NoPadding"}},
//                    {"OFB", new[] {"NoPadding"}},
//                    {"CTS", new[] {"NoPadding"}}
//                }
//            },
//            {
//                "PBE",
//                new Dictionary<string, string[]>()
//                {
//                    {"WithAES256-GCM-SCRYPT", new[] {"NoPadding"}},
//                    {"WithSHA256And128Bit-AES-CBC-BC", new[] {"PKCS7"}},
//                    {"WithSHAAnd40BitRC4", new[] {"NoPadding"}}
//                }
//            }
//        };
    }
}


//{
//"AES",
//new Dictionary<string, Dictionary<string> string[]>()
//{
//    {"Which key length of AES you wanna use?",new[] {"128","192","256"}},
//    {"Which Blockmode should be used?", new[]{"ECB","CBC","GCM","OFB","CTS"}},
//    {"Which Padding should be used?", new[]{"ECB","CBC","GCM","OFB","CTS"}}
//    {"ECB", new[] {"ZeroBytePadding", "PKCS7"}},
//    {"CBC", new[] {"ZeroBytePadding", "PKCS7"}},
//    {"GCM", new[] {"NoPadding"}},
//    {"OFB", new[] {"NoPadding"}},
//    {"CTS", new[] {"ZeroBytePadding", "PKCS7", "NoPadding"}}
//}
//},