using System;
using BcFactory;
using BcFactory.Resources;
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
                    PaddingDialog();
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
            IntegrityOptions IntegrityOptions()
            {
                Console.WriteLine("\nThese are your current options:\n");
                int j = 0;
                foreach (var option in _config.GetIntegrityOptions())
                {
                    Console.WriteLine($"{j}. {option}");
                    j++;
                }
                Console.WriteLine("\nPlease enter your selection:[0]");
                var selected = Console.ReadLine();
                return (IntegrityOptions) Convert.ToInt32(selected);
            }

            if (blockMode == BlockMode.GCM) return;
            Console.WriteLine("Please choose next which form of integrity should be used.\n");
            int i = 0;
            foreach (Integrity options in Enum.GetValues(typeof(Integrity)))
            {
                Console.WriteLine($"{i}. {options}");
                i++;
            }
            var buffer = Console.ReadLine();
            _config.Integrity = (Integrity)Convert.ToInt32(buffer);
            _config.IntegrityOptions = IntegrityOptions();
        }

        private void PaddingDialog()
        {
            Console.WriteLine("Please choose next which padding should be used.\n");
            int i = 0;
            foreach (var padding in _config.GetValidPaddings())
            {
                Console.WriteLine($"{i}. {padding}");
                i++;
            }
            var buffer = Console.ReadLine();
            _config.Padding = (Padding)Convert.ToInt32(buffer);
        }

        private void BlockModeDialog()
        {
            Console.WriteLine("Please choose next which block mode should be used.\n");
            int i = 0;
            foreach (var blockMode in _config.GetValidBlockModes())
            {
                Console.WriteLine($"{i}. {blockMode}");
                i++;
            }
            var buffer = Console.ReadLine();
            _config.BlockMode = (BlockMode)Convert.ToInt32(buffer);
        }

        private void CipherDialog()
        {
            CipherAlgorithm cipherAlgorithm = CipherAlgorithm.AES;
            Console.WriteLine("Currently there is only AES available for encryption.\nWhich key length you wanna use?\n");
            int i = 0;
            foreach (var keySize in _config.GetKeySizes())
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
            foreach (PbeAlgorithm algorithm in Enum.GetValues(typeof(PbeAlgorithm)))
            {
                Console.WriteLine($"{i}. {algorithm}");
                i++;
            }
            var buffer = Console.ReadLine();
            _config.PbeAlgorithm = (PbeAlgorithm)Convert.ToInt32(buffer);
        }

    }
}