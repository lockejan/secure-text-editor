using System;
using System.IO;
using System.Reflection;
using System.Text;
using Medja.Controls;
using Medja.Theming;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace SecureTextEditor
{
    public class SecureTextEditorView : ContentControl
    {
        private string _path = "dummy.txt";
        private readonly IControlFactory _controlFactory;

        private Button _loadBtn;
        private Button _saveBtn;
        private Button _cryptBtn;

        private ComboBox _cipherModeComboBox;
        private ComboBox _paddingModeComboBox;
        private ComboBox _blockModeComboBox;
        private ComboBox _macModeComboBox;

        private bool _cryptor = false;
        private SecureTextEditorModel _cryptoFabric;

        private TextEditor _textBox;
        public string Text
        {
            get { return _textBox.GetText(); }
            set { _textBox.SetText(value); }
        }

        /// <summary>
        /// Creates mainView Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SecureTextEditorView(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _textBox = controlFactory.Create<TextEditor>();

            _cryptoFabric = new SecureTextEditorModel();

            CreateButtons(controlFactory);
            RegisterButtonEvents();
            
            CreateComboBoxes(controlFactory);
            
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_textBox);
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
            buttonStackPanel.ChildrenWidth = 100;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Add(_loadBtn);
            buttonStackPanel.Add(_saveBtn);
            buttonStackPanel.Add(_cryptBtn);
            buttonStackPanel.Add(_cipherModeComboBox);
//            buttonStackPanel.ChildrenWidth(_cipherModeComboBox, 90);
            buttonStackPanel.Add(_blockModeComboBox);
            buttonStackPanel.Add(_paddingModeComboBox);
            buttonStackPanel.Add(_macModeComboBox);
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
            
            _cryptBtn = controlFactory.Create<Button>();
            _cryptBtn.Text = "Crypt";
        }

        private void CreateComboBoxes(IControlFactory controlFactory)
        {
            _cipherModeComboBox = controlFactory.Create<ComboBox>();
            _cipherModeComboBox.Title = "Cipher";
            _cipherModeComboBox.Add("AES-128");
            _cipherModeComboBox.Add("AES-192");
            _cipherModeComboBox.Add("AES-256");            
            
            _blockModeComboBox = controlFactory.Create<ComboBox>();
            _blockModeComboBox.Title = "Blockmode";
            _blockModeComboBox.Add("ECB");
            _blockModeComboBox.Add("CBC");
            _blockModeComboBox.Add("OFB");
            _blockModeComboBox.Add("CTS");
            _blockModeComboBox.Add("GCM");
            
            _paddingModeComboBox = controlFactory.Create<ComboBox>();
            _paddingModeComboBox.Title = "Padding";
            _paddingModeComboBox.Add("None");
            _paddingModeComboBox.Add("PKCS7");
            _paddingModeComboBox.Add("ZeroByte");
            
            _macModeComboBox = controlFactory.Create<ComboBox>();
            _macModeComboBox.Title = "AUTH";
            _macModeComboBox.Add("None");
            _macModeComboBox.Add("SHA-256");
            _macModeComboBox.Add("AESCMAC");
            _macModeComboBox.Add("HMACSHA256");
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
            _cryptBtn.InputState.Clicked += OnCryptButtonClicked;
        }
        
        private void LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                Text = File.ReadAllText(path,Encoding.UTF8);
                var cryptoData = File.ReadAllText("dummy.crypto", Encoding.UTF8);
                _cryptoFabric = JsonConvert.DeserializeObject<SecureTextEditorModel>(cryptoData);
                Console.WriteLine(_cryptoFabric);
                
                _path = AssemblyDirectory + "/../../../" + path;
            }
            FocusManager.Default.SetFocus(_textBox);
        }
        
        private void SaveTextfile()
        {
            var tmp = _cryptoFabric.EncryptTextToBytes(Text, _cryptoFabric.AES.Key, _cryptoFabric.AES.IV);
            
//            Console.WriteLine(JsonConvert.SerializeObject(_cryptoFabric));
            File.WriteAllText("./dummy.crypto",JsonConvert.SerializeObject(_cryptoFabric), Encoding.UTF8);
            
            File.WriteAllText(_path, Convert.ToBase64String(tmp), Encoding.UTF8);
            FocusManager.Default.SetFocus(_textBox);
        }

        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            LoadTextfile("dummy.txt");
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveTextfile();
        }
        
        private void OnCryptButtonClicked(object sender, EventArgs e)
        {
            if (_cryptor == false)
            {
                Text = Convert.ToBase64String(_cryptoFabric.EncryptTextToBytes(Text, _cryptoFabric.AES.Key, _cryptoFabric.AES.IV));
                _cryptor = true;
            }
            else
            {
                Text = _cryptoFabric.DecryptText(Convert.FromBase64String(Text), _cryptoFabric.AES.Key, _cryptoFabric.AES.IV);
                _cryptor = false;
            }

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