using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CryptoAdapter;
using Newtonsoft.Json;


namespace SecureTextEditor
{
    /// <summary>
    /// CLI class to provide a terminal wizard to setup all parameters
    /// needed for encryption and integrity operations.
    /// </summary>
    public static class SteSaveCli
    {
        private static Dictionary<string, Dictionary<string, string>> setupConfig = new
            Dictionary<string, Dictionary<string, string>>();
        
        private static char[] userPassword;

        private static String WorkingDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        /// <summary>
        /// Entry point of CLI saveDialog.
        /// Prints out welcome message and then hands over to main Dialog function.
        /// </summary>
        public static void SaveDialog()
        {
            Console.WriteLine("\nYou just started the save dialog.\n" +
                              "A wizard will guide you through the options.\n");
            InitDialog();
        }

        private static void InitDialog()
        {
            bool userWantsMore = true;
            const string cipherMenu = "Password Based Encryption or just Cipher?\n" +
                                      " 0. PBE\n" +
                                      " 1. Cipher\n" +
                                      " 2. Get me out of here\n" +
                                      "\nEnter your selection:[0]";
            
            while (userWantsMore)
            {
                Console.WriteLine(cipherMenu);
                var userInput = 0;
                var buffer = Console.ReadLine();
                try
                {
                    userInput = Convert.ToInt32(buffer);
                }
                catch (Exception e)
                {
                    if (buffer != null && !buffer.Equals(""))
                    {
                        Console.WriteLine($"The provided input {buffer} is not valid.\nPlease try again.\n");
                    }
                    continue;
                }

                switch (userInput)
                {
                    case 0: 
                        setupConfig.Clear();
                        PbeDialog();
                        PasswordDialog();
                        IntegrityDialog("");
                        ProcessConfig();
                        FileDialog();
                        userWantsMore = false;
                        break;
                    case 1: 
                        setupConfig.Clear();
                        CipherDialog();
                        BlockModeDialog();
                        PaddingDialog(setupConfig["Cipher"]["BlockMode"]);
                        IntegrityDialog(setupConfig["Cipher"]["BlockMode"]);
                        ProcessConfig();
                        FileDialog();
                        userWantsMore = false;
                        break;
                    case 2:
                        Console.WriteLine("Alright. Cya next time. Bye bye.");
                        userWantsMore = false;
                        break;
                }
            }
            
        }

//        private static void SaveFile(string path, SecureTextEditorModel model)
        private static void SaveFile(string path, string json)
        {
//            string json = JsonConvert.SerializeObject(model);
            File.WriteAllText(path, json);
        }
        
        private static void FileDialog()
        {
            Console.WriteLine("You are currently in the " +
                              $"following directory:\n{WorkingDirectory}");
            Console.WriteLine("Your file will be saved here.\n" +
                              "Please enter a filename to proceed:");
            var fileName = Console.ReadLine();
            
            //TODO save json to drive
            // model = CALL static class which then processes the config list and gives back some sort of object
            SaveFile(fileName, "Hallo Welt");
            
            //#############################################################
            CryptoFactory bouncyCastle = new BouncyCastleFactory(); 
//            CustomCipherFactory cipherObject = bouncyCastle.GetCipher("Cipher/AES/192/CBC/PKCS7"); 
            CustomCipherFactory cipherObject = bouncyCastle.GetCipher(setupConfig); 
            var result = cipherObject.EncryptTextToBytes("Hallo Welt");
            Console.WriteLine(Convert.ToBase64String(result));
            
            var plain = cipherObject.DecryptBytesToText(result);
            Console.WriteLine(plain);
            //############################################################
            
            Console.WriteLine("Your encrypted text has been " +
                              $"saved under '{fileName}.ste'\n" +
                              "with the following configuration:");

            foreach (var entries in setupConfig)
            {
                Console.WriteLine($"\n{entries.Key}:");
                foreach (var value in entries.Value)
                {
                    Console.WriteLine($"   {value.Key} - {value.Value}");
                }

            }
        }

        private static void ProcessConfig()
        {
            //TODO CALL CLASS TO HANDLE MAIN EXECUTION
//            SteMainFactory(setupConfig);
        }
        
        private static void IntegrityDialog(string blockMode)
        {
            string IntegrityDetails(string selected)
            {
                Console.WriteLine("\nThese are your current options:\n");
                int j = 0;
                foreach (var option in SteMenu.IntegrityMenuTree[selected])
                {
                  Console.WriteLine($"{j}. {option}");
                  j++;
                }
                Console.WriteLine("\nPlease enter your selection:[0]");
                return Console.ReadLine();
            }
            
            if (blockMode.Equals("GCM")) return;
            Console.WriteLine("Please choose next which form of integrity should be used.\n");
            int i = 0;
            foreach (var integrityOptions in SteMenu.IntegrityMenuTree)
            {
                Console.WriteLine($"{i}. {integrityOptions.Key}");
                i++;
            }
            var buffer = Console.ReadLine();
            var selectedIntegrityMode = SteMenu.IntegrityMenuTree.Keys.ToList()[Convert.ToInt32(buffer)];
            
            buffer = IntegrityDetails(selectedIntegrityMode);
            var selectedIntegrityOption = SteMenu.IntegrityMenuTree[selectedIntegrityMode][Convert.ToInt32(buffer)];
            setupConfig.Add("Integrity",new Dictionary<string, string>(){{"Mode",selectedIntegrityMode}});
            setupConfig["Integrity"].Add("Option", selectedIntegrityOption);
        }

        private static void PaddingDialog(string blockMode)
        {
            Console.WriteLine("Please choose next which padding should be used.\n");
            int i = 0;
            foreach (var padding in SteMenu.CipherOptionsMenuTree[blockMode])
            {
                Console.WriteLine($"{i}. {padding}");
                i++;
            }
            var buffer = Console.ReadLine();
            var selectedPadding = SteMenu.CipherOptionsMenuTree[blockMode][Convert.ToInt32(buffer)];
            setupConfig["Cipher"].Add("Padding",selectedPadding);
        }

        private static void BlockModeDialog()
        {
            Console.WriteLine("Please choose next which block mode should be used.\n");
            int i = 0;
            foreach (var blockMode in SteMenu.CipherOptionsMenuTree)
            {
                Console.WriteLine($"{i}. {blockMode.Key}");
                i++;
            }
            var buffer = Console.ReadLine();
            var selectedBlockMode = SteMenu.CipherOptionsMenuTree.Keys.ToList()[Convert.ToInt32(buffer)];
            setupConfig["Cipher"].Add("BlockMode",selectedBlockMode);
        }

        private static void CipherDialog()
        {
            const string ciperAlgorithm = "AES";
            Console.WriteLine("Currently there is only AES available for encryption.\nWhich key length you wanna use?\n");
            foreach (var cipherCollection in SteMenu.CipherMenuTree)
            {
                int i = 0;
                foreach (var keySize in cipherCollection.Value)
                {
                    Console.WriteLine($"{i}. {keySize} bit");
                    i++;
                }
            }
            var buffer = Console.ReadLine();
            var selectedKeyLength = SteMenu.CipherMenuTree["AES"][Convert.ToInt32(buffer)];
            setupConfig.Add("Cipher", new Dictionary<string, string>(){{"Algorithm",ciperAlgorithm}});
            setupConfig["Cipher"].Add("KeySize",selectedKeyLength);
        }

        private static void PasswordDialog()
        {
            Console.WriteLine("\nYou have to provide a password in Order to use PBE!\nPlease enter: ");
            var userPassword = Console.ReadLine();
            //TODO change password input to some sort of CharArray
            //TODO WRITE Password to CharArray coming from readLine -> currently static string
            setupConfig["PBE"].Add("Password",userPassword);
        }

        private static void PbeDialog()
        {
            Console.WriteLine("Which PBE option you wanna use?\n");
            int i = 0;
            foreach (var pbeAlgorithm in SteMenu.PBEMenuTree)
            {
                Console.WriteLine($"{i}. PBE{pbeAlgorithm.Key}");
                i++;
            }
            var buffer = Console.ReadLine();
            var selectedPbe = SteMenu.PBEMenuTree.Keys.ToList()[Convert.ToInt32(buffer)];
            setupConfig.Add("PBE", new Dictionary<string, string>(){{"Algorithm",selectedPbe}});
//            setupConfig["PBE"].Add("Cipher",pbeCipher);
//            setupConfig["PBE"].Add("Blockmode",pbeBlockMode);
//            setupConfig["PBE"].Add("Padding",pbePadding);
        }
        
    }

//    public class NameValuePair
//    {
//        KeyValuePair<string, string> it;
//
//        public NameValuePair(string name, string value)
//        {
//            it = new KeyValuePair<string, string>( name, value );
//        }
//
//        public string Name
//        {
//            get { return it.Key; }
//        }
//
//        public string Value
//        {
//            get { return it.Value; }
//        }
//    }
}