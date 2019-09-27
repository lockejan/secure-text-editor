using System;
using System.IO;
using System.Text;
using BcFactory;
using Newtonsoft.Json;
using SecureTextEditor.FileHandler;

namespace SecureTextEditor.CLI
{

    public static class SteLoadCli
    {
        private static CryptoConfig _config;
        private static String LoadSteFile(String path)
        {
            path = SteHelper.WorkingDirectory + path;
            if (File.Exists($"{path}.ste"))
            {
                try
                {
                    var cryptoData = File.ReadAllText($"{path}.ste", Encoding.UTF8);
                    _config = JsonConvert.DeserializeObject<CryptoConfig>(cryptoData);
                    
                    if (File.Exists($"{path}.key"))
                        _config.Key = File.ReadAllBytes($"{path}.key");
                    
                    if (_config.IsIntegrityActive && File.Exists($"{path}.pem"))
                        _config.SignaturePublicKey = File.ReadAllText($"{path}.pem");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return SteCryptoHandler.ProcessConfigToLoad(_config);
            }
            return "File not found!";
        }
        
        public static String LoadTextDialog()
        {
            Console.WriteLine("Please enter the file you wanna open:\n");
            return LoadSteFile(Console.ReadLine());
        }

    }
}