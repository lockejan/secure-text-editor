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
        private string _blockmode;
        private string _padding;
        private byte[] _textBytes;
        private string _plainText;
        private byte[] _encryptedBytes;

        private readonly AesEngine _myAes;
        private byte[] _myKey;
        private byte[] _myIv;

        public BcCipher(Dictionary<string, string> config)
        {
            _algo = config["Algorithm"];
            _keyLength = config["KeySize"];
            _blockmode = config["BlockMode"];
            _padding = config["Padding"];

            _myAes = new AesEngine();
            GenerateKey(_algo+_keyLength);
            _myIv =_blockmode.Equals("ECB") ? null : GenerateIv();
        }

        public BcCipher(string key, string iv, Dictionary<string, string> config)
        {
            _algo = config["Algorithm"];
            _keyLength = config["KeySize"];
            _blockmode = config["BlockMode"];
            _padding = config["Padding"];
            
            _myAes = new AesEngine();
            _myKey = Encoding.UTF8.GetBytes(key);
            _myIv =  _blockmode.Equals("ECB") ? null : Encoding.UTF8.GetBytes(iv);
        }
        
        public override Dictionary<string,string> Result()
        {
            Dictionary<string, string> result = new Dictionary<string, string>
            {
                {"Algorithm", _algo},
                {"KeySize", _keyLength},
                {"BlockMode", _blockmode},
                {"Padding", _padding},
                {"Iv", Convert.ToBase64String(_myIv)},
                {"Cipher", Convert.ToBase64String(_encryptedBytes)}
            };
            
            return result;
        }
        
        private void GenerateKey(string cipher)
        {
            CipherKeyGenerator gen = new CipherKeyGenerator();
            gen = GeneratorUtilities.GetKeyGenerator(cipher);
            _myKey = gen.GenerateKey(); 
        }

        private byte[] GenerateIv()
        {
            SecureRandom random = new SecureRandom();
            var iv = new byte[_myKey.Length];
            random.NextBytes(iv);
            return iv;
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
                        _encryptedBytes = ecb.DoFinal(_textBytes);
                    }
                    else
                    {
                        PaddedBufferedBlockCipher ecb = new PaddedBufferedBlockCipher(_myAes, new Pkcs7Padding());
                        ecb.Init(true, keyParam);
                        _encryptedBytes = ecb.DoFinal(_textBytes);
                    }
                    break;
                case "CBC":
                    if (_padding == "ZeroBytePadding")
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(true, keyParamWithIv);
                        _encryptedBytes =  cbc.DoFinal(_textBytes);                            
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(true, keyParamWithIv);
                        _encryptedBytes =  cbc.DoFinal(_textBytes);                            
                    }

                    break;
                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, keyParamWithIv);
                    _encryptedBytes =  cts.DoFinal(_textBytes);
                    break;
                
                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(true, keyParamWithIv);
                    _encryptedBytes =  ofb.DoFinal(_textBytes);
                    break;
                
                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(true, parameters);

                    _encryptedBytes = new byte[gcm.GetOutputSize(_textBytes.Length)];
                    Int32 returnedLength = gcm.ProcessBytes
                        (_textBytes, 0, _textBytes.Length, _encryptedBytes, 0);
                    gcm.DoFinal(_encryptedBytes, returnedLength);
                    break;
            }
            return _encryptedBytes;
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
                        _plainText = Encoding.UTF8.GetString(ecb.DoFinal(cipherBytes));
                    }
                    else
                    {
                        var ecb = new PaddedBufferedBlockCipher(_myAes, new Pkcs7Padding());
                        ecb.Init(false, keyParam);
                        _plainText = Encoding.UTF8.GetString(ecb.DoFinal(cipherBytes));
                    }
                    break;

                case "CBC":
                    if (_padding == "ZeroBytePadding")
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(false, keyParamWithIv);
                        _plainText = Encoding.UTF8.GetString(cbc.DoFinal(cipherBytes));
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(false, keyParamWithIv);
                        _plainText = Encoding.UTF8.GetString(cbc.DoFinal(cipherBytes));                            
                    }
                    break;

                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(false, keyParamWithIv);
                    _plainText = Encoding.UTF8.GetString(cts.DoFinal(cipherBytes));
                    break;

                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(false, keyParamWithIv);
                    _plainText = Encoding.UTF8.GetString(ofb.DoFinal(cipherBytes));
                    break;
                
                case "GCM":
                    var gcm = new GcmBlockCipher(_myAes);
                    AeadParameters parameters = 
                        new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);

                    gcm.Init(false, parameters);

                    byte[] decryptedBytes = new byte[gcm.GetOutputSize(cipherBytes.Length)];
                    Int32 retLen = gcm.ProcessBytes
                        (cipherBytes, 0, cipherBytes.Length, decryptedBytes, 0);
                    gcm.DoFinal(decryptedBytes, retLen);
                    _plainText = Encoding.UTF8.GetString(decryptedBytes).TrimEnd("\r\n\0".ToCharArray());
                    break;
            }
            return _plainText;
        }
    }

}