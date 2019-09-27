using System.Collections.Generic;
using BcFactory.Resources;
using Newtonsoft.Json;

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

        [JsonProperty(Required = Required.Always)]
        public string Encoding { get; }

        [JsonProperty(Required = Required.Default)]
        public string IvOrSalt { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string SignaturePublicKey { get; set; }
        
        [JsonIgnoreAttribute]
        public string SignaturePrivateKey { get; set; }

        [JsonProperty(Required = Required.Default)]
        private string Signature { get; set; }

        /// <summary>
        /// Generated cipher of cipher engine.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Cipher { get; set; }
        
        /// <summary>
        /// Referring to whether or not encryption is activated.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool IsEncryptActive { get; set; }
        
        /// <summary>
        /// Representing the used cipher algorithm.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public CipherAlgorithm CipherAlgorithm { get; set; }
        
        /// <summary>
        /// Representing the key size used by related cipher algorithm.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int KeySize { get; set; }
        
        /// <summary>
        /// Representing the block mode used by the cipher engine.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public BlockMode BlockMode { get; set; }
        
        /// <summary>
        /// Representing the padding used by the cipher engine.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public Padding Padding { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public bool IsPbeActive { get; set; }
        
        [JsonProperty(Required = Required.Default)]
        public PbeAlgorithm PbeAlgorithm { get; set; }
        
        [JsonProperty(Required = Required.Default)]
        public PbeDigest PbeDigest { get; set; }
        
        [JsonIgnoreAttribute]
        public char[] PbePassword { get; set; }

        [JsonIgnoreAttribute]
        public byte[] Key { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public bool IsIntegrityActive { get; set; }
        
        [JsonProperty(Required = Required.Default)]
        public Integrity Integrity { get; set; }
        
        [JsonProperty(Required = Required.Default)]
        public IntegrityOptions IntegrityOptions { get; set; }

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
                   $"SignaturePrivateKey: {SignaturePrivateKey},\n" +
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
                   $"PbePassword: {PbePassword},\n" +
                   $"Key: {Key},\n" +
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
        public IEnumerable<BlockMode> GetValidBlockModes()
        {
            // https://alexatnet.com/cs8-switch-statement/
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
                        foreach (var value in EnumExtensions.ValuesExcept(BlockMode.None))
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

            if (IsPbeActive)
            {
                if (PbeAlgorithm == PbeAlgorithm.SCRYPT)
                    return new[] { 256 };

                if (PbeAlgorithm == PbeAlgorithm.PBKDF2)
                {
                    if (CipherAlgorithm == CipherAlgorithm.AES)
                        return new[] { 128 };

                    if (CipherAlgorithm == CipherAlgorithm.RC4)
                        return new[] { 40 };
                }
            }
            return Resources.KeySize.AES;
        }
    }
}