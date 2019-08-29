using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// SaveDialog view class to gather all options which then should be forwarded to the cipherEngines.
    /// Gets all menu possibilities from static dict in SteMenu Class.
    /// </summary>
    public class SteSaveDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private TextBlock _currentDir;
        private TextBox _filenameInput;
        
        private CheckBox _encryptionCheckBox;
        private CheckBox _pbeCheckBox;
        private CheckBox _integrityCheckBox;
        
        private TextBox _passwordInput;
        private ComboBox _pbeSpecComboBox;
        
        private ComboBox _cipherAlgorithmComboBox;
        private ComboBox _cipherKeyLengthComboBox;
        private ComboBox _blockModeComboBox;
        private ComboBox _paddingComboBox;
        
        private ComboBox _integrityComboBox;
        private ComboBox _integritySpecComboBox;
        
        private TextBlock _cipherAlgorithmLabel;
        private TextBlock _cipherKeyLengthLabel;
        private TextBlock _blockModeLabel;
        private TextBlock _paddingLabel;
        private TextBlock _passwordLabel;
        private TextBlock _pbeDigestLabel;
        private TextBlock _pbeDigestInfo;
        
        private VerticalStackPanel _firstColumnStack;
        private VerticalStackPanel _secColumnStack;
        private VerticalStackPanel _thirdColumnStack;
        
        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteSaveDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _firstColumnStack = GetStackPanel(140);
            _secColumnStack = GetStackPanel(150);
            _thirdColumnStack = GetStackPanel(160);

            CreateLabels();
            CreateInputs();
            CreateComboBoxes();
            CreateCheckBoxes();
            //InitStateOfCombos();
            RegisterEventHandler();
            FillStackPanels();
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void CreateCheckBoxes()
        {
            CheckBox Init(string title)
            {
                var checkBox = _controlFactory.Create<CheckBox>();
                checkBox.Text = title;
                checkBox.Padding.SetTopAndBottom(5);
                return checkBox;
            }

            _encryptionCheckBox = Init("Encryption ?");
            _pbeCheckBox = Init("PBE ?");
            _integrityCheckBox = Init("Integrity ?");
        }

        private void InitStateOfCombos()
        {
            _blockModeComboBox.Visibility = Visibility.Hidden;
            _blockModeLabel.Visibility = Visibility.Hidden;
            _paddingComboBox.IsEnabled = false;
            _passwordInput.IsEnabled = false;
        }

        private void CreateInputs()
        {
            TextBox CreateTextInput()
            {
                var textBox = _controlFactory.Create<TextBox>();
                return textBox;
            }
            _filenameInput = CreateTextInput();
            _passwordInput = CreateTextInput();
        }

        private Control CreateDockPanel()
        {
            var dockPanel = _controlFactory.Create<DockPanel>();
            dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            dockPanel.VerticalAlignment = VerticalAlignment.Stretch;
            dockPanel.Add(Dock.Left, _firstColumnStack);
            dockPanel.Add(Dock.Left, _secColumnStack);
            dockPanel.Add(Dock.Left, _thirdColumnStack);

            return dockPanel;
        }

        private VerticalStackPanel GetStackPanel(int width)
        {
            var vertStack = _controlFactory.Create<VerticalStackPanel>();
            vertStack.ChildrenHeight = 30;
            vertStack.Position.Width = width;
            //vertStack.Margin.SetAll(5);
            vertStack.Padding.SetAll(5);
            return vertStack;
        }

        private void FillStackPanels()
        {
            void FillRow(Control first, Control sec, Control third)
            {
                _firstColumnStack.Add(first ?? new Control());
                _secColumnStack.Add(sec ?? new Control());
                _thirdColumnStack.Add(third ?? new Control());
            }
            
            FillRow(GetLabel("Current DIR"), _currentDir,null);
            FillRow(GetLabel("Filename"), _filenameInput,null);
            //FillRow(GetLabel("Encryption"), _encryptionCheckBox, null);
            FillRow( _encryptionCheckBox, null,null);
            FillRow(_pbeCheckBox,null,null);
            FillRow(_passwordLabel, _passwordInput, null);
//            FillRow(null,_pbeSpecComboBox, null);
            FillRow(_pbeSpecComboBox, _pbeDigestLabel, _pbeDigestInfo);
//            FillRow(null,_pbeDigestLabel, _pbeDigestInfo);
//            FillRow(_cipherAlgorithmLabel, _cipherAlgorithmComboBox, _cipherKeyLengthComboBox);
            FillRow(null, _cipherAlgorithmComboBox, _cipherKeyLengthComboBox);
            FillRow(null, _blockModeComboBox, _paddingComboBox);
            //FillRow(_passwordLabel, _passwordInput);
            //FillRow(GetLabel("Integrity"), _integrityCheckBox,null);
            FillRow( _integrityCheckBox,null,null);
            FillRow( _integrityComboBox,_integritySpecComboBox,null);
//            FillRow(null,_integrityComboBox,null);
//            FillRow(null,_integritySpecComboBox,null);
            
            //vertStack.Background = _textBox.Background;
        }

        private TextBlock GetLabel(string text)
        {
            return _controlFactory.CreateTextBlock(text);
        }
        private void CreateLabels()
        {
            _currentDir = GetLabel(SteHelper.WorkingDirectory);
            _cipherKeyLengthLabel = GetLabel("Key length");
            _blockModeLabel = GetLabel("Blockmode");
            _paddingLabel = GetLabel("Padding");
            _passwordLabel = GetLabel("Password");
            _pbeDigestLabel = GetLabel("used digest");
            _pbeDigestInfo = GetLabel(null);
            _cipherAlgorithmLabel = GetLabel("Cipher");
    }

        private void CreateComboBoxes()
        {
            ComboBox Init(string title, int width)
            {
                var comboBox = _controlFactory.Create<ComboBox>();
                comboBox.Title = title;
                comboBox.Position.Width = width;
                return comboBox;
            }

            _pbeSpecComboBox = Init("PBE", 100);
            foreach (var member in Enum.GetNames(typeof(SteMenu.PBECipher)))
            {
                _pbeSpecComboBox.Add(member);
            }
            
            _cipherAlgorithmComboBox = Init("Cipher",100);
            foreach (var member in Enum.GetNames(typeof(SteMenu.Cipher)))
            {
                _cipherAlgorithmComboBox.Add(member);
            }
            
            _cipherKeyLengthComboBox = Init("Keysize",100);
            foreach (var member in SteMenu.KeySize)
            {
                _cipherKeyLengthComboBox.Add($"{Convert.ToString(member)} bit");
            }
            
            _blockModeComboBox = Init("Blockmode",130);
            foreach (var member in Enum.GetNames(typeof(SteMenu.BlockMode)))
            {
                _blockModeComboBox.Add(member);
            }
            
            _paddingComboBox = Init("Padding",150);
            foreach (var member in Enum.GetNames(typeof(SteMenu.Padding)))
            {
                _paddingComboBox.Add(member);
            }
            
            _integrityComboBox = Init("Integrity", 150);
            foreach (var option in Enum.GetNames(typeof(SteMenu.Integrity)))
            {
                _integrityComboBox.Add(option);
            }

            _integritySpecComboBox = Init("Options",100);
            foreach (var option in Enum.GetNames(typeof(SteMenu.DigestOptions)))
            {
                _integritySpecComboBox.Add(option);
            }
        }

        private void RegisterEventHandler()
        {
            _integrityComboBox.PropertySelectedItem.PropertyChanged += HandleIntegrity;
        }

        private void HandleIntegrity(object sender, EventArgs e)
        {
        /*    string[] digestOptions = SteMenu.IntegrityMenuTree["Digest"];
            var currentDigestItem = (_digestComboBox.SelectedItem as ComboBoxMenuItem)?.Title;
            
            if (currentDigestItem != null && digestOptions.Contains(currentDigestItem))
                _digestComboBox.SelectItem("None");
        */}
        
        public void ResetPassword()
        {
            _passwordInput = null;
        }
        
        private void UpdateCombos(){
                
//            foreach( KeyValuePair<string, string[]> kvp in Menu.CipherMenuTree["AES"])
//            {
//                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.GetValue(0));
//                _blockModeComboBox.Add(kvp.Key);
//            }

//            _cipherComboBox.Clear();
//            _keySizeComboBox.Clear();
//            _blockModeComboBox.Clear();
//            _paddingComboBox.Clear();
            //_integrityOptionsComboBox.Clear();
        }
    }

}