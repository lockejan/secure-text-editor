using System;
using System.IO;
using System.Text;
using BcFactory;
using Newtonsoft.Json;

namespace SecureTextEditor.FileHandler
{
    /// <summary>
    /// Entry class for loading and saving processes.
    /// Handles all necessary calls to further process all tasks needed to encrypt or decrypt and then gets back to Editor.
    /// </summary>
    public static class SteCryptoHandler
    {
        
        private static CryptoConfig _config;
        
        /// <summary>
        /// Currently only BouncyCastle is implemented.
        /// Depending on the configuration of setupConfig different factories will then called.
        /// After processing of configuration the return values will be transferred to JSON-Model.
        /// (Finally the serialized or deserialized JSON will then returned.)#NotYet
        /// </summary>
        /// <param name="fileName">fileName to be used to save files to disk.</param>
        /// <param name="plainText">String plainText coming from Editor.</param>
        /// <param name="config">CryptoConfig object which holds parameters and results needed to further process</param>
        public static void ProcessConfigToSave(string fileName, string plainText, CryptoConfig config)
        {
            fileName = SteHelper.WorkingDirectory + fileName;

            if (config.IsEncryptActive)
            {
                if (config.IsPbeActive)
                {
                    var pbeEncrypt = CryptoFactory.Create(config);
                    // JUST FOR TESTING - CHANGE SIGNATURE TO ACCEPT CHAR[]   
                    config.Key = pbeEncrypt.EncryptTextToBytes(config.PbePassword.ToString());
                }

                var crypt = CryptoFactory.Create(config);
                config.Cipher = Convert.ToBase64String(crypt.EncryptTextToBytes(plainText));
                config.IvOrSalt = crypt.MyIv;
                config.Key = crypt.MyKey;
                
                var decrypted = crypt.DecryptBytesToText(Convert.FromBase64String(config.Cipher));
                Console.WriteLine($"Decrypted cipher: {decrypted}");
            }

            if (config.IsIntegrityActive)
            {
                var sign = IntegrityFactory.Create(config);
                config.Signature = Convert.ToBase64String(sign.SignBytes(config.Cipher));
                
                //var verified = sign.VerifySign(config.Signature, config);
                //Console.WriteLine($"Signature/Digest verified: {verified}");
                if (config.IsIntegrityActive)
                    SavePrivateCert(fileName,config.SignaturePrivateKey);
            }

            SaveKey(fileName, config.Key);
            
            SaveFile(fileName,config);
        }
        
        public static CryptoConfig LoadSteFile(String path)
        {
            path = SteHelper.WorkingDirectory + path;
            
                try
                {
                    if (File.Exists($"{path}.ste"))
                    {
                        var cryptoData = File.ReadAllText($"{path}.ste", Encoding.UTF8);
                        _config = JsonConvert.DeserializeObject<CryptoConfig>(cryptoData);
                        
                        if (File.Exists($"{path}.key"))
                            _config.Key = File.ReadAllBytes($"{path}.key");
                        
                        if (_config.IsIntegrityActive && File.Exists($"{path}.pem"))
                            _config.SignaturePublicKey = File.ReadAllText($"{path}.pem");
                        return _config;
                    }
                    throw new FileNotFoundException("Given filename doesn't exist.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }
        
        public static string ProcessConfigToLoad(CryptoConfig config)
        {
            var cryptoConfig = CryptoFactory.Create(config);
            return cryptoConfig.DecryptBytesToText(Convert.FromBase64String(config.Cipher));
        }
        
        private static void SaveKey(string path, byte[] key)
        {
            File.WriteAllBytes($"{path}.key", key);
        }
        private static void SavePrivateCert(string path, string privateKey)
        {
            File.WriteAllText($"{path}.pkey", privateKey, Encoding.UTF8);
        }
        
        private static void SaveFile(string path, CryptoConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include });
            File.WriteAllText($"{path}.ste", json, Encoding.UTF8);
        }

    }
}