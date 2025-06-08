using System;
using System.IO;
using System.Text;
using CryptoEngine;
using CryptoEngine.Resources;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace SecureTextEditor
{
    /// <summary>
    /// Static class which is responsible for file IO operations.
    /// Loading and saving KeyFiles and JSON-CryptoConfig-Object.
    /// Calls the CipherFactory to trigger processing of the CryptoConfig object.
    /// Defines all used file extensions.
    /// </summary>
    public static class FileHandler
    {
        private const string FileExtension = "ste";
        private const string KeyExtension = "key";
        private const string DigestKeyExtension = "digKey";
        private const string PrivKeyExtension = "privKey";
        private const string PubKeyExtension = "pubKey";
        
        /// <summary>
        /// Triggers processing of CryptoConfig object.
        /// Passes Config object to processing class constructors. 
        /// </summary>
        /// <param name="plainText">string coming from MainView's TextEditor.</param>
        /// <param name="config">CryptoConfig object holding all config and cipher parameters.</param>
        /// <returns>updated CryptoConfig object</returns>
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
                var digestBuilder = CryptoFactory.CreateDigest(config);
                config = digestBuilder.SignInput(config.Cipher);
            }
            
            return config;
        }

        /// <summary>
        /// Checks for present keys, creates a full qualified filename and calls proper save-methods. 
        /// </summary>
        /// <param name="fileName">filename string which has been provided in SaveDialog.</param>
        /// <param name="config">CryptoConfig object which contains keyFiles and config</param>
        public static void SaveToDisk(string fileName, CryptoConfig config)
        {
            var fqfn = WorkingDirectory + fileName;
            
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
        
        
        /// <summary>
        /// Tries to read and to deserialize json-cryptoConfig-file from disk.
        /// </summary>
        /// <param name="fileName">string containing filename without file-extension</param>
        /// <returns>deserialized cryptoConfig object</returns>
        public static CryptoConfig LoadSteFile(string fileName)
        {
            var fqfn = WorkingDirectory + fileName;
            
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

        /// <summary>
        /// Checks configuration and then tries to load all needed secrets from disk.
        /// Secrets can be base64 strings or bytes. 
        /// </summary>
        /// <param name="fileName">string coming from loadDialog.</param>
        /// <param name="config">cryptoConfig object which holds config state.</param>
        /// <returns>updated cryptoConfig object with loaded secrets</returns>
        public static CryptoConfig LoadKeys(string fileName, CryptoConfig config)
        {
            var fqfn = WorkingDirectory + fileName;
            
            if (!config.IsPbeActive && File.Exists($"{fqfn}.{KeyExtension}"))
                config.Key = File.ReadAllBytes($"{fqfn}.{KeyExtension}");

            if (!config.IsIntegrityActive) return config;
            
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
        
        /// <summary>
        /// Triggers processing of CryptoConfig object.
        /// Passes Config object to processing class constructors.
        /// Also verifies integrity of cipher if used. 
        /// </summary>
        /// <param name="config">config state after loading all files from disk.</param>
        /// <returns>decrypted cipher in string representation.</returns>
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
            catch (InvalidCipherTextException e)
            {
                Console.WriteLine(e);
                return "GCM mac error.";
            }
            catch (FormatException e)
            {
                Console.WriteLine(e);
                return "Format error.";
            }
        }

        private static string WorkingDirectory
        {
            get
            {
                string codeBase = Directory.GetCurrentDirectory() + "/../../../../";
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return $"{Path.GetDirectoryName(path)}/";
            }
        }
    }
}