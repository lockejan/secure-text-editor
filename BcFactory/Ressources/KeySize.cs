namespace BcFactory
{
    /// <summary>
    /// All available Keysizes for given block and stream ciphers.
    /// </summary>
    public static class KeySize
    {
        /// <summary>
        /// Supported key length for AES.
        /// </summary>
        public static readonly int[] AES = { 128, 192, 256 };
        /// <summary>
        /// Supported key length for RC4.
        /// </summary>
        public static readonly int[] RC4 = { 40, 128, 256, 512, 1024, 2048 };
    }
}