using System;
using System.Collections.Generic;
using BcFactory.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BcFactory
{
    /// <summary>
    /// Configuration class which holds all relevant attributes for further processing.
    /// Properties are just having basic getter and setter.
    /// ToString-method is customized to display state of attributes for debugging.
    /// </summary>
    public class CryptoConfig
    {
        [JsonProperty(Required = Required.Always)]
        private string FormatVersion { get; } = "0.1";

        /// <summary>
        /// System encoding to help in case of encoding errors on different environment.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Encoding { get; } = System.Text.Encoding.Default.ToString();

        /// <summary>
        /// Iv for passwordless created ciphers engine or Salt if the key was derived using pbe.
        /// </summary>
        [JsonProperty]
        public byte[] IvOrSalt { get; set; }

        /// <summary>
        /// String representation of publicKey certificate to verify signature on load.
        /// </summary>
        [JsonIgnoreAttribute]
        public string SignaturePublicKey { get; set; }
        
        /// <summary>
        /// String representation of privateKey certificate which has been used to sign message. 
        /// </summary>
        [JsonIgnoreAttribute]
        public string SignaturePrivateKey { get; set; }

        /// <summary>
        /// String representation of Signature of cipher respective message.
        /// </summary>
        [JsonProperty]
        public string Signature { get; set; }

        /// <summary>
        /// Generated cipher text of cipher engine.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Cipher { get; set; }
        
        /// <summary>
        /// Referring to whether or not encryption is activated.
        /// </summary>
        [JsonProperty]
        public bool IsEncryptActive { get; set; }
        
        /// <summary>
        /// String representation of used cipher algorithm.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public CipherAlgorithm CipherAlgorithm { get; set; }
        
        /// <summary>
        /// Representing the key size used by cipher algorithm.
        /// </summary>
        [JsonProperty]
        public int KeySize { get; set; }
        
        /// <summary>
        /// Representing the block mode used by the cipher engine.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockMode BlockMode { get; set; }
        
        /// <summary>
        /// Representing the padding used by the cipher engine.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Padding Padding { get; set; }
        
        /// <summary>
        /// Referring to whether or not password based encryption has been used.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool IsPbeActive { get; set; }
        
        /// <summary>
        /// used PbeAlgorithm to generate key.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PbeAlgorithm PbeAlgorithm { get; set; }
        
        /// <summary>
        /// Integrity or digest mode which has been used during key derivation.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PbeDigest PbeDigest { get; set; }
        
        /// <summary>
        /// Password which comes from user through load or save dialog.
        /// </summary>
        [JsonIgnoreAttribute]
        public char[] PbePassword { get; set; }

        /// <summary>
        /// secret key which is needed for en- and decryption.
        /// </summary>
        [JsonIgnoreAttribute]
        public byte[] Key { get; set; }
        
        /// <summary>
        /// states if some of the provided integrity engine was used. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool IsIntegrityActive { get; set; }
        
        /// <summary>
        /// refers to the form of integrity - digest or cert(dsa)
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Integrity Integrity { get; set; }
        
        /// <summary>
        /// string representation of used config for digest or certificate(dsa).
        /// currently mac or digest.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IntegrityOptions IntegrityOptions { get; set; }

        /// <summary>
        /// key which will created if AesCmac is used.
        /// </summary>
        [JsonIgnoreAttribute]
        public byte[] DigestKey { get; set; }
        
        /// <summary>
        /// Custom ToString to display state of CryptoConfig.
        /// </summary>
        /// <returns>String representation of current CryptoConfig.</returns>
        public override string ToString()
        {
            return $"FormatVersion: {FormatVersion},\n" +
                   $"Encoding: {Encoding},\n" +
                   $"IvOrSalt: {IvOrSalt},\n" +
                   $"SignaturePublicKey: {SignaturePublicKey},\n" +
                   $"Signature: {Signature},\n" +
                   $"Cipher: {Cipher},\n" +
                   $"IsEncryptActive: {IsEncryptActive},\n" +
                   $"CipherAlgorithm: {CipherAlgorithm},\n" +
                   $"KeySize: {KeySize},\n" +
                   $"BlockMode: {BlockMode},\n" +
                   $"Padding: {Padding},\n" +
                   $"IsPbeActive: {IsPbeActive},\n" +
                   $"PbeAlgorithm: {PbeAlgorithm},\n" +
                   $"PbeDigest: {PbeDigest},\n" +
                   $"IsIntegrityActive: {IsIntegrityActive},\n" +
                   $"Integrity: {Integrity},\n" +
                   $"IntegrityOptions: {IntegrityOptions},\n";
        }

        /// <summary>
        /// Determines which CipherAlgorithms are supported for given PBE-Algorithms.
        /// </summary>
        /// <returns>Enum holding valid CipherAlgorithms</returns>
        public IEnumerable<CipherAlgorithm> GetValidAlgorithms()
        {
            yield return CipherAlgorithm.AES;

            if (PbeAlgorithm == PbeAlgorithm.PBKDF2)
                yield return CipherAlgorithm.RC4;
        }

        /// <summary>
        /// Determines valid Blockmodes for given Cipher- or/and PBE-Algorithms.
        /// </summary>
        /// <returns>Enum holding valid BlockModes</returns>
        public IEnumerable<BlockMode> GetValidBlockModes(BlockMode skip = BlockMode.None)
        {
            switch (CipherAlgorithm)
            {
                case CipherAlgorithm.AES:
                    if (IsPbeActive)
                    {
                        if (PbeAlgorithm == PbeAlgorithm.PBKDF2)
                            yield return BlockMode.CBC;
                        if (PbeAlgorithm == PbeAlgorithm.SCRYPT)
                            yield return BlockMode.GCM;
                    }
                    else
                    {
                        foreach (var value in EnumExtensions.ValuesExcept(BlockMode.None, skip))
                            yield return value;
                    }
                    break;
                case CipherAlgorithm.RC4:
                    yield return BlockMode.None;
                    break;
            }
        }

        /// <summary>
        /// Determines valid Paddings for given Blockmode.
        /// </summary>
        /// <returns>Enum holding valid Paddings</returns>
        public IEnumerable<Padding> GetValidPaddings()
        {
            switch (BlockMode)
            {
                case BlockMode.ECB:
                case BlockMode.CBC:
                    yield return Padding.Pkcs7;

                    if (!IsPbeActive)
                        yield return Padding.ZeroByte;
                    break;
                default:
                    yield return Padding.None;
                    break;
            }
        }

        /// <summary>
        /// Determines valid integrity options for given integrity selection (like certificate or digest).
        /// </summary>
        /// <returns>Enum holding valid IntegrityOptions</returns>
        public IEnumerable<IntegrityOptions> GetIntegrityOptions()
        {
            yield return IntegrityOptions.Sha256;

            if (Integrity != Integrity.Digest)
                yield break;

            foreach (var value in EnumExtensions.ValuesExcept(IntegrityOptions.Sha256))
                yield return value;
        }

        /// <summary>
        /// Determines valid integrity labels for given PBE-Parameters.
        /// </summary>
        /// <returns>Enum entry holding the related Digest</returns>
        public PbeDigest GetDigest()
        {
            if (PbeAlgorithm == PbeAlgorithm.PBKDF2)
                return CipherAlgorithm == CipherAlgorithm.AES
                    ? PbeDigest.SHA256
                    : PbeDigest.SHA1;

            return PbeDigest.GCM;
        }

        /// <summary>
        /// Determines valid key sizes based on given Cipher- and/or PBE-Algorithms.
        /// </summary>
        /// <returns>Enum holding valid IntegrityOptions</returns>
        public int[] GetKeySizes()
        {
            if (CipherAlgorithm == CipherAlgorithm.RC4)
                return Resources.KeySize.RC4;

            if (!IsPbeActive) return Resources.KeySize.AES;

            return PbeAlgorithm switch
            {
                PbeAlgorithm.SCRYPT => new[] {256},
                PbeAlgorithm.PBKDF2 when CipherAlgorithm == CipherAlgorithm.AES => new[] {128},
                PbeAlgorithm.PBKDF2 when CipherAlgorithm == CipherAlgorithm.RC4 => new[] {40},
                _ => Resources.KeySize.AES
            };
        }
        
        /// <summary>
        /// Clear all secrets from current state.
        /// </summary>
        public void ClearSecrets(bool beforeSave)
        {
            if (beforeSave)
            {
                if (Key != null && IsPbeActive)
                    Key = null;

                if (PbePassword != null)
                    PbePassword = null;
            }
            else
            {
                if (DigestKey != null)
                    DigestKey = null;

                if (Key != null)
                    Array.Clear(Key,0, Key.Length);
    
            }
        }
    }
}
