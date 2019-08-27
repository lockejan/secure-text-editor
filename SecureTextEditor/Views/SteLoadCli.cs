using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// 
    /// </summary>
    public static class SteLoadCli
    {
        private static SteModel _jsonModel;
        private static string _key;
        
        private static String LoadTextfile(String path)
        {
            path = SteHelper.WorkingDirectory + "/../../../" + path;
            if (File.Exists($"{path}.ste"))
            {
                var cryptoData = File.ReadAllText($"{path}.ste", Encoding.UTF8);
                _jsonModel = JsonConvert.DeserializeObject<SteModel>(cryptoData);
                
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    {"Algorithm", "AES"},
                    {"KeySize", "192"},
                    {"BlockMode", "CBC"},
                    {"Padding", "PKCS7"},
                    {"IvOrSalt", "oKqt6baRQ+6/m7J59TTDmizRKwVLybQz"},
                    {"cipher", "q5ca6f0RDjljoMJa0zHBGQ=="}
                };
                
                _key = File.ReadAllText($"{path}.key", Encoding.UTF8);
                return SteCryptoHandler.ProcessConfigToLoad(_key, param);
            }
            return "File not found!";
        }

        /// <summary>
        /// 
        /// </summary>
        public static String LoadTextDialog()
        {
            Console.WriteLine("Please enter the file you wanna open:\n");
            return LoadTextfile(Console.ReadLine());
        }

    }
}