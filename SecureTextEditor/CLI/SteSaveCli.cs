using System;
using BcFactory;
using SecureTextEditor.FileHandler;

namespace SecureTextEditor.CLI
{
    /// <summary>
    /// CLI class to provide a terminal wizard to setup all parameters
    /// needed for encryption and integrity operations.
    /// </summary>
    public class SteSaveCli
    {
        private readonly CryptoConfig _config = new CryptoConfig();

        private char[] _pbePassword;

        /// <summary>
        /// Entry point of CLI saveDialog.
        /// Prints out welcome message and then hands over to main Dialog function.
        /// </summary>
        public void SaveDialog(string plainText)
        {
            Console.WriteLine("\nYou just started the save dialog.\n" +
                              "A wizard will guide you through the options.\n");
            const string cipherMenu = "Password Based Encryption or just Cipher?\n" +
                                      " 0. PBE\n" +
                                      " 1. Cipher\n" +
                                      " 2. Get me out of here\n" +
                                      "\nEnter your selection:[0]";

            //_config.Clear();

            Console.WriteLine(cipherMenu);
            var userInput = ReadInt();

            switch (userInput)
            {
                case 0:
                    PbeDialog();
                    PasswordDialog();
                    IntegrityDialog(BlockMode.None);
                    FileDialog(plainText);
                    break;
                case 1:
                    CipherDialog();
                    BlockModeDialog();
                    PaddingDialog(_config.BlockMode);
                    IntegrityDialog(_config.BlockMode);
                    FileDialog(plainText);
                    break;
                default: // all other 
                    Console.WriteLine("Alright. Cya next time. Bye bye.");
                    break;
            }
        }

        private static int ReadInt()
        {
            int result;

            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine($"The provided input is not valid.\nPlease try again.\n");
            }

            return result;
        }

        private void FileDialog(string plainText)
        {
            Console.WriteLine("\nYou are currently in the " +
                              $"following directory:\n{SteHelper.WorkingDirectory}");
            Console.WriteLine("Your file will be saved here.\n" +
                              "Please enter a filename to proceed:");
            var fileName = Console.ReadLine();

            try
            {
                SteCryptoHandler.ProcessConfigToSave(fileName, plainText, _config);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine("Your encrypted plainText has been " +
                              $"saved under '{fileName}.ste'\n" +
                              "with the following configuration:");
            Console.WriteLine(_config);
        }

        private void IntegrityDialog(BlockMode blockMode)
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

            if (blockMode == BlockMode.GCM) return;
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
            _config.Integrity = selectedIntegrityMode;
            _config.IntegrityOptions = selectedIntegrityOption;
        }

        private void PaddingDialog(BlockMode blockMode)
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
            _config.Padding = selectedPadding;
        }

        private void BlockModeDialog()
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
            _config.BlockMode = selectedBlockMode;
        }

        private void CipherDialog()
        {
            CipherAlgorithm cipherAlgorithm = CipherAlgorithm.AES;
            Console.WriteLine("Currently there is only AES available for encryption.\nWhich key length you wanna use?\n");
            int i = 0;
            foreach (var keySize in KeySize.AES)
            {
                Console.WriteLine($"{i}. {keySize} bit");
                i++;
            }
            var buffer = Console.ReadLine();
            _config.CipherAlgorithm = cipherAlgorithm;
            _config.KeySize = KeySize.AES[Convert.ToInt32(buffer)];
        }

        private void PasswordDialog()
        {
            Console.WriteLine("\nYou have to provide a password in Order to use PBE!\nPlease enter: ");
            var userPassword = Console.ReadLine();
            //TODO change password input to some sort of CharArray
            //TODO WRITE Password to CharArray coming from readLine -> currently static string
            _config.PbePassword = userPassword.ToCharArray();
        }

        private void PbeDialog()
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
            _config.PbeAlgorithm = selectedPbe;
        }

    }
}