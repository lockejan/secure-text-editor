using System;
using System.IO;
using System.Text;
using BcFactory;
using BcFactory.Resources;
using Newtonsoft.Json;

namespace SecureTextEditor.FileHandler
{

    public static class SteCryptoHandler
    {
        public static CryptoConfig ProcessConfigOnSave(string plainText, CryptoConfig config)
        {
            if (config.IsEncryptActive)
            {
                if (config.IsPbeActive)
                {
                    var pbe = CryptoFactory.CreatePbe(config, config.PbePassword);
                    config = pbe.GenerateKeyBytes(config.PbePassword);
                    Array.Clear(config.Key,0, config.Key.Length);
                }

                var crypt = CryptoFactory.CreateCipher(config);
                config = crypt.EncryptTextToBytes(plainText);
            }

            if (!config.IsIntegrityActive) return config;
            
            var sign = config.Integrity switch
            {
                Integrity.Digest => CryptoFactory.CreateDigest(config),
                Integrity.Dsa => CryptoFactory.CreateCert(config),
                _ => throw new ArgumentException("Unsupported Integrity Mode!")
            };
                
            config = sign.SignBytes(config.Cipher);
            return config;
        }

        public static void SaveToDisk(string fileName, CryptoConfig config)
        {
            fileName = SteHelper.WorkingDirectory + fileName;
            
            if (config.Key != null)
                SaveKey(fileName, config.Key);
            
            if (config.SignaturePrivateKey != null)
                SaveKey($"{fileName}.privKey", config.SignaturePrivateKey);
            
            if (config.SignaturePublicKey != null)
                SaveKey($"{fileName}.pubKey", config.SignaturePublicKey);
            
            if (config.DigestKey != null)
                SaveKey(fileName, config.DigestKey);
            
            SaveFile(fileName, config);
        }
        
        private static void SaveKey(string path, byte[] key)
        {
            File.WriteAllBytes(path, key);
        }
        
        private static void SaveKey(string path, string key)
        {
            File.WriteAllText(path, key);
        }
        
        private static void SaveFile(string path, CryptoConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include });
            File.WriteAllText($"{path}.ste", json, Encoding.UTF8);
        }
        
        
        public static CryptoConfig LoadSteFile(string path)
        {
            path = SteHelper.WorkingDirectory + path;
            
                try
                {
                    var cryptoData = File.ReadAllText($"{path}.ste", Encoding.UTF8);
                    CryptoConfig config = JsonConvert.DeserializeObject<CryptoConfig>(cryptoData);
                    return config;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e + $"Given file doesn't exist in DIR {path}.");
                    throw;
                }
        }

        public static CryptoConfig LoadKeys(string path, CryptoConfig config)
        {
            path = SteHelper.WorkingDirectory + path;
            
            if (!config.IsPbeActive && File.Exists($"{path}.key"))
                config.Key = File.ReadAllBytes($"{path}.key");
                
            if (config.IsIntegrityActive)
                switch (config.Integrity)
                {
                    case Integrity.Digest:
                        if(!File.Exists($"{path}.digKey")) break;
                        config.DigestKey = File.ReadAllBytes($"{path}.digKey");
                        break;
                    case Integrity.Dsa:
                        if(!File.Exists($"{path}.pubKey")) break;
                        config.SignaturePublicKey = File.ReadAllText($"{path}.pubKey");
                        if(!File.Exists($"{path}.privKey")) break;
                        config.SignaturePrivateKey = File.ReadAllText($"{path}.privKey");
                        break;
                }
            return config;
        }
        
        public static string ProcessConfigOnLoad(CryptoConfig config)
        {
            if (config.IsPbeActive)
            {
                var pbe = CryptoFactory.CreatePbe(config, config.PbePassword);
                config = pbe.GenerateKeyBytes(config.PbePassword);
            }
            
            var cryptoConfig = CryptoFactory.CreateCipher(config);

            if (!config.IsIntegrityActive)
                return cryptoConfig.DecryptBytesToText(Convert.FromBase64String(config.Cipher));
            
            var sign = config.Integrity switch
            {
                Integrity.Digest => CryptoFactory.CreateDigest(config),
                Integrity.Dsa => CryptoFactory.CreateCert(config),
                _ => throw new ArgumentException("Unsupported Integrity Mode!")
            };
            
            config = sign.SignBytes(config.Cipher);
                
            var verified = sign.VerifySign(config.Signature);
            Console.WriteLine($"Signature/Digest verified: {verified}");

            return cryptoConfig.DecryptBytesToText(Convert.FromBase64String(config.Cipher));
        }

    }
}