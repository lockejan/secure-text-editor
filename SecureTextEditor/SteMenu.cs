using System.Collections.Generic;

namespace SecureTextEditor
{
    
    /// <summary>
    /// SteMenu properties for SaveDialog
    /// </summary>
    public static class SteMenu
    {
        /// <summary>
        /// SteMenu entries for supported ciphers and their key lengths
        /// </summary>
        public static readonly IDictionary<string, string[]> CipherMenuTree = new Dictionary<string, string[]>()
        {
            {"AES", new[] {"128", "192","256"}}
        };
        
        /// <summary>
        /// SteMenu entries for supported block modes and padding options
        /// </summary>
        public static readonly IDictionary<string, string[]> CipherOptionsMenuTree = new Dictionary<string, string[]>()
        {
            {"ECB", new[] {"ZeroBytePadding", "PKCS7"}},
            {"CBC", new[] {"ZeroBytePadding", "PKCS7"}},
            {"GCM", new[] {"NoPadding"}},
            {"OFB", new[] {"NoPadding"}},
            {"CTS", new[] {"NoPadding"}}
        };
        
        
        public static readonly IDictionary<string, string[]> PBEMenuTree = new Dictionary<string, string[]>()
        {
            {"WithAES256-GCM-SCRYPT", new[] {"GCM","NoPadding"}},
            {"WithSHA256And128Bit-AES-CBC-BC", new[] {"CBC", "PKCS7"}},
            {"WithSHAAnd40BitRC4", new[] {"-","NoPadding"}}
        };

        public static readonly IDictionary<string, string[]> IntegrityMenuTree = new Dictionary<string, string[]>()
        {
            {"Digest", new[] {"SHA-256","AESCMAC","HMAC-SHA256"}},
            {"Digital Signature", new[] {"SHA256withDSA"}},
        };
        
        public static readonly IDictionary<string, Dictionary<string, string[]>> EncryptMenuTree = new Dictionary<string, Dictionary<string, string[]>>()
        {
            {
                "AES",
                new Dictionary<string, string[]>()
                {
                    {"ECB", new[] {"ZeroBytePadding", "PKCS7"}},
                    {"CBC", new[] {"ZeroBytePadding", "PKCS7"}},
                    {"GCM", new[] {"NoPadding"}},
                    {"OFB", new[] {"NoPadding"}},
                    {"CTS", new[] {"NoPadding"}}
                }
            },
            {
                "PBE",
                new Dictionary<string, string[]>()
                {
                    {"WithAES256-GCM-SCRYPT", new[] {"NoPadding"}},
                    {"WithSHA256And128Bit-AES-CBC-BC", new[] {"PKCS7"}},
                    {"WithSHAAnd40BitRC4", new[] {"NoPadding"}}
                }
            }
        };
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