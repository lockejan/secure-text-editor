using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CryptoAdapter;
using Newtonsoft.Json;
using SecureTextEditor.FileHandler;

namespace SecureTextEditor
{
    /// <summary>
    /// 
    /// </summary>
    public static class SteCryptoHandler
    {
        /// <summary>
        /// Method to call CryptoAdapter.
        /// Will then get back some a cryptoFactory.
        /// Currently only BouncyCastle is implemented.
        /// Depending on the configuration of setupConfig different factories will then called.
        /// After processing of configuration the return values will be transferred to JSON-Model.
        /// (Finally the serialized or deserialized JSON will then returned.)#NotYet
        /// </summary>
        /// <param name="fileName">Path including fileName relative to the current dir.</param>
        /// <param name="plainText">String plainText coming from Editor.</param>
        /// <param name="setupConfig">Dict holding all collected details to further process</param>
        public static void ProcessConfigToSave(string fileName, string plainText, Dictionary<string, Dictionary<string, string>> setupConfig)
        {
            fileName = $"../../../{fileName}";
            
            CryptoFactory bouncyCastle = new BouncyCastleFactory();
            CustomCipherFactory cipherObject = bouncyCastle.GetCipher(setupConfig);
            var encrypted = cipherObject.EncryptTextToBytes(plainText);
            var result = cipherObject.Result();
            SaveFile(fileName, result.GetValueOrDefault("Key"));
            result.Remove("Key");
            
            SteModel jsonModel = new SteModel("UTF8",setupConfig,
                result.GetValueOrDefault("Iv"),
                result.GetValueOrDefault("PublicKey"),
                result.GetValueOrDefault("Signature"),
                result.GetValueOrDefault("Cipher"));
            
            SaveFile(fileName,jsonModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="setupConfig"></param>
        /// <returns></returns>
        public static string ProcessConfigToLoad(string key,Dictionary<string, string> setupConfig)
        {
            CryptoFactory bouncyCastle = new BouncyCastleFactory();

            var iv = setupConfig["IvOrSalt"];
            
            CustomCipherFactory cipherObject = bouncyCastle.GetCipher(key, iv, setupConfig);
            return cipherObject.DecryptBytesToText(Convert.FromBase64String(setupConfig["cipher"]));
        }


        private static void SaveFile(string path, string key)
        {
            File.WriteAllText($"./{path}.key", key, Encoding.UTF8);
        }
        private static void SaveFile(string path, SteModel model)
        {
            string json = JsonConvert.SerializeObject(model,Formatting.Indented,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            File.WriteAllText($"./{path}.ste", json, Encoding.UTF8);
        }
        

    }
}