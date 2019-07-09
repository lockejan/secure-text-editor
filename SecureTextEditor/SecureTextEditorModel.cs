using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Medja.Controls;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace SecureTextEditor
{
    /// <summary>
    /// Model for STE which contains all necessary parameters and attributes to be stored on disk or in memory
    /// </summary>
    public class SecureTextEditorModel
    {
        private char[] _text;
        public char[] Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private byte[] _myKey; 
        
        private byte[] _myIv; 

        private AesEngine _myAes; 
        
        private string _path = "dummy.txt";
        
        /// <summary>
        /// Default constructor of Model class
        /// </summary>
        public SecureTextEditorModel()
        {
            _myAes = new AesEngine();
            _myKey = null;
            _myIv = null;
        }

        private void GenerateKey(String cipher)
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

        public byte[] EncryptTextToBytes(string plainText, string algo, string blockmode, string padding)
        {
            GenerateKey(algo);
            GenerateIv();

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);

            switch (blockmode)
            {
                case "ECB":
                    if (padding == "ZeroBytePadding")
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

                case "CBC":
                    if (padding == "ZeroBytePadding")
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

                case "CTS":
                    var cts = new CtsBlockCipher(new CbcBlockCipher(_myAes));
                    cts.Init(true, keyParamWithIv);
                    return cts.DoFinal(inputBytes);                        

                case "OFB":
                    var ofb = new BufferedBlockCipher(new OfbBlockCipher(_myAes,8));
                    ofb.Init(true, keyParamWithIv);
                    return ofb.DoFinal(inputBytes);                        

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
            }
            
            return new byte[16];
        }

        public string DecryptText(byte[] cipherBytes, string blockmode, string padding)
        {
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            ParametersWithIV keyParamWithIv = new ParametersWithIV(keyParam, _myIv, 0, 16);
            
            switch (blockmode)
            {
                case "ECB":
                    if (padding == "ZeroBytePadding")
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
                    if (padding == "ZeroBytePadding")
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
        
        public byte[] SHA256(byte[] data)
        {
            Sha256Digest sha256 = new Sha256Digest();
            sha256.BlockUpdate(data, 0, data.Length);
            byte[] hash = new byte[sha256.GetDigestSize()];
            sha256.DoFinal(hash, 0);
            return hash;
        }
        
        public byte[] HMacSha256(byte[] data)
        {
            Sha256Digest sha256 = new Sha256Digest();
            HMac hMac = new HMac(sha256);
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            
            hMac.Init(keyParam);
            
            hMac.BlockUpdate(data, 0, data.Length);
            
            byte[] hash = new byte[hMac.GetMacSize()];

            hMac.DoFinal(hash,0);

            return hash;
        }
        
        public byte[] AesCMac(byte[] data)
        {
            CMac mac = new CMac(_myAes);
            
            KeyParameter keyParam = new KeyParameter(_myKey);
            
            mac.Init(keyParam);
            
            mac.BlockUpdate(data, 0, data.Length);
            
            byte[] hash = new byte[mac.GetMacSize()];
            
            mac.DoFinal(hash,0);

            return hash;
        }
        
        public String LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path,Encoding.UTF8);
//                var cryptoData = File.ReadAllText("dummy.crypto", Encoding.UTF8);
//                _cryptoFabric = JsonConvert.DeserializeObject<SecureTextEditorModel>(cryptoData);
//                Console.WriteLine(_cryptoFabric);
                
//                _path = AssemblyDirectory + "/../../../" + path;
            }

            return "File not found!";
        }
        
        public void SaveTextfile()
        {
//            var tmp = _cryptoFabric.EncryptTextToBytes(Text, _cryptoFabric.KEY);
            
//            Console.WriteLine(JsonConvert.SerializeObject(_cryptoFabric));
//            File.WriteAllText("./dummy.crypto",JsonConvert.SerializeObject(_cryptoFabric), Encoding.UTF8);
//            
//            File.WriteAllText(_path, Convert.ToBase64String(tmp), Encoding.UTF8);
//            FocusManager.Default.SetFocus(_textBox);
        }
        
        private String AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        
    }
    
}