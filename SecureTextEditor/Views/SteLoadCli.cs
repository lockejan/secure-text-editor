using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class SteLoadCli
    {
        private String LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                var cryptoData = File.ReadAllText($"{path}.ste", Encoding.UTF8);
                Console.WriteLine(JsonConvert.DeserializeObject<SteModel>(cryptoData));
                return "Please switch back to your editor";
            }
            return "File not found!";
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadTextDialog()
        {
            Console.WriteLine("Please enter the file you wanna open:\n");
            Console.WriteLine(LoadTextfile(Console.ReadLine()));
            
        }

    }
}