using System;
using System.IO;
using System.Text;
using BcFactory;
using BcFactory.Resources;
using Newtonsoft.Json;

namespace SecureTextEditor.FileHandler
{

    public static class FileHandler
    {
        private const string FileExtension = "ste";
        private const string KeyExtension = "key";
        private const string DigestKeyExtension = "digKey";
        private const string PrivKeyExtension = "privKey";
        private const string PubKeyExtension = "pubKey";
        
        public static CryptoConfig ProcessConfigOnSave(string plainText, CryptoConfig config)
        {
            if (config.IsEncryptActive)
            {
                if (config.IsPbeActive)
                {
                    var pbeBuilder = CryptoFactory.CreatePbe(config);
                    config = pbeBuilder.GenerateKeyBytes();
                }

                var cipherBuilder = CryptoFactory.CreateCipher(config);
                config = cipherBuilder.EncryptTextToBytes(plainText);
                //Array.Clear(config.Key,0, config.Key.Length);
            }

            if (!config.IsIntegrityActive) return config;

            if (config.Integrity == Integrity.Dsa)
            {
                var certBuilder = CryptoFactory.CreateCert(config);
                certBuilder.GenerateCerts();
                config = certBuilder.SignInput(config.Cipher);
            }
            else
            {
                var certBuilder = CryptoFactory.CreateDigest(config);
                config = certBuilder.SignInput(config.Cipher);
            }
            
            return config;
        }

        public static void SaveToDisk(string fileName, CryptoConfig config)
        {
            var fqfn = SteHelper.WorkingDirectory + fileName;
            
            if (config.Key != null)
                SaveKey($"{fqfn}.{KeyExtension}", config.Key);
            
            if (config.SignaturePrivateKey != null)
                SaveKey($"{fqfn}.{PrivKeyExtension}", config.SignaturePrivateKey);
            
            if (config.SignaturePublicKey != null)
                SaveKey($"{fqfn}.{PubKeyExtension}", config.SignaturePublicKey);
            
            if (config.DigestKey != null)
                SaveKey($"{fqfn}.{DigestKeyExtension}", config.DigestKey);
            
            SaveFile($"{fqfn}.{FileExtension}", config);
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
            var json = JsonConvert.SerializeObject(config, Formatting.Indented,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include });
            File.WriteAllText(path, json, Encoding.UTF8);
        }
        
        
        public static CryptoConfig LoadSteFile(string fileName)
        {
            var fqfn = SteHelper.WorkingDirectory + fileName;
            
                try
                {
                    var cryptoData = File.ReadAllText($"{fqfn}.{FileExtension}", Encoding.UTF8);
                    CryptoConfig config = JsonConvert.DeserializeObject<CryptoConfig>(cryptoData);
                    return config;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e + $"Given file doesn't exist in DIR {fqfn}.");
                    throw;
                }
        }

        public static CryptoConfig LoadKeys(string fileName, CryptoConfig config)
        {
            var fqfn = SteHelper.WorkingDirectory + fileName;
            
            if (!config.IsPbeActive && File.Exists($"{fqfn}.{KeyExtension}"))
                config.Key = File.ReadAllBytes($"{fqfn}.{KeyExtension}");
                
            if (config.IsIntegrityActive)
                switch (config.Integrity)
                {
                    case Integrity.Digest:
                        if(!File.Exists($"{fqfn}.{DigestKeyExtension}")) break;
                        config.DigestKey = File.ReadAllBytes($"{fqfn}.{DigestKeyExtension}");
                        break;
                    case Integrity.Dsa:
                        if(!File.Exists($"{fqfn}.{PubKeyExtension}")) break;
                        config.SignaturePublicKey = File.ReadAllText($"{fqfn}.{PubKeyExtension}");
                        if(!File.Exists($"{fqfn}.{PrivKeyExtension}")) break;
                        config.SignaturePrivateKey = File.ReadAllText($"{fqfn}.{PrivKeyExtension}");
                        break;
                }
            return config;
        }
        
        public static string ProcessConfigOnLoad(CryptoConfig config)
        {
            if (config.IsIntegrityActive)
            {
                if (config.Integrity == Integrity.Dsa)
                {
                    var certBuilder = CryptoFactory.CreateCert(config);
                    certBuilder.GenerateCerts();
                    config = certBuilder.SignInput(config.Cipher);
                    
                    var result = certBuilder.VerifySign(config.Signature, config.Cipher);
                    Console.WriteLine($"Signature verified: {result}");
                }
                else
                {
                    var certBuilder = CryptoFactory.CreateDigest(config);
                    config = certBuilder.SignInput(config.Cipher);
                    
                    var result = certBuilder.VerifySign(config.Signature, config.Cipher);
                    Console.WriteLine($"Digest verified: {result}");
                }
            }

            if (config.IsPbeActive)
            {
                var pbeBuilder = CryptoFactory.CreatePbe(config);
                config = pbeBuilder.GenerateKeyBytes();
            }
            
            var cipherBuilder = CryptoFactory.CreateCipher(config);
            try
            {
                return cipherBuilder.DecryptBytesToText(Convert.FromBase64String(config.Cipher));

            }
            catch (Org.BouncyCastle.Crypto.InvalidCipherTextException e)
            {
                Console.WriteLine(e);
                //throw;
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
            }

            //return cipherBuilder.DecryptBytesToText(Convert.FromBase64String(config.Cipher));
            return "not good";
        }

    }
}