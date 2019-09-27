namespace BcFactory
{
    /// <summary>
    /// All available Digests and Algorithms which are available to be used with DSA or just as Digest.
    /// Not all combinations are possible. The related business logic is implemented somewhere else. 
    /// </summary>
    public enum IntegrityOptions
    {
        Sha256 = 0,
        AesCmac = 1,
        HmacSha256 = 2,
    };
}