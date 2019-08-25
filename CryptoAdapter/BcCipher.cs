using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CryptoAdapter
{

    public class BcCipher : CustomCipherFactory
    {
        private string _algo;
        private string _keyLength;
        private byte[] _textBytes;
        private string _blockmode;
        private string _padding;
        
        private byte[] _myKey;
        private byte[] _myIv;
        private AesEngine _myAes;

        public BcCipher(Dictionary<string, string> config)
        {
            _myAes = new AesEngine();
            _myKey = null;
            _myIv = null;
            
//            var configs = config.Split('/');
            Console.WriteLine("Encryption startet");
            foreach (var entries in config)
            {
                Console.WriteLine($"- {entries}");
            }
//            , string algo, string plainText, string blockmode, string padding
//            var test = "Cipher/AES/192/CBC/PKCS7";
            _algo = config["Algorithm"];
            _keyLength = config["KeySize"];
            _blockmode = config["BlockMode"];
            _padding = config["Padding"];
//            _textBytes = Encoding.UTF8.GetBytes(plainText);

            GenerateKey(_algo+_keyLength);
            GenerateIv();
            

        }
        
        private void GenerateKey(string cipher)
        {
            CipherKeyGenerator gen = new CipherKeyGenerator();
            gen = GeneratorUtilities.GetKeyGenerator(cipher);
            _myKey = gen.GenerateKey(); 
        }

        private void GenerateIv()
        {
            SecureRandom random = new SecureRandom();
            _myIv = new byte[_myKey.Length];
            random.NextBytes(_myIv);
        }
        
        public override byte[] EncryptTextToBytes(string input)
        {
            _textBytes = Encoding.UTF8.GetBytes(input);
            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);

            switch (_blockmode)
            {
                case "ECB":
                    if (_padding == "ZeroBytePadding")
                    {
                        PaddedBufferedBlockCipher ecb = new PaddedBufferedBlockCipher(_myAes, new ZeroBytePadding());
                        ecb.Init(true, keyParam);
                        return ecb.DoFinal(_textBytes);
                    }
                    else
                    {
                        PaddedBufferedBlockCipher ecb = new PaddedBufferedBlockCipher(_myAes, new Pkcs7Padding());
                        ecb.Init(true, keyParam);
                        return ecb.DoFinal(_textBytes);
                    }

                case "CBC":
                    if (_padding == "ZeroBytePadding")
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(true, keyParamWithIv);
                        return cbc.DoFinal(_textBytes);                            
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(true, keyParamWithIv);
                        return cbc.DoFinal(_textBytes);                            
                    }

                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, keyParamWithIv);
                    return cts.DoFinal(_textBytes);                        

                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(true, keyParamWithIv);
                    return ofb.DoFinal(_textBytes);                        

                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(true, parameters);

                    byte[] encryptedBytes = new byte[gcm.GetOutputSize(_textBytes.Length)];
                    Int32 retLen = gcm.ProcessBytes
                        (_textBytes, 0, _textBytes.Length, encryptedBytes, 0);
                    gcm.DoFinal(encryptedBytes, retLen);
                    return encryptedBytes;
            }
            
            return new byte[16];
        }

        public override string DecryptBytesToText(byte[] cipherBytes)
        {
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);
            
            switch (_blockmode)
            {
                case "ECB":
                    if (_padding == "ZeroBytePadding")
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

                case "CBC":
                    if (_padding == "ZeroBytePadding")
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

                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(false, keyParamWithIv);
                    return Encoding.UTF8.GetString(cts.DoFinal(cipherBytes));                        

                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(false, keyParamWithIv);
                    return Encoding.UTF8.GetString(ofb.DoFinal(cipherBytes));
                
                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(false, parameters);

                    byte[] decryptedBytes = new byte[gcm.GetOutputSize(cipherBytes.Length)];
                    Int32 retLen = gcm.ProcessBytes
                        (cipherBytes, 0, cipherBytes.Length, decryptedBytes, 0);
                    gcm.DoFinal(decryptedBytes, retLen);
                    return Encoding.UTF8.GetString(decryptedBytes).TrimEnd("\r\n\0".ToCharArray());
            }

            return "Dummy";
        }
    }




}