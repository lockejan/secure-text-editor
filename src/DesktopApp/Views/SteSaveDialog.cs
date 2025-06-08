using System;
using CryptoEngine;
using SecureTextEditor.Views;

namespace DesktopApp.Views
{
    /// <summary>
    /// SaveDialog view class to gather all options which then should be forwarded to the cipherEngines.
    /// Gets all menu possibilities from static dict in SteMenu Class.
    /// </summary>
    public class SteSaveDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;
        private readonly CryptoConfig _config;
        /// <summary>
        /// Public interface to get CryptoConfig-Object.
        /// </summary>
        public CryptoConfig Config => _config;

        private TextBlock _currentDir;
        private TextBox _filenameInput;
        /// <summary>
        /// Public interface to get entered filename string.
        /// </summary>
        public TextBox Filename => _filenameInput;

        private CheckBox _encryptionCheckBox;
        private CheckBox _pbeCheckBox;
        private CheckBox _integrityCheckBox;

        private TextBox _passwordInput;
        /// <summary>
        /// Public interface to get provided password in saveDialog.
        /// </summary>
        public TextBox Password => _passwordInput;
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

        private readonly int _inputLength;

        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory">medja.UI object which is needed for control creation.</param>
        public SteSaveDialog(IControlFactory controlFactory, int inputLength)
        {
            _controlFactory = controlFactory;
            _inputLength = inputLength;
            
            _config = new CryptoConfig();

            _firstColumnStack = GetVertStackPanel(140);
            _secColumnStack = GetVertStackPanel(150);
            _thirdColumnStack = GetVertStackPanel(160);

            //Filler CREATION
            _pbeFill = new Control();

            CreateLabels();
            CreateInputs();
            CreateAndBindComboBoxes();
            CreateCheckBoxes();
            FillStackPanels();
            UpdateSectionVisibility(Sections.Encryption, Visibility.Visible);
            UpdateSectionVisibility(Sections.Pbe, Visibility.Collapsed);
            UpdateSectionVisibility(Sections.Integrity, Visibility.Collapsed);
            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void UpdateSectionVisibility(Sections section, Visibility visibility)
        {
            switch (section)
            {
                case Sections.Pbe:
                    for (int i = 2; i < 5; i++)
                    {
                        _firstColumnStack.Children[i].Visibility = visibility;
                        _secColumnStack.Children[i].Visibility = visibility;
                        _thirdColumnStack.Children[i].Visibility = visibility;
                    }
                    break;
                case Sections.Encryption:
                    for (int i = 5; i < 8; i++)
                    {
                        _firstColumnStack.Children[i].Visibility = visibility;
                        _secColumnStack.Children[i].Visibility = visibility;
                        _thirdColumnStack.Children[i].Visibility = visibility;
                    }
                    break;
                case Sections.Integrity:
                    for (int i = 9; i < 11; i++)
                    {
                        _firstColumnStack.Children[i].Visibility = visibility;
                        _secColumnStack.Children[i].Visibility = visibility;
                        _thirdColumnStack.Children[i].Visibility = visibility;
                    }
                    break;
            }
            // enforce layout update
            IsLayoutUpdated = false;
            FocusManager.Default.SetFocus(_filenameInput);
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
            _encryptionCheckBox.IsChecked = true;
            _pbeCheckBox = Init("PBE ?");
            _integrityCheckBox = Init("Integrity ?");

            var isEncryptActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsEncryptActive, (p, v) => p.IsEncryptActive = v);
            isEncryptActiveProperty.BindTo(_encryptionCheckBox.PropertyIsChecked);
            _encryptionCheckBox.InputState.Clicked += ToggleEncryptionSection;

            var isPbeActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsPbeActive, (p, v) => p.IsPbeActive = v);
            isPbeActiveProperty.BindTo(_pbeCheckBox.PropertyIsChecked);
            _pbeCheckBox.InputState.Clicked += TogglePbeSection;
            _pbeCheckBox.PropertyIsChecked.PropertyChanged += OnPbeCheckBoxIsCheckedChanged;

            var isIntegrityActiveProperty = new PropertyWrapper<CryptoConfig, bool>(_config,
                p => p.IsIntegrityActive, (p, v) => p.IsIntegrityActive = v);
            isIntegrityActiveProperty.BindTo(_integrityCheckBox.PropertyIsChecked);
            _integrityCheckBox.InputState.Clicked += ToggleIntegritySection;
        }

        private void OnPbeCheckBoxIsCheckedChanged(object sender, EventArgs e)
        {
            UpdateKeySizeComboBox();
            UpdateBlockComboBox();
            UpdatePaddingComboBox();
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

            var horiStackPanel = GetHoriStackPanel(80);
            var labelCol = GetVertStackPanel(140);
            var contentCol = GetVertStackPanel(410);

            labelCol.Add(GetLabel("Current DIR"));
            labelCol.Add(GetLabel("Filename"));
            contentCol.Add(_currentDir);
            contentCol.Add(_filenameInput);

            horiStackPanel.Add(labelCol);
            horiStackPanel.Add(contentCol);

            dockPanel.Add(Dock.Top, horiStackPanel);
            dockPanel.Add(Dock.Left, _firstColumnStack);
            dockPanel.Add(Dock.Left, _secColumnStack);
            dockPanel.Add(Dock.Left, _thirdColumnStack);

            return dockPanel;
        }
        private HorizontalStackPanel GetHoriStackPanel(int height)
        {
            var horiStack = _controlFactory.Create<HorizontalStackPanel>();
            horiStack.Position.Height = height;
            return horiStack;
        }

        private VerticalStackPanel GetVertStackPanel(int width)
        {
            var vertStack = _controlFactory.Create<VerticalStackPanel>();
            vertStack.ChildrenHeight = 30;
            vertStack.Position.Width = width;
            vertStack.Padding.SetAll(5);
            return vertStack;
        }

        private void FillStackPanels()
        {
            void FillColumn(Control first, Control sec, Control third)
            {
                var customFill = new Control();
                customFill.Margin.SetTopAndBottom(5);

                _firstColumnStack.Add(first ?? new Control());
                _secColumnStack.Add(sec ?? customFill);
                _thirdColumnStack.Add(third ?? customFill);
            }

            FillColumn(_encryptionCheckBox, null, null);
            FillColumn(_pbeCheckBox, null, null);
            FillColumn(_passwordLabel, _passwordInput, _pbeFill);
            FillColumn(_pbeSpecLabel, _pbeSpecComboBox, new Control());
            FillColumn(_pbeDigestLabel, _pbeDigestInfo, new Control());
            FillColumn(_cipherAlgorithmLabel, _cipherAlgorithmComboBox, _cipherKeyLengthComboBox);
            FillColumn(_blockModeLabel, _blockModeComboBox, new Control());
            FillColumn(_paddingLabel, _paddingComboBox, new Control());
            FillColumn(_integrityCheckBox, null, null);
            FillColumn(_integrityLabel, _integrityComboBox, new Control());
            FillColumn(_integritySpecLabel, _integritySpecComboBox, new Control());
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
            _cipherAlgorithmComboBox.AddRange(_config.GetValidAlgorithms());
            _cipherAlgorithmComboBox.SelectFirstItem();
        }

        private void UpdateBlockComboBox()
        {
            _blockModeComboBox.Clear();
            _blockModeComboBox.AddRange(_inputLength > 15
                ? _config.GetValidBlockModes()
                : _config.GetValidBlockModes(BlockMode.CTS));
            _blockModeComboBox.SelectFirstItem();
        }

        private void UpdatePaddingComboBox()
        {
            _paddingComboBox.Clear();
            _paddingComboBox.AddRange(_config.GetValidPaddings());
            _paddingComboBox.SelectFirstItem();
        }

        private void UpdateIntegrityOptions()
        {
            _integritySpecComboBox.Clear();
            _integritySpecComboBox.AddRange(_config.GetIntegrityOptions());
            _integritySpecComboBox.SelectFirstItem();
        }

        private void UpdateUsedDigestInfo()
        {
            _pbeDigestInfo.Text = _config.GetDigest().ToString();
        }

        private void UpdateKeySizeComboBox()
        {
            _cipherKeyLengthComboBox.Clear();

            var keySizes = _config.GetKeySizes();

            _cipherKeyLengthComboBox.AddRange(keySizes.Select(keySize => $"{Convert.ToString(keySize)} bit"));
            _cipherKeyLengthComboBox.SelectFirstItem();
        }

        private void OnKeyLengthSelectedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            _config.KeySize = ParseKeySize((ComboBoxMenuItem)e.NewValue);
        }

        private int ParseKeySize(ComboBoxMenuItem selectedItem)
        {
            var value = selectedItem?.Title;

            if (string.IsNullOrEmpty(value))
                return -1;

            value = value.Substring(0, value.IndexOf(" bit", StringComparison.Ordinal));
            return int.Parse(value);
        }

        private void ToggleEncryptionSection(object sender, EventArgs e)
        {
            if (_encryptionCheckBox.IsChecked)
            {
                UpdateSectionVisibility(Sections.Encryption, Visibility.Visible);
                _pbeCheckBox.IsEnabled = true;
                return;
            }

            UpdateSectionVisibility(Sections.Encryption, Visibility.Collapsed);
            UpdateSectionVisibility(Sections.Pbe, Visibility.Collapsed);
            _pbeCheckBox.IsChecked = false;
            _pbeCheckBox.IsEnabled = false;
        }

        private void TogglePbeSection(object sender, EventArgs e)
        {
            if (_pbeCheckBox.IsChecked)
            {
                UpdateSectionVisibility(Sections.Pbe, Visibility.Visible);
                return;
            }
            UpdateSectionVisibility(Sections.Pbe, Visibility.Collapsed);
        }

        private void ToggleIntegritySection(object sender, EventArgs e)
        {
            if (_integrityCheckBox.IsChecked)
            {
                UpdateSectionVisibility(Sections.Integrity, Visibility.Visible);
                return;
            }
            UpdateSectionVisibility(Sections.Integrity, Visibility.Collapsed);
        }
        
    }

}