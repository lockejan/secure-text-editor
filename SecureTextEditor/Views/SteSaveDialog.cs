using System;
using System.Collections.Generic;
using System.Linq;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;
using Medja;
using Medja.Properties;
using BcFactory;
using SecureTextEditor.FileHandler;

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
            _config = new CryptoConfig {PbePassword = "secret123".ToCharArray()};

            _firstColumnStack = GetStackPanel(140);
            _secColumnStack = GetStackPanel(150);
            _thirdColumnStack = GetStackPanel(160);

            //Filler CREATION
            _pbeFill = new Control();

            CreateLabels();
            CreateInputs();
            CreateAndBindComboBoxes();
            CreateCheckBoxes();
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
            
            var isEncryptActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsEncryptActive, (p, v) => p.IsEncryptActive = v);
            isEncryptActiveProperty.BindTo(_encryptionCheckBox.PropertyIsChecked);
            _encryptionCheckBox.InputState.Clicked += ToggleEncryptionSection;

            var isPbeActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsPbeActive, (p, v) => p.IsPbeActive = v);
            isPbeActiveProperty.BindTo(_pbeCheckBox.PropertyIsChecked);
            _pbeCheckBox.InputState.Clicked += TogglePbeSection;
            _pbeCheckBox.PropertyIsChecked.PropertyChanged += (s, e) => UpdateKeySizeComboBox();
            _pbeCheckBox.PropertyIsChecked.PropertyChanged += (s, e) => UpdateBlockComboBox();
            _pbeCheckBox.PropertyIsChecked.PropertyChanged += (s, e) => UpdatePaddingComboBox();

            var isIntegrityActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsIntegrityActive, (p, v) => p.IsIntegrityActive = v);
            isIntegrityActiveProperty.BindTo(_integrityCheckBox.PropertyIsChecked);
            _integrityCheckBox.InputState.Clicked += ToggleIntegritySection;
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
            void Rowspan(Control first, Control sec)
            {
                _firstColumnStack.Add(first);
                _secColumnStack.Add(sec);
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

        private void CreateAndBindComboBoxes()
        {
            ComboBox Init(string title, int width)
            {
                var comboBox = _controlFactory.Create<ComboBox>();
                comboBox.Title = title;
                comboBox.Position.Width = width;
                return comboBox;
            }

            var pbeAlgorithmProperty = new PropertyWrapper<CryptoConfig, PbeAlgorithm>(_config,
                p => p.PbeAlgorithm, (p, v) => p.PbeAlgorithm = v);
            
            _pbeSpecComboBox = Init("PBE", 100);
            _pbeSpecComboBox.BindEnum(pbeAlgorithmProperty);
            _pbeSpecComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateUsedDigestInfo();
            _pbeSpecComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateCipherAlgorithm();
            _pbeSpecComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateBlockComboBox();
            _pbeSpecComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateKeySizeComboBox();
            
            
            var cipherAlgorithmProperty = new PropertyWrapper<CryptoConfig, CipherAlgorithm>(_config,
                p => p.CipherAlgorithm, (p, v) => p.CipherAlgorithm = v);

            _cipherAlgorithmComboBox = Init("Cipher", 100);
            _cipherAlgorithmComboBox.BindEnum(cipherAlgorithmProperty);
            _cipherAlgorithmComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateKeySizeComboBox();
            _cipherAlgorithmComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateBlockComboBox();
            _cipherAlgorithmComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateUsedDigestInfo();
            UpdateUsedDigestInfo();

            _cipherKeyLengthComboBox = Init("Keysize", 100);
            _cipherKeyLengthComboBox.PropertySelectedItem.PropertyChanged += OnKeyLengthSelectedItemChanged;
            UpdateKeySizeComboBox();

            
            var blockModeProperty = new PropertyWrapper<CryptoConfig, BlockMode>(_config,
                p => p.BlockMode, (p, v) => p.BlockMode = v);
            _blockModeComboBox = Init("Blockmode", 130);
            _blockModeComboBox.BindEnum(blockModeProperty);
            _blockModeComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdatePaddingComboBox();


            var paddingProperty = new PropertyWrapper<CryptoConfig, Padding>(_config,
                p => p.Padding, (p, v) => p.Padding = v);
            _paddingComboBox = Init("Padding", 150);
            _paddingComboBox.BindEnum(paddingProperty);
            UpdateBlockComboBox();
            UpdatePaddingComboBox();

            
            var integrityProperty = new PropertyWrapper<CryptoConfig, Integrity>(_config,
                p => p.Integrity, (p, v) => p.Integrity = v);
            _integrityComboBox = Init("Integrity", 150);
            _integrityComboBox.BindEnum(integrityProperty);
            _integrityComboBox.PropertySelectedItem.PropertyChanged += (s, e) => UpdateIntegrityOptions();
            
            var integrityOptionsProperty = new PropertyWrapper<CryptoConfig, IntegrityOptions>(_config,
                p => p.IntegrityOptions, (p, v) => p.IntegrityOptions = v);
            _integritySpecComboBox = Init("Options", 100);
            _integritySpecComboBox.BindEnum(integrityOptionsProperty);
            UpdateIntegrityOptions();
        }

        private void UpdateCipherAlgorithm()
        {
            _cipherAlgorithmComboBox.Clear();

            _cipherAlgorithmComboBox.Add(CipherAlgorithm.AES.ToString());

            if (_config.PbeAlgorithm == PbeAlgorithm.PBKDF2)
                _cipherAlgorithmComboBox.Add(CipherAlgorithm.RC4.ToString());
            
            _cipherAlgorithmComboBox.SelectedItem = _cipherAlgorithmComboBox.ItemsPanel.Children[0];
        }

        private void UpdateBlockComboBox()
        {
            _blockModeComboBox.Clear();

            switch (_config.CipherAlgorithm)
            {
                case CipherAlgorithm.AES:
                    if (_config.IsPbeActive)
                    {
                        if (_config.PbeAlgorithm == PbeAlgorithm.PBKDF2)
                            _blockModeComboBox.Add(BlockMode.CBC.ToString());
                        if (_config.PbeAlgorithm == PbeAlgorithm.SCRYPT)
                            _blockModeComboBox.Add(BlockMode.GCM.ToString());
                    }
                    
                    if (!_config.IsPbeActive)
                    {
                        foreach (var value in Enum.GetValues(typeof(BlockMode))
                            .Cast<BlockMode>()
                            .Where(val => val!=BlockMode.None))
                        {
                            _blockModeComboBox.Add(value.ToString());
                        }
                    }
                    break;
                case CipherAlgorithm.RC4:
                    _blockModeComboBox.Add(BlockMode.None.ToString());
                    break;
            }
            _blockModeComboBox.SelectedItem = _blockModeComboBox.ItemsPanel.Children[0];
        }
        
        private void UpdatePaddingComboBox()
        {
            _paddingComboBox.Clear();
            
            switch (_config.BlockMode)
            {
              case BlockMode.ECB:
              case BlockMode.CBC:
                  _paddingComboBox.Add(BcFactory.Padding.Pkcs7.ToString());
                  if (!_config.IsPbeActive)
                    _paddingComboBox.Add(BcFactory.Padding.ZeroByte.ToString());
                  break;
              default:
                  _paddingComboBox.Add(BcFactory.Padding.None.ToString());
                  break;
            }
            _paddingComboBox.SelectedItem = _paddingComboBox.ItemsPanel.Children[0];
        }

        private void UpdateIntegrityOptions()
        {
            _integritySpecComboBox.Clear();
            var options = GetIntegrityOptions();
            
            foreach (var integrityOption in options)
            {
                _integritySpecComboBox.Add(integrityOption.ToString());
            }
            _integritySpecComboBox.SelectedItem = _integritySpecComboBox.ItemsPanel.Children[0];
        }

        private IEnumerable<IntegrityOptions> GetIntegrityOptions()
        {
                yield return IntegrityOptions.Sha256;

                if (_config.Integrity != Integrity.Digest) yield break;
                foreach (var value in Enum.GetValues(typeof(IntegrityOptions))
                    .Cast<IntegrityOptions>()
                    .Where(val => val!=IntegrityOptions.Sha256))
                {
                    yield return value;
                }
        }
        
        private void UpdateUsedDigestInfo()
        {
            String GetDigestInfo()
            {
                if (_config.PbeAlgorithm == PbeAlgorithm.PBKDF2)
                    return _config.CipherAlgorithm == CipherAlgorithm.AES ? PbeDigest.SHA256.ToString() : PbeDigest.SHA1.ToString();;
                return PbeDigest.GCM.ToString();;
            }
            _pbeDigestInfo.Text = GetDigestInfo();
        }

        private void UpdateKeySizeComboBox()
        {
            _cipherKeyLengthComboBox.Clear();
            
            var keySizes = GetKeySizes();
//            keySizes.Select(i => _cipherKeyLengthComboBox.Add($"{Convert.ToString(i)} bit"));
            foreach (var keySize in keySizes)
            {
                _cipherKeyLengthComboBox.Add($"{Convert.ToString(keySize)} bit");
            }
            _cipherKeyLengthComboBox.SelectedItem = _cipherKeyLengthComboBox.ItemsPanel.Children[0];
        }

        private int[] GetKeySizes()
        {
            if (_config.CipherAlgorithm == CipherAlgorithm.RC4)
                return KeySize.RC4;

            if (_config.IsPbeActive && _config.PbeAlgorithm == PbeAlgorithm.SCRYPT)
                return KeySize.AES.Where(x => x == 256).ToArray();

            if (_config.IsPbeActive && _config.PbeAlgorithm == PbeAlgorithm.PBKDF2 && _config.CipherAlgorithm == CipherAlgorithm.AES)
                return KeySize.AES.Where(x => x == 128).ToArray();

            return KeySize.AES;
        }

        private void OnKeyLengthSelectedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            _config.KeySize = ParseKeySize((ComboBoxMenuItem) e.NewValue);
        }

        private int ParseKeySize(ComboBoxMenuItem selectedItem)
        {
            var value = selectedItem?.Title;

            if (string.IsNullOrEmpty(value))
                return -1;

            value = value.Substring(0, value.IndexOf(" bit"));
            return int.Parse(value);
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
            //#######################################
            Console.WriteLine(_config.ToString());
            
            if (_config.IsEncryptActive)
            {
                if (_config.IsPbeActive)
                {
                    var pbecrypt = CryptoFactory.Create(_config);
                    // JUST FOR TESTING - CHANGE SIGNATURE TO ACCEPT CHAR[]   
                    var pbeKey = pbecrypt.EncryptTextToBytes(_config.PbePassword.ToString());
                    var tester = "Hallo Welt";
                    var crypt = CryptoFactory.Create(_config, pbeKey);
                    var encrypted = crypt.EncryptTextToBytes(tester);

                    Console.WriteLine($"Cipher:{Convert.ToBase64String(encrypted)}");
                    var decrypted = crypt.DecryptBytesToText(encrypted);
                    Console.WriteLine($"Text: {decrypted}");
                }
                else
                {
                    var tester = "Hallo Welt";
                    var crypt = CryptoFactory.Create(_config);
                    var encrypted = crypt.EncryptTextToBytes(tester);

                    Console.WriteLine($"Cipher:{Convert.ToBase64String(encrypted)}");
                    var decrypted = crypt.DecryptBytesToText(encrypted);
                    Console.WriteLine($"Text: {decrypted}");
                }
            }

            if (_config.IsIntegrityActive)
            {
                var sign = IntegrityFactory.Create(_config);
                var digest = Convert.ToBase64String(sign.SignBytes("Hallo Welt"));
                var result = sign.VerifySign(digest, "Hallo Welt");
                Console.WriteLine(result);
            }
            //#########################################
            
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