namespace BcFactory
{
    public class PbeCipherBuilder : ICrypto
    {
        public PbeCipherBuilder(CryptoConfig config)
        {
            
        }
        public byte[] EncryptTextToBytes(string content)
        {
            throw new System.NotImplementedException();
        }

        public string DecryptBytesToText(byte[] cipherBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}