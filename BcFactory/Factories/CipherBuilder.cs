using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BcFactory.Factories
{
    /// <inheritdoc />
    public class CipherBuilder : ICrypto
    {
        private readonly CryptoConfig _config;

        private byte[] _textBytes;
        private byte[] _encryptedBytes;
        private string _plainText;

        private AesEngine _myAes;
        private RC4Engine _myRc4;
        private byte[] _myIv;
        private byte[] _myKey;

        /// <inheritdoc />
        public CipherBuilder(CryptoConfig config)
        {
            _config = config;

            var keySizeString = GetKeySizeString();

            if (_config.PbeKey == null)
                GenerateKey(_config.CipherAlgorithm.ToString() + keySizeString);

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
            _myKey = gen.GenerateKey();
        }

        private void InitEngine()
        {
            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
                _myRc4 = new RC4Engine();
            else
            {
                _myAes = new AesEngine();
                _myIv = _config.BlockMode == BlockMode.ECB ? null : GenerateIv();
            }
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

        /// <inheritdoc />
        public byte[] EncryptTextToBytes(string input)
        {
            _textBytes = Encoding.UTF8.GetBytes(input);
            KeyParameter keyParam = new KeyParameter(_myKey);

            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
            {
                byte[] outBuffer = new byte[_config.KeySize];
                _myRc4.Init(true, keyParam);
                _myRc4.ProcessBytes(_textBytes, 0, _textBytes.Length, outBuffer, 0);
                //One occurence of broken cipherText with current LINQ. 
                return outBuffer.Where(x => x != 0).ToArray();
            }

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
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes), new ZeroBytePadding());
                        cbc.Init(true, GetKeyParamWithIv(keyParam));
                        _encryptedBytes = cbc.DoFinal(_textBytes);
                    }
                    else
                    {
                        var cbc = new PaddedBufferedBlockCipher(new CbcBlockCipher(_myAes), new Pkcs7Padding());
                        cbc.Init(true, GetKeyParamWithIv(keyParam));
                        _encryptedBytes = cbc.DoFinal(_textBytes);
                    }

                    break;
                case BlockMode.CTS:
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, GetKeyParamWithIv(keyParam));
                    _encryptedBytes = cts.DoFinal(_textBytes);
                    break;

                case BlockMode.OFB:
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes, 8));
                    ofb.Init(true, GetKeyParamWithIv(keyParam));
                    _encryptedBytes = ofb.DoFinal(_textBytes);
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

        public string DecryptBytesToText(byte[] cipherBytes)
        {
            KeyParameter keyParam = new KeyParameter(_myKey);

            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
            {
                byte[] outBuffer = new byte[_config.KeySize];
                _myRc4.Init(false, keyParam);
                _myRc4.ProcessBytes(cipherBytes, 0, cipherBytes.Length, outBuffer, 0);
                UpdatePlainText(outBuffer);
            }
            else
            {
                IBufferedCipher cipher = null;
                IBlockCipherPadding padding;
                //int byteCount = -1;

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
                        var parameters = new AeadParameters(new KeyParameter(_myKey), 128, _myIv, null);
                        gcm.Init(false, parameters);

                        byte[] decryptedBytes = new byte[gcm.GetOutputSize(cipherBytes.Length)];
                        Int32 returnedLength = gcm.ProcessBytes(cipherBytes, 0, cipherBytes.Length, decryptedBytes, 0);

                        var len = gcm.DoFinal(decryptedBytes, 0);
                        // 3 param = len or byteCount?
                        _plainText = Encoding.UTF8.GetString(decryptedBytes, 0, len);
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
            var decryptedBytes = cipher.DoFinal(cipherBytes);
            UpdatePlainText(decryptedBytes);
        }

        private void UpdatePlainText(byte[] decryptedBytes)
        {
            _plainText = Encoding.UTF8.GetString(decryptedBytes);
        }

        private IBlockCipherPadding GetBlockCipherPadding()
        {
            return Padding.ZeroByte == _config.Padding
                ? new ZeroBytePadding()
                : (IBlockCipherPadding)new Pkcs7Padding();
        }
    }
}