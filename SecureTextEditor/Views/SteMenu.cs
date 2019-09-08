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
        public static int[] KeySize = {128, 192, 256, 40};

        public enum CipherAlgorithm
        {
            AES,
            RC4
        };

        public enum BlockMode
        {
            ECB,
            CBC,
            GCM,
            OFB,
            CTS
        };

        public enum PBE
        {
            Yes,
            No
        };

        public enum PBECipher
        {
            PBKDF2,
            SCRYPT
        };

        public enum PBEDigest
        {
            SHA1,
            SHA256
        };

        public enum Integrity
        {
            Digest,
            DSA
        };

        [Flags]
        public enum DigestOptions
        {
            SHA256 = 1,
            AESCMAC = 1 << 1,
            HMACSHA256 = 1 << 2,
//            SHA256withDSA = 1 << 3
        };

        [Flags]
        public enum Padding
        {
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
            var BlockCipherDict = new Dictionary<CipherType, Dictionary<CipherAlgorithm,Dictionary<BlockMode, Padding>>>()
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
            var StreamCipherDict = new Dictionary<CipherAlgorithm, Padding>()
            {
                {CipherAlgorithm.RC4, Padding.NoPadding}
            };
*/

            //var value = BlockCipherDict[CipherAlgorithm.AES];

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
            {"AES", new[] {"128", "192", "256"}}
        };
    }
}