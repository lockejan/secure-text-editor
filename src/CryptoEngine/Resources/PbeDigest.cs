namespace CryptoEngine.Resources
{
    /// <summary>
    /// All PBE related forms of hashes or other algorithms which are providing any sort of integrity.
    /// These are currently only used to be displayed in the frontend and not for processing. 
    /// </summary>
    public enum PbeDigest { GCM, SHA1, SHA256 }
}