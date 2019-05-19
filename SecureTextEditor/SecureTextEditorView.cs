using System;
using System.IO;
using System.Reflection;
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
            
            CreateButtons(controlFactory);
            RegisterButtonEvents();
            
//            var buttonStackPanel = CreateButtonPanel(controlFactory);
//            CreateDockPanel(buttonStackPanel, controlFactory);
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
            buttonStackPanel.ChildrenWidth = 60;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Children.Add(_loadBtn);
            buttonStackPanel.Children.Add(_saveBtn);
            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.Right = 25;
            buttonStackPanel.Margin.SetTopAndBottom(5);

            return buttonStackPanel;
        }

        private void CreateButtons(IControlFactory controlFactory)
        {
            _loadBtn = controlFactory.Create<Button>();
            _loadBtn.Text = "Load";
            
            _saveBtn = controlFactory.Create<Button>();
            _saveBtn.Text = "Save";
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
        }


        private void LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                Text = File.ReadAllText(path);
                _path = AssemblyDirectory + "/../../../" + path;
            }
            FocusManager.Default.SetFocus(_textBox);
        }
        
        private void SaveTextfile()
        {
            EncryptText(Text);
            File.WriteAllText(_path, Text, Encoding.UTF8);
            FocusManager.Default.SetFocus(_textBox);
        }

        private void EncryptText(string text)
        {
//            CipherFactory.
            
//            byte[] keyBytes = Hex.Decode(Text);
////            Secre key = new SecretKeyPacket(keyBytes, "AES");
//            IDigest digest = new Sha256Digest();
//            byte[] result = new byte[digest.GetDigestSize()];
//            digest.Update((byte)(1 >> 24));
//            digest.Update((byte)(1 >> 16));
//            digest.Update((byte)(1 >> 8));
//            digest.Update((byte)1);
//            digest.BlockUpdate(z, 0, z.Length);
//            digest.BlockUpdate(otherInfo, 0, otherInfo.Length);
//            digest.DoFinal(result, 0);
        }

        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            LoadTextfile("dummy.txt");
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveTextfile();
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