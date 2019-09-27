using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BcFactory
{
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

        //[JSONIgnore]
        public byte[] PbeKey { get; set; }

        public bool IsIntegrityActive { get; set; }
        public Integrity Integrity { get; set; }
        public IntegrityOptions IntegrityOptions { get; set; }

        /// <summary>
        /// Custom ToString to display state of CryptoConfig.
        /// </summary>
        /// <returns>String representation of current CryptoConfig.</returns>
        public override string ToString()
        {
            return $"IsEncryptActive: {IsEncryptActive},\n" +
                   $"IsPbeActive: {IsPbeActive},\n" +
                   $"PbePassword: {PbePassword},\n" +
                   $"PbeAlgorithm: {PbeAlgorithm},\n" +
                   $"PbeDigest: {PbeDigest},\n" +
                   $"CipherAlgorithm: {CipherAlgorithm},\n" +
                   $"KeySize: {KeySize},\n" +
                   $"BlockMode: {BlockMode},\n" +
                   $"Padding: {Padding},\n" +
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
        /// Determines valid Paddings for given Blockmodes.
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
        /// Determines valid IntegrityOptions for given Integrity selection.
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
        /// Determines valid IntegrityLabels for given PBE-Parameters.
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
        /// Determines valid Keysizes based on given Cipher- and/or PBE-Algorithms.
        /// </summary>
        /// <returns>Enum holding valid IntegrityOptions</returns>
        public int[] GetKeySizes()
        {
            if (CipherAlgorithm == CipherAlgorithm.RC4)
                return BcFactory.KeySize.RC4;

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
            return BcFactory.KeySize.AES;
        }
    }
}