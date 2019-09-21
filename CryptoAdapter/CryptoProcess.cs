using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CryptoAdapter
{
    public class CryptoProcess
    {
        private CryptoConfig _config;

        private byte[] _textBytes;
        private string _cipherText;
        private byte[] _encryptedBytes;

        private readonly AesEngine _myAes;
        private byte[] _myIv;
        private byte[] _myKey;

        public CryptoProcess(CryptoConfig config)
        {
            _config = config;
            
            _myAes = new AesEngine();
            GenerateKey(_config.Algorithm.ToString()+_config.KeySize);
            _myIv = _config.BlockMode == BlockMode.ECB ? null : GenerateIv();
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

        private ParametersWithIV GetKeyParamWithIv(KeyParameter keyParam)
        {
            return new ParametersWithIV(keyParam, _myIv, 0, 16);
        }

        public byte[] EncryptTextToBytes(string input)
        {
            _textBytes = Encoding.UTF8.GetBytes(input);
            KeyParameter keyParam = new KeyParameter(_myKey);

            switch (_config.BlockMode)
            {
                case BlockMode.ECB:
                    if (Padding.ZeroByte == _config.Padding)
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
                case BlockMode.CBC:
                    if (Padding.ZeroByte == _config.Padding)
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new ZeroBytePadding());
                        cbc.Init(true, GetKeyParamWithIv(keyParam));
                        _encryptedBytes =  cbc.DoFinal(_textBytes);                            
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes),new Pkcs7Padding());
                        cbc.Init(true, GetKeyParamWithIv(keyParam));
                        _encryptedBytes =  cbc.DoFinal(_textBytes);                            
                    }

                    break;
                case BlockMode.CTS:
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, GetKeyParamWithIv(keyParam));
                    _encryptedBytes =  cts.DoFinal(_textBytes);
                    break;
                
                case BlockMode.OFB:
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(true, GetKeyParamWithIv(keyParam));
                    _encryptedBytes =  ofb.DoFinal(_textBytes);
                    break;
                
                case BlockMode.GCM:
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
        
    }
}