using System;
using System.Linq;
using System.Text;
using BcFactory.Resources;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    /// <inheritdoc />
    public class CipherBuilder : ICipher
    {
        private readonly CryptoConfig _config;

        private byte[] _inputBytes;
        private byte[] _encryptedBytes;
        private string _plainText;

        private AesEngine _myAes;
        private RC4Engine _myRc4;

        /// <inheritdoc />
        public CipherBuilder(CryptoConfig config)
        {
            _config = config;

            var keySizeString = GetKeySizeString();

            if (_config.Key == null)
                GenerateKey(_config.CipherAlgorithm + keySizeString);

            InitEngine();
        }

        private string GetKeySizeString()
        {
            return _config.CipherAlgorithm == CipherAlgorithm.AES
            ? _config.KeySize.ToString()
            : "";
        }

        private void GenerateKey(string cipher)
        {
            var gen = GeneratorUtilities.GetKeyGenerator(cipher);
            _config.Key = gen.GenerateKey();
        }

        private void InitEngine()
        {
            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
                _myRc4 = new RC4Engine();
            else
            {
                _myAes = new AesEngine();
                if (_config.IvOrSalt == null && _config.BlockMode != BlockMode.ECB)
                    _config.IvOrSalt = GenerateIv();
            }
        }

        private byte[] GenerateIv()
        {
            var random = new SecureRandom();
            var iv = new byte[_config.Key.Length];
            random.NextBytes(iv);
            return iv;
        }

        private ParametersWithIV GetKeyParamWithIv(KeyParameter keyParam)
        {
            return new ParametersWithIV(keyParam, _config.IvOrSalt, 0, 16);
        }

        /// <inheritdoc />
        public CryptoConfig EncryptTextToBytes(string input)
        {
            _inputBytes = Encoding.UTF8.GetBytes(input);
            var keyParam = new KeyParameter(_config.Key);

            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
            {
                var outBuffer = new byte[_config.KeySize];
                _myRc4.Init(true, keyParam);
                _myRc4.ProcessBytes(_inputBytes, 0, _inputBytes.Length, outBuffer, 0);
                
                _config.Cipher = Convert.ToBase64String(outBuffer.Where(x => x != 0).ToArray());
                return _config;
            }
            
            IBufferedCipher cipher = null;
            IBlockCipherPadding padding;
            
            switch (_config.BlockMode)
            {
                case BlockMode.ECB:
                    padding = GetBlockCipherPadding();
                    var ecb = new PaddedBufferedBlockCipher(_myAes, padding);
                    ecb.Init(true, keyParam);
                    cipher = ecb;
                    break;

                case BlockMode.CBC:
                    padding = GetBlockCipherPadding();
                    var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes), padding);
                    cbc.Init(true, GetKeyParamWithIv(keyParam));
                    cipher = cbc;
                    break;

                case BlockMode.CTS:
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, GetKeyParamWithIv(keyParam));
                    cipher = cts;
                    break;

                case BlockMode.OFB:
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes, 8));
                    ofb.Init(true, GetKeyParamWithIv(keyParam));
                    cipher = ofb;
                    break;

                case BlockMode.GCM:
                    var gcm = new GcmBlockCipher(_myAes);
                    var parameters = new AeadParameters(new KeyParameter(_config.Key), 128, _config.IvOrSalt, null);
                    gcm.Init(true, parameters);

                    _encryptedBytes = new byte[gcm.GetOutputSize(_inputBytes.Length)];
                    Int32 returnedLength = gcm.ProcessBytes(_inputBytes, 0, _inputBytes.Length, _encryptedBytes, 0);
                    gcm.DoFinal(_encryptedBytes, returnedLength);
                    break;
                
                case BlockMode.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_config.BlockMode));
            }
            
            if (cipher != null)
                UpdateEncryptedBytes(cipher);
            
            _config.Cipher = Convert.ToBase64String(_encryptedBytes);
            return _config;
        }

        public string DecryptBytesToText(byte[] cipherBytes)
        {
            var keyParam = new KeyParameter(_config.Key);

            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
            {
                var outBuffer = new byte[_config.KeySize];
                _myRc4.Init(false, keyParam);
                _myRc4.ProcessBytes(cipherBytes, 0, cipherBytes.Length, outBuffer, 0);
                UpdatePlainText(outBuffer);
            }
            else
            {
                IBufferedCipher cipher = null;
                IBlockCipherPadding padding;

                switch (_config.BlockMode)
                {
                    case BlockMode.ECB:
                        padding = GetBlockCipherPadding();
                        var ecb = new PaddedBufferedBlockCipher(_myAes, padding);
                        ecb.Init(false, keyParam);
                        cipher = ecb;
                        break;

                    case BlockMode.CBC:
                        padding = GetBlockCipherPadding();
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes), padding);
                        cbc.Init(false, GetKeyParamWithIv(keyParam));
                        cipher = cbc;
                        break;

                    case BlockMode.CTS:
                        var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                        cts.Init(false, GetKeyParamWithIv(keyParam));
                        cipher = cts;
                        break;

                    case BlockMode.OFB:
                        var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes, 8));
                        ofb.Init(false, GetKeyParamWithIv(keyParam));
                        cipher = ofb;
                        break;

                    case BlockMode.GCM:
                        var gcm = new GcmBlockCipher(_myAes);
                        var parameters = new AeadParameters(new KeyParameter(_config.Key), 128, _config.IvOrSalt, null);
                        gcm.Init(false, parameters);

                        byte[] decryptedBytes = new byte[gcm.GetOutputSize(cipherBytes.Length)];
                        int returnedLength = gcm.ProcessBytes(cipherBytes, 0, cipherBytes.Length, decryptedBytes, 0);

                        var len = gcm.DoFinal(decryptedBytes, 0);
                        // 3 param = len or byteCount?
                        _plainText = Encoding.UTF8.GetString(decryptedBytes, 0, len);
                        break;
                    
                    case BlockMode.None:
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_config.BlockMode));
                }

                if (cipher != null)
                    UpdatePlainText(cipher, cipherBytes);
            }

            return _plainText;
        }

        private void UpdatePlainText(IBufferedCipher cipher, byte[] cipherBytes)
        {
            byte[] decryptedBytes = cipher.DoFinal(cipherBytes);
            UpdatePlainText(decryptedBytes);
        }

        private void UpdatePlainText(byte[] decryptedBytes)
        {
            _plainText = Encoding.UTF8.GetString(decryptedBytes);
        } 
        
        private void UpdateEncryptedBytes(IBufferedCipher cipher)
        {
            _encryptedBytes = cipher.DoFinal(_inputBytes);
        }

        private IBlockCipherPadding GetBlockCipherPadding()
        {
            return Padding.ZeroByte == _config.Padding
                ? new ZeroBytePadding()
                : (IBlockCipherPadding)new Pkcs7Padding();
        }
    }
}