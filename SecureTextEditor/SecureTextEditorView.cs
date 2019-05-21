using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace SecureTextEditor
{
    public class SecureTextEditorView : ContentControl
    {
        private string _path;
        private readonly IControlFactory _controlFactory;

        private Button _loadBtn;
        private Button _saveBtn;
        private Button _decryptBtn;
        private Button _encryptBtn;

        private ComboBox _paddingModeComboBox;
        private ComboBox _blockModeComboBox;
        
        private TextEditor _textBox;
        public string Text
        {
            get { return _textBox.GetText(); }
            set { _textBox.SetText(value); }
        }

        private Aes _myAes;

        /// <summary>
        /// Creates mainView Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SecureTextEditorView(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _textBox = controlFactory.Create<TextEditor>();

            CreateCryptoHandler();
            
            CreateButtons(controlFactory);
            RegisterButtonEvents();
            
            CreateComboBoxes(controlFactory);
            
//            var buttonStackPanel = CreateButtonPanel(controlFactory);
//            CreateDockPanel(buttonStackPanel, controlFactory);
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_textBox);
        }

        private void CreateCryptoHandler()
        {
            _myAes = Aes.Create();
            _myAes.Mode = CipherMode.ECB;
            _myAes.Padding = PaddingMode.None;
        }

        private Control CreateDockPanel()
        {
            var dockPanel = _controlFactory.Create<DockPanel>();
            dockPanel.Add(Dock.Bottom, CreateButtonPanel());
            dockPanel.Add(Dock.Fill, _textBox);
            dockPanel.Background = _textBox.Background;

            return dockPanel;
        }

        private HorizontalStackPanel CreateButtonPanel()
        {
            var buttonStackPanel = _controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 90;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Add(_loadBtn);
            buttonStackPanel.Add(_saveBtn);
            buttonStackPanel.Add(_decryptBtn);
            buttonStackPanel.Add(_encryptBtn);
            buttonStackPanel.Add(_paddingModeComboBox);
            buttonStackPanel.Add(_blockModeComboBox);
            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
//            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.SetLeftAndRight(5);
            buttonStackPanel.Margin.SetTopAndBottom(5);

            return buttonStackPanel;
        }

        private void CreateButtons(IControlFactory controlFactory)
        {
            _loadBtn = controlFactory.Create<Button>();
            _loadBtn.Text = "Load";
            
            _saveBtn = controlFactory.Create<Button>();
            _saveBtn.Text = "Save";
            
            _decryptBtn = controlFactory.Create<Button>();
            _decryptBtn.Text = "Decrypt";
            
            _encryptBtn = controlFactory.Create<Button>();
            _encryptBtn.Text = "Encrypt";
        }

        private void CreateComboBoxes(IControlFactory controlFactory)
        {
            _paddingModeComboBox = controlFactory.Create<ComboBox>();
            _paddingModeComboBox.Title = "Padding";
            _paddingModeComboBox.Add("None");
            _paddingModeComboBox.Add("PKCS7");
            _paddingModeComboBox.Add("ZeroByte");
            
            _blockModeComboBox = controlFactory.Create<ComboBox>();
            _blockModeComboBox.Title = "Blockmode";
            _blockModeComboBox.Add("ECB");
            _blockModeComboBox.Add("CBC");
            _blockModeComboBox.Add("OFB");
            _blockModeComboBox.Add("CTS");
            _blockModeComboBox.Add("GCM");
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
            _decryptBtn.InputState.Clicked += OnDecryptButtonClicked;
            _encryptBtn.InputState.Clicked += OnEncryptButtonClicked;
        }


        private void LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                Text = File.ReadAllText(path,Encoding.UTF8);
                _path = AssemblyDirectory + "/../../../" + path;
            }
            FocusManager.Default.SetFocus(_textBox);
        }
        
        private void SaveTextfile()
        {
            EncryptTextToBytes(Text, _myAes.Key, _myAes.IV);
            File.WriteAllText(_path, Text, Encoding.UTF8);
            FocusManager.Default.SetFocus(_textBox);
        }

        private byte[] EncryptTextToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            
            // Return the encrypted bytes from the memory stream.
//            Text = Convert.ToBase64String(encrypted);
            return encrypted;
        }

        private string DecryptText(byte[] cipherBytes, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherBytes == null || cipherBytes.Length <= 0)
                throw new ArgumentNullException("cipherBytes");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            
//            byte[] cipherBytes = Encoding.UTF8.GetBytes(cipherText);
            
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }
        
        
        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            LoadTextfile("dummy.txt");
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveTextfile();
        }
        
        private void OnEncryptButtonClicked(object sender, EventArgs e)
        {
            Text = Convert.ToBase64String(EncryptTextToBytes(Text, _myAes.Key, _myAes.IV));
//            DecryptText(cipherText, _myAes.Key, _myAes.IV);
        }

        private void OnDecryptButtonClicked(object sender, EventArgs e)
        {
            var encryptedBytes = Convert.FromBase64String(Text);
            Text = DecryptText(encryptedBytes, _myAes.Key, _myAes.IV);
        }
        
        private String AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}