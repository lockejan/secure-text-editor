using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SecureTextEditor
{
    public class SecureTextEditorModel
    {
        private char[] _text;
        public char[] Text
        {
            get { return _text; }
            set { _text = value; }
        }

        
        private AesEngine _myAes; 
        public AesEngine AES
        {
            get { return _myAes; }
            set { _myAes = value; }
        }

        private byte[] _myKey; 
        public byte[] KEY
        {
            get { return _myKey; }
            set { _myKey = value; }
        }

        
        private byte[] _myIv; 
        public byte[] IV
        {
            get { return _myIv; }
            set { _myIv = value; }
        }
        
        public SecureTextEditorModel()
        {
            _myAes = new AesEngine();
            _myKey = null;
            _myIv = null;
        }

        private byte[] GenerateKey(String cipher)
        {
            CipherKeyGenerator gen = new CipherKeyGenerator();
            gen = GeneratorUtilities.GetKeyGenerator(cipher);
            return gen.GenerateKey(); 
        }

        private void GenerateIv()
        {
            SecureRandom random = new SecureRandom();
            _myIv = new byte[_myKey.Length];
            
            random.NextBytes(_myIv);
        }

        public byte[] EncryptTextToBytes(string plainText, string algo, string blockmode, string padding)
        {
            _myKey = GenerateKey(algo);
            GenerateIv();

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);

            switch (blockmode)
            {
                case "ECB":
                    if (padding == "NoPadding")
                    {
                        BufferedBlockCipher ecb = new BufferedBlockCipher(_myAes);
                        ecb.Init(true, keyParam);
                        return ecb.DoFinal(inputBytes);
                    }
                    else if (padding == "ZeroBytePadding")
                    {
                        PaddedBufferedBlockCipher ecb = new PaddedBufferedBlockCipher(_myAes, new ZeroBytePadding());
                        ecb.Init(true, keyParam);
                        return ecb.DoFinal(inputBytes);
                    }
                    else
                    {
                        PaddedBufferedBlockCipher ecb = new PaddedBufferedBlockCipher(_myAes, new Pkcs7Padding());
                        ecb.Init(true, keyParam);
                        return ecb.DoFinal(inputBytes);
                    }
//                    break;
                case "CBC":
                    if (padding == "NoPadding")
                    {
                        var cbc = new BufferedBlockCipher(new CbcBlockCipher(_myAes));
                        cbc.Init(true, keyParamWithIv);
                        return cbc.DoFinal(inputBytes);        
                    }
                    else if (padding == "ZeroBytePadding")
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(true, keyParamWithIv);
                        return cbc.DoFinal(inputBytes);                            
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(true, keyParamWithIv);
                        return cbc.DoFinal(inputBytes);                            
                    }
//                    break;
                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, keyParamWithIv);
                    return cts.DoFinal(inputBytes);                        
//                    break;
                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(true, keyParamWithIv);
                    return ofb.DoFinal(inputBytes);                        
//                    break;
                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(true, parameters);

                    byte[] encryptedBytes = new byte[gcm.GetOutputSize(inputBytes.Length)];
                    Int32 retLen = gcm.ProcessBytes
                        (inputBytes, 0, inputBytes.Length, encryptedBytes, 0);
                    gcm.DoFinal(encryptedBytes, retLen);
                    return encryptedBytes;
//                    break;
            }
            
            return new byte[16];
        }

        public string DecryptText(byte[] cipherBytes, string blockmode, string padding)
        {
//            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(_myAes);

            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);
            
            switch (blockmode)
            {
                case "ECB":
                    if (padding == "NoPadding")
                    {
                        var ecb = new BufferedBlockCipher(_myAes);
                        ecb.Init(false, keyParam);
                        return Encoding.UTF8.GetString(ecb.DoFinal(cipherBytes));
                    }
                    else if (padding == "ZeroBytePadding")
                    {
                        var ecb = new PaddedBufferedBlockCipher(_myAes, new ZeroBytePadding());
                        ecb.Init(false, keyParam);
                        return Encoding.UTF8.GetString(ecb.DoFinal(cipherBytes));
                    }
                    else
                    {
                        var ecb = new PaddedBufferedBlockCipher(_myAes, new Pkcs7Padding());
                        ecb.Init(false, keyParam);
                        return Encoding.UTF8.GetString(ecb.DoFinal(cipherBytes));
                    }
//                    break;
                case "CBC":
                    if (padding == "NoPadding")
                    {
                        var cbc = new BufferedBlockCipher(new CbcBlockCipher(_myAes));
                        cbc.Init(false, keyParamWithIv);
                        return Encoding.UTF8.GetString(cbc.DoFinal(cipherBytes)); 
                    }
                    else if (padding == "ZeroBytePadding")
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(false, keyParamWithIv);
                        return Encoding.UTF8.GetString(cbc.DoFinal(cipherBytes));
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(false, keyParamWithIv);
                        return Encoding.UTF8.GetString(cbc.DoFinal(cipherBytes));                            
                    }
//                    break;
                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(false, keyParamWithIv);
                    return Encoding.UTF8.GetString(cts.DoFinal(cipherBytes));                        
//                    break;
                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(false, keyParamWithIv);
                    return Encoding.UTF8.GetString(ofb.DoFinal(cipherBytes));                        
//                    break;
                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(false, parameters);

                    byte[] decryptedBytes = new byte[gcm.GetOutputSize(cipherBytes.Length)];
                    Int32 retLen = gcm.ProcessBytes
                        (cipherBytes, 0, cipherBytes.Length, decryptedBytes, 0);
                    gcm.DoFinal(decryptedBytes, retLen);
                    return Encoding.UTF8.GetString(decryptedBytes); //.TrimEnd("\r\n\0".ToCharArray())
//                    break;
            }

            return "Dummy";
        }
    }
    
}