using System;
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

        private Control _encryptFillOne;
        private Control _encryptFillTwo;
        private Control _pbeFill;
        
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
            
            //Filler CREATION
            _pbeFill = new Control();
            _encryptFillOne = new Control();
            _encryptFillTwo = new Control();

            CreateLabels();
            CreateInputs();
            CreateComboBoxes();
            CreateCheckBoxes();
            //InitStateOfCombos();
            RegisterEventHandler();
            FillStackPanels();
            CollapseSections(0);
            CollapseSections(2);
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void CollapseSections(int section)
        {
            // ENC BASIC
            if (section == 0)
            {
                _cipherAlgorithmComboBox.Visibility = Visibility.Collapsed;
                _cipherKeyLengthComboBox.Visibility = Visibility.Collapsed;
                _blockModeComboBox.Visibility = Visibility.Collapsed;
                _paddingComboBox.Visibility = Visibility.Collapsed;
                _cipherAlgorithmLabel.Visibility = Visibility.Collapsed;
                _cipherKeyLengthLabel.Visibility = Visibility.Collapsed;
                _blockModeLabel.Visibility = Visibility.Collapsed;
                _paddingLabel.Visibility = Visibility.Collapsed;
                _encryptFillOne.Visibility = Visibility.Collapsed;
                _encryptFillTwo.Visibility = Visibility.Collapsed;
            }
            
            // PBE
            if (section == 0 || section == 1)
            {
//                _pbeFill.Visibility = Visibility.Collapsed;
//                _passwordInput.Visibility = Visibility.Collapsed;
//                _pbeSpecComboBox.Visibility = Visibility.Collapsed;
//                _passwordLabel.Visibility = Visibility.Collapsed;
//                _pbeDigestLabel.Visibility = Visibility.Collapsed;
//                _pbeDigestInfo.Visibility = Visibility.Collapsed;
                _firstColumnStack.Children[4].Visibility = Visibility.Collapsed;
                _firstColumnStack.Children[5].Visibility = Visibility.Collapsed;
                _secColumnStack.Children[4].Visibility = Visibility.Collapsed;
                _secColumnStack.Children[5].Visibility = Visibility.Collapsed;
                _thirdColumnStack.Children[4].Visibility = Visibility.Collapsed;
                _thirdColumnStack.Children[5].Visibility = Visibility.Collapsed;
                
            }
            
            // DIGEST
            if (section == 2)
            {
                _integrityComboBox.Visibility = Visibility.Collapsed;
                _integritySpecComboBox.Visibility = Visibility.Collapsed;
            }
        
        }
        private void ViewSections(int section)
        {
            // ENC BASIC
            if (section == 0)
            {
                _cipherAlgorithmComboBox.Visibility = Visibility.Visible;
                _cipherKeyLengthComboBox.Visibility = Visibility.Visible;
                _blockModeComboBox.Visibility = Visibility.Visible;
                _paddingComboBox.Visibility = Visibility.Visible;
                _cipherAlgorithmLabel.Visibility = Visibility.Visible;
                _cipherKeyLengthLabel.Visibility = Visibility.Visible;
                _blockModeLabel.Visibility = Visibility.Visible;
                _paddingLabel.Visibility = Visibility.Visible;
                _encryptFillOne.Visibility = Visibility.Visible;
                _encryptFillTwo.Visibility = Visibility.Visible;
            }
            
            // PBE
            if (section == 0 || section == 1)
            {
                _pbeFill.Visibility = Visibility.Visible;
                _passwordInput.Visibility = Visibility.Visible;
                _pbeSpecComboBox.Visibility = Visibility.Visible;
                _passwordLabel.Visibility = Visibility.Visible;
                _pbeDigestLabel.Visibility = Visibility.Visible;
                _pbeDigestInfo.Visibility = Visibility.Visible;
//                _firstColumnStack.Children[4].Visibility = Visibility.Visible;
//                _firstColumnStack.Children[5].Visibility = Visibility.Visible;
//                _secColumnStack.Children[4].Visibility = Visibility.Visible;
//                _secColumnStack.Children[5].Visibility = Visibility.Visible;
//                _thirdColumnStack.Children[4].Visibility = Visibility.Visible;
//                _thirdColumnStack.Children[5].Visibility = Visibility.Visible;
                
            }
            
            // DIGEST
            if (section == 2)
            {
                _integrityComboBox.Visibility = Visibility.Visible;
                _integritySpecComboBox.Visibility = Visibility.Visible;
            }
        
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
            
            FillRow(GetLabel("Current DIR"), _currentDir,new Control());
            FillRow(GetLabel("Filename"), _filenameInput,new Control());
            FillRow(_encryptionCheckBox, null,null);
            FillRow(_pbeCheckBox,null,null);
            FillRow(_passwordLabel, _passwordInput, _pbeFill);
            FillRow(_pbeSpecComboBox, _pbeDigestLabel, _pbeDigestInfo);
            FillRow(_encryptFillOne, _cipherAlgorithmComboBox, _cipherKeyLengthComboBox);
            FillRow(_encryptFillTwo, _blockModeComboBox, _paddingComboBox);
            FillRow(_integrityCheckBox,null,null);
            FillRow(_integrityComboBox,_integritySpecComboBox,null);
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
            _encryptionCheckBox.InputState.Clicked += ToggleEncryptionSection;
            _pbeCheckBox.InputState.Clicked += TogglePbeSection;
            _integrityCheckBox.InputState.Clicked += ToggleIntegritySection;
            
            _integrityComboBox.PropertySelectedItem.PropertyChanged += HandleIntegrity;
        }


        private void ToggleEncryptionSection(object sender, EventArgs e)
        {
            if (_encryptionCheckBox.IsChecked)
            {
                ViewSections(0);
                return;
            }
            CollapseSections(0);
        }

        private void TogglePbeSection(object sender, EventArgs e)
        {
            if (_pbeCheckBox.IsChecked)
            {
                ViewSections(1);
                return;
            }
            CollapseSections(1);
            
        }
        
        private void ToggleIntegritySection(object sender, EventArgs e)
        {
            if (_integrityCheckBox.IsChecked)
            {
                ViewSections(2);
                return;
            }
            CollapseSections(2);
            
        }
        
        private void HandleIntegrity(object sender, EventArgs e)
        {

        }
   
        public void ResetPassword()
        {
            _passwordInput = null;
        }
        
        private void UpdateCombos()
        {
            
        }
    }

}