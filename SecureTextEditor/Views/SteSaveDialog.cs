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

        private ComboBox _cipherModeComboBox;
        private ComboBox _cipherAlgorithmComboBox;
        private ComboBox _blockModeComboBox;
        private ComboBox _paddingComboBox;
        private TextBox _passwordInput;
        
        private TextBlock _cipherModeLabel;
        private TextBlock _cipherAlgorithmLabel;
        private TextBlock _blockModeLabel;
        private TextBlock _paddingLabel;
        private TextBlock _passwordLabel;

        private TextBlock _digestLabel;
        private TextBlock _signingLabel;
        private ComboBox _digestComboBox;
        private ComboBox _signingComboBox;

        private TextBlock _currentDirLabel;
        private readonly TextBlock _currentDir;
        private TextBlock _filenameInputLabel;
        private TextBox _filenameInput;

        private Button _confirmButton;
        private Button _cancelButton;
        
        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteSaveDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _currentDir = CreateTextBlock(SteHelper.WorkingDirectory);

            CreateButtons();
            CreateLabels();
            CreatInputs();
            CreateComboBoxes();
            InitStateOfCombos();
            RegisterEventHandler();

            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void InitStateOfCombos()
        {
            _blockModeComboBox.IsEnabled = false;
            _paddingComboBox.IsEnabled = false;
            _passwordInput.IsEnabled = false;
        }

        private void CreatInputs()
        {
            _filenameInput = CreateInput();
            _passwordInput = CreateInput();
        }

        private TextBox CreateInput()
        {
            var textBox = _controlFactory.Create<TextBox>();
            return textBox;
        }

        private Control CreateDockPanel()
        {
            var dockPanel = _controlFactory.Create<DockPanel>();
            dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            dockPanel.VerticalAlignment = VerticalAlignment.Stretch;
            dockPanel.Add(Dock.Fill, CreateTablePanel());

            return dockPanel;
        }

        private TablePanel CreateTablePanel()
        {
            var tablePanel = _controlFactory.Create<TablePanel>();
            const float rowHeight = 30;

            tablePanel.Columns.Add(new ColumnDefinition(150));
            tablePanel.Columns.Add(new ColumnDefinition(470));
            for (int i = 0; i < 11; i++)
            {
                tablePanel.Rows.Add(new RowDefinition(rowHeight));
            }

            return FillTablePanel(tablePanel);
        }

        private TablePanel FillTablePanel(TablePanel grid)
        {
            grid.Children.Add(_cipherModeLabel);
            grid.Children.Add(_cipherModeComboBox);
            grid.Children.Add(_cipherAlgorithmLabel);
            grid.Children.Add(_cipherAlgorithmComboBox);
            grid.Children.Add(_blockModeLabel);
            grid.Children.Add(_blockModeComboBox);
            grid.Children.Add(_paddingLabel);
            grid.Children.Add(_paddingComboBox);
            grid.Children.Add(_passwordLabel);
            grid.Children.Add(_passwordInput);
            grid.Children.Add(_digestLabel);
            grid.Children.Add(_digestComboBox);
            grid.Children.Add(_signingLabel);
            grid.Children.Add(_signingComboBox);
            grid.Children.Add(_currentDirLabel);
            grid.Children.Add(_currentDir);
            grid.Children.Add(_filenameInputLabel);
            grid.Children.Add(_filenameInput);
            grid.Children.Add(_confirmButton);
            grid.Children.Add(_cancelButton);

            return grid;
        }

        private TextBlock CreateTextBlock(String text)
        {
            var textBlock = _controlFactory.CreateTextBlock(text);
//            textBlock.Text = text;
//            textBlock.Renderer = new TextBlockRenderer(textBlock);
            return textBlock;
        }

        private void CreateLabels()
        {
            _blockModeLabel = CreateTextBlock("Blockmode:");
            _cipherModeLabel = CreateTextBlock("Ciphermode:");
            _cipherAlgorithmLabel = CreateTextBlock("Cipher:");
            _currentDirLabel = CreateTextBlock("Current DIR:");
            _digestLabel = CreateTextBlock("Digest:");
            _filenameInputLabel = CreateTextBlock("Filename:");
            _paddingLabel = CreateTextBlock("Padding:");
            _passwordLabel = CreateTextBlock("Password:");
            _signingLabel = CreateTextBlock("Digital Sign:");
        }

        private void CreateButtons()
        {
            _confirmButton = _controlFactory.Create<Button>();
            _confirmButton.Position.Width = 100;
            _confirmButton.Text = "Confirm";

            _cancelButton = _controlFactory.Create<Button>();
            _cancelButton.Position.Width = 100;
            _cancelButton.Text = "Cancel";
        }

        private void CreateComboBoxes()
        {
            _cipherModeComboBox = _controlFactory.Create<ComboBox>();
            _cipherModeComboBox.Title = "CipherMode";
/*            foreach (var mode in SteMenu.CipherMode)
            {
                _cipherModeComboBox.Add(mode);
            }*/

            _cipherAlgorithmComboBox = _controlFactory.Create<ComboBox>();
            _cipherAlgorithmComboBox.Title = "Cipher";
            //cipherMode : BlockCipher | StreamCipher | PBE
            _cipherAlgorithmComboBox.Position.Width = 200;

            foreach (var option in SteMenu.CipherMenuTree)
            {
                foreach (var keyLength in option.Value)
                {
                    _cipherAlgorithmComboBox.Add(option.Key + keyLength);
                }
            }

            foreach (var option in SteMenu.PBEMenuTree)
            {
                _cipherAlgorithmComboBox.Add("PBE" + option.Key);
            }

            _blockModeComboBox = _controlFactory.Create<ComboBox>();
            _blockModeComboBox.Title = "Blockmode";
            _blockModeComboBox.Position.Width = 130;

            _paddingComboBox = _controlFactory.Create<ComboBox>();
            _paddingComboBox.Title = "Padding";
            _paddingComboBox.Position.Width = 150;

            _digestComboBox = _controlFactory.Create<ComboBox>();
            _digestComboBox.Title = "Digest";
            _digestComboBox.Position.Width = 150;
            _digestComboBox.Add("None");
            foreach (var option in SteMenu.IntegrityMenuTree["Digest"])
            {
                _digestComboBox.Add(option);
            }

            _signingComboBox = _controlFactory.Create<ComboBox>();
            _signingComboBox.Title = "Sign";
            _signingComboBox.Position.Width = 150;
            _signingComboBox.Add("None");
            foreach (var option in SteMenu.IntegrityMenuTree["Digital Signature"])
            {
                _signingComboBox.Add(option);
            }
        }

        private void RegisterEventHandler()
        {
            _digestComboBox.PropertySelectedItem.PropertyChanged += HandleDigest;
            _signingComboBox.PropertySelectedItem.PropertyChanged += HandleSigning;
            _confirmButton.InputState.Clicked += OnConfirmButtonClicked;
            _cancelButton.InputState.Clicked += OnCancelButtonClicked;
        }

        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Connection to next process isn't implemented yet.\n Come back later.");
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Usually one could expect the window to close here.\n but not yet...");
        }

        private void HandleSigning(object sender, EventArgs e)
        {
            string[] digestOptions = SteMenu.IntegrityMenuTree["Digest"];
            var currentDigestItem = (_digestComboBox.SelectedItem as ComboBoxMenuItem)?.Title;
            
            if (currentDigestItem != null && digestOptions.Contains(currentDigestItem))
                _digestComboBox.SelectItem("None");
        }
        
        private void HandleDigest(object sender, EventArgs e)
        {
            string[] signOptions = SteMenu.IntegrityMenuTree["Digital Signature"];
            var currentSignItem = (_signingComboBox.SelectedItem as ComboBoxMenuItem)?.Title;
            
            if (currentSignItem != null && signOptions.Contains(currentSignItem))
                _signingComboBox.SelectItem("None");
        }
        
        private void UpdateCombos(){
                
//            foreach( KeyValuePair<string, string[]> kvp in Menu.CipherMenuTree["AES"])
//            {
//                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.GetValue(0));
//                _blockModeComboBox.Add(kvp.Key);
//            }

//            _blockModeComboBox.Clear();
//            _paddingComboBox.Add("NoPadding");
//            _paddingComboBox.Add("PKCS7");
//            _paddingComboBox.Add("ZeroByte");

        }
    }

}