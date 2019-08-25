using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CryptoAdapter;
using Newtonsoft.Json;

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
        /// After processing of configuration the return values will be transfered to JSON-Model.
        /// (Finally the serialized or deserialized JSON will then returned.)#NotYet
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="plainText"></param>
        /// <param name="setupConfig"></param>
        public static void ProcessConfigToSave(string fileName, string plainText, Dictionary<string, Dictionary<string, string>> setupConfig)
        {
            CryptoFactory bouncyCastle = new BouncyCastleFactory();
            CustomCipherFactory cipherObject = bouncyCastle.GetCipher(setupConfig);
            var encresult = cipherObject.EncryptTextToBytes(plainText);
            var result = cipherObject.Result();
            //var iV;
            //var cipher;
            //var key;
            Console.WriteLine(Convert.ToBase64String(encresult));

            var plain = cipherObject.DecryptBytesToText(encresult);
            Console.WriteLine(plain);

            fileName = $"../../../{fileName}";
            SaveFile(fileName,result);
        }

        public static void ProcessConfigToLoad(Dictionary<string, Dictionary<string, string>> setupConfig)
        {
            Console.WriteLine("ProcessConfigToLoad");
        }
        
        
        //private static void SaveFile(string path, SecureTextEditorModel model)
        private static void SaveFile(string path, Dictionary<string, string> model)
        {
            //TODO save json to hardDrive
            string json = JsonConvert.SerializeObject(model);
            File.WriteAllText($"./{path}.ste", json, Encoding.UTF8);
        }
        
        

    }
}