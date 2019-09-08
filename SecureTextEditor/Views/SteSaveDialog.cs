using System;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;
using Medja;
using Medja.Properties;
using CryptoAdapter;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// SaveDialog view class to gather all options which then should be forwarded to the cipherEngines.
    /// Gets all menu possibilities from static dict in SteMenu Class.
    /// </summary>
    public class SteSaveDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;
        private readonly CryptoConfig _config;

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
        private TextBlock _blockModeLabel;
        private TextBlock _paddingLabel;
        private TextBlock _passwordLabel;
        private TextBlock _pbeDigestLabel;
        private TextBlock _pbeDigestInfo;

        private readonly Control _pbeFill;
        
        // private int? bla;
        // if(bla.HasValue) var value = bla.Value;
        // if(bla == null)...
        // var notNullValue = value ?? 0;
        // 
        // void Set(object value) { _localValue = value ??  throw new ArgumentNullException(nameof(value)); }

        private readonly VerticalStackPanel _firstColumnStack;
        private readonly VerticalStackPanel _secColumnStack;
        private readonly VerticalStackPanel _thirdColumnStack;
        private TextBlock _pbeSpecLabel;
        private TextBlock _integrityLabel;
        private TextBlock _integritySpecLabel;

        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteSaveDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _config = new CryptoConfig();
            _firstColumnStack = GetStackPanel(140);
            _secColumnStack = GetStackPanel(150);
            _thirdColumnStack = GetStackPanel(160);

            //Filler CREATION
            _pbeFill = new Control();

            CreateLabels();
            CreateInputs();
            CreateComboBoxes();
            CreateCheckBoxes();
            RegisterEventHandler();
            FillStackPanels();
            UpdateSectionVisibility(0, Visibility.Collapsed);
            UpdateSectionVisibility(1, Visibility.Collapsed);
            UpdateSectionVisibility(2, Visibility.Collapsed);
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void UpdateSectionVisibility(int section, Visibility visibility)
        {
            switch (section)
            {
            case 0:
                _firstColumnStack.Children[7].Visibility = visibility;
                _secColumnStack.Children[7].Visibility = visibility;
                _thirdColumnStack.Children[7].Visibility = visibility;
                _firstColumnStack.Children[8].Visibility = visibility;
                _secColumnStack.Children[8].Visibility = visibility;
                _thirdColumnStack.Children[8].Visibility = visibility;
                _firstColumnStack.Children[9].Visibility = visibility;
                _secColumnStack.Children[9].Visibility = visibility;
                _thirdColumnStack.Children[9].Visibility = visibility;
                break;
            case 1:
                _firstColumnStack.Children[4].Visibility = visibility;
                _secColumnStack.Children[4].Visibility = visibility;
                _thirdColumnStack.Children[4].Visibility = visibility;
                _firstColumnStack.Children[5].Visibility = visibility;
                _secColumnStack.Children[5].Visibility = visibility;
                _thirdColumnStack.Children[5].Visibility = visibility;
                _firstColumnStack.Children[6].Visibility = visibility;
                _secColumnStack.Children[6].Visibility = visibility;
                _thirdColumnStack.Children[6].Visibility = visibility;
                break;
            case 2:
                _firstColumnStack.Children[11].Visibility = visibility;
                _secColumnStack.Children[11].Visibility = visibility;
                _thirdColumnStack.Children[11].Visibility = visibility;
                _firstColumnStack.Children[12].Visibility = visibility;
                _secColumnStack.Children[12].Visibility = visibility;
                _thirdColumnStack.Children[12].Visibility = visibility;
                break;
            }
            // enforce layout update
            IsLayoutUpdated = false;
        }

        private void CreateCheckBoxes()
        {
            CheckBox Init(string title)
            {
                var checkBox = _controlFactory.Create<CheckBox>();
                checkBox.Text = title;
                checkBox.Margin.SetTopAndBottom(5);
                return checkBox;
            }

            _encryptionCheckBox = Init("Encryption ?");
            _pbeCheckBox = Init("PBE ?");
            _pbeCheckBox.IsEnabled = false;
            _integrityCheckBox = Init("Integrity ?");
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
            vertStack.Padding.SetAll(5);
            return vertStack;
        }

        private void FillStackPanels()
        {
            void FillRow(Control first, Control sec, Control third)
            {
                var customFill = new Control();
                customFill.Margin.SetTopAndBottom(5);

                _firstColumnStack.Add(first ?? new Control());
                _secColumnStack.Add(sec ?? customFill);
                _thirdColumnStack.Add(third ?? customFill);
            }

            FillRow(GetLabel("Current DIR"), _currentDir, new Control());
            FillRow(GetLabel("Filename"), _filenameInput, new Control());
            FillRow(_encryptionCheckBox, null, null);
            FillRow(_pbeCheckBox, null, null);
            FillRow(_passwordLabel, _passwordInput, _pbeFill);
            FillRow(_pbeSpecLabel, _pbeSpecComboBox, new Control());
            FillRow(_pbeDigestLabel, _pbeDigestInfo, new Control());
            FillRow(_cipherAlgorithmLabel, _cipherAlgorithmComboBox, _cipherKeyLengthComboBox);
            FillRow(_blockModeLabel, _blockModeComboBox,new Control());
            FillRow(_paddingLabel, _paddingComboBox, new Control());
            FillRow(_integrityCheckBox, null, null);
            FillRow(_integrityLabel, _integrityComboBox, new Control());
            FillRow(_integritySpecLabel, _integritySpecComboBox, new Control());
        }

        private TextBlock GetLabel(string text)
        {
            var label = _controlFactory.CreateTextBlock(text);
            label.IsEnabled = false;
            return label;
        }

        private void CreateLabels()
        {
            _currentDir = GetLabel(SteHelper.WorkingDirectory);
            _cipherAlgorithmLabel = GetLabel("Cipher");
            _blockModeLabel = GetLabel("Blockmode");
            _paddingLabel = GetLabel("Padding");
            _passwordLabel = GetLabel("Password");
            _pbeSpecLabel = GetLabel("PBE Type");
            _pbeDigestLabel = GetLabel("used digest");
            _pbeDigestInfo = GetLabel(null);
            _integrityLabel = GetLabel("Type");
            _integritySpecLabel = GetLabel("Config");
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

            var cipherAlgorithmProperty = new PropertyWrapper<CryptoConfig, CipherAlgorithm>(_config,
                p => p.Algorithm, (p, v) => p.Algorithm = v);

            _cipherAlgorithmComboBox = Init("Cipher", 100);
            _cipherAlgorithmComboBox.BindEnum(cipherAlgorithmProperty);
            /*foreach (var member in Enum.GetNames(typeof(SteMenu.Cipher)))
            {
                _cipherAlgorithmComboBox.Add(member);
            }*/
            
//            var cipherKeySizeProperty = new PropertyWrapper<CryptoConfig, KeySize>(_config,
//                p => p.KeySize, (p, v) => p.KeySize = v);

            _cipherKeyLengthComboBox = Init("Keysize", 100);
//            _cipherKeyLengthComboBox.BindEnum(cipherKeySizeProperty);
            foreach (var member in SteMenu.KeySize)
            {
                _cipherKeyLengthComboBox.Add($"{Convert.ToString(member)} bit");
            }

            
            var blockModeProperty = new PropertyWrapper<CryptoConfig, BlockMode>(_config,
                p => p.BlockMode, (p, v) => p.BlockMode = v);

            _blockModeComboBox = Init("Blockmode", 130);
            _blockModeComboBox.BindEnum(blockModeProperty);
//            foreach (var member in Enum.GetNames(typeof(SteMenu.BlockMode)))
//            {
//                _blockModeComboBox.Add(member);
//            }

            var paddingProperty = new PropertyWrapper<CryptoConfig, Padding>(_config,
                p => p.Padding, (p, v) => p.Padding = v);

            _paddingComboBox = Init("Padding", 150);
            _paddingComboBox.BindEnum(paddingProperty);
//            foreach (var member in Enum.GetNames(typeof(SteMenu.Padding)))
//            {
//                _paddingComboBox.Add(member);
//            }

            _integrityComboBox = Init("Integrity", 150);
            foreach (var option in Enum.GetNames(typeof(SteMenu.Integrity)))
            {
                _integrityComboBox.Add(option);
            }

            _integritySpecComboBox = Init("Options", 100);
            foreach (var option in Enum.GetNames(typeof(SteMenu.DigestOptions)))
            {
                _integritySpecComboBox.Add(option);
            }
        }

        private void RegisterEventHandler()
        {
            _encryptionCheckBox.InputState.Clicked += ToggleEncryptionSection;
            _pbeCheckBox.InputState.Clicked += TogglePbeSection;
            _integrityCheckBox.InputState.Clicked += ToggleIntegritySection;
        }


        private void ToggleEncryptionSection(object sender, EventArgs e)
        {
            if (_encryptionCheckBox.IsChecked)
            {
                UpdateSectionVisibility(0, Visibility.Visible);
                _pbeCheckBox.IsEnabled = true;
                return;
            }

            UpdateSectionVisibility(0, Visibility.Collapsed);
            UpdateSectionVisibility(1, Visibility.Collapsed);
            _pbeCheckBox.IsChecked = false;
            _pbeCheckBox.IsEnabled = false;
        }

        private void TogglePbeSection(object sender, EventArgs e)
        {
            if (_pbeCheckBox.IsChecked)
            {
                UpdateSectionVisibility(1, Visibility.Visible);
                return;
            }

            UpdateSectionVisibility(1, Visibility.Collapsed);
        }

        private void ToggleIntegritySection(object sender, EventArgs e)
        {
            Console.WriteLine(_config.ToString());
            if (_integrityCheckBox.IsChecked)
            {
                UpdateSectionVisibility(2, Visibility.Visible);
                return;
            }

            UpdateSectionVisibility(2, Visibility.Collapsed);
        }

        public void ResetPassword()
        {
            _passwordInput = null;
        }
        
    }

}