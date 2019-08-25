using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Medja.Controls;
using Medja.OpenTk.Rendering;
using Medja.OpenTk.Themes;
using Medja.Primitives;
using Medja.Theming;
using Medja.Utils;
using SkiaSharp;

namespace SecureTextEditor
{
    public class SteSaveDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private ComboBox _cipherAlgorithmComboBox;
        private ComboBox _blockModeComboBox;
        private ComboBox _paddingComboBox;
        private TextBox _passwordInput;
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

        private Button _saveButton;
        private Button _cancelButton;
        
        private String _workingDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        
        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteSaveDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _currentDir = CreateTextBlock(_workingDirectory);

            CreateButtons();
            CreateLabels();
            CreatInputs();
            CreateComboBoxes();
            InitStateOfCombos();

            Content = CreateDockPanel();
//            Content.HorizontalAlignment = HorizontalAlignment.Stretch;
//            Content.VerticalAlignment = VerticalAlignment.Top;
            FocusManager.Default.SetFocus(_cipherAlgorithmComboBox);
//            this.DebugLayout();
        }

        private void InitStateOfCombos()
        {
//            _blockModeComboBox.IsEnabled = false;

//            Console.WriteLine(_cipherAlgorithmComboBox.);
//            _digestComboBox.SelectItem("None");
//            _signingComboBox.SelectItem("None");
//            Console.WriteLine(_digestComboBox.SelectedItem);
//            Console.WriteLine(_digestComboBox.DisplayText);
//
//            if (_digestComboBox.DisplayText.Equals("None"))
//            {
//                Console.WriteLine("NOnE IS NONE");
//            }
//            
//            foreach (var item in _cipherAlgorithmComboBox.GetChildren())
//            {
//                Console.WriteLine(item);
//            }

            
//            _paddingComboBox.IsEnabled = false;
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
//            const float columnWidth = 300;
            
            tablePanel.Columns.Add(new ColumnDefinition(150));
            tablePanel.Columns.Add(new ColumnDefinition(470));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            tablePanel.Rows.Add(new RowDefinition(rowHeight));
            
            return FillTablePanel(tablePanel);
        }

        private TablePanel FillTablePanel(TablePanel grid)
        {
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
            grid.Children.Add(_saveButton);
            grid.Children.Add(_cancelButton);

            return grid;
        }
        
//        private TextBlock CreateTextBlock(String text)
//        {
//            var textBlock = _controlFactory.Create<TextBlock>();
//            textBlock.Text = text;
//            textBlock.Renderer = new TextBlockRenderer(textBlock);
//            return textBlock;
//        }
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
            _cipherAlgorithmLabel = CreateTextBlock("Cipher/PBE:");
            _currentDirLabel = CreateTextBlock("Parent Working Directory:");
            _digestLabel = CreateTextBlock("Digest:");
            _filenameInputLabel = CreateTextBlock("Filename:");
            _paddingLabel = CreateTextBlock("Padding:");
            _passwordLabel = CreateTextBlock("Password:");
            _signingLabel = CreateTextBlock("Digital Sign:");
        }

        private void CreateButtons()
        {
            _saveButton = _controlFactory.Create<Button>();
            _saveButton.Position.Width = 100;
            _saveButton.Text = "Save";

            _cancelButton = _controlFactory.Create<Button>();
            _cancelButton.Position.Width = 100;
            _cancelButton.Text = "Cancel";
        }

        private void CreateComboBoxes()
        {
            _cipherAlgorithmComboBox = _controlFactory.Create<ComboBox>();
            _cipherAlgorithmComboBox.Title = "Cipher/PBE";
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
            
//            foreach( KeyValuePair<string, string[]> kvp in Menu.CipherMenuTree["AES"])
//            {
//                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.GetValue(0));
//                _blockModeComboBox.Add(kvp.Key);
//            }
            
//            _blockModeComboBox.Add("ECB");
//            _blockModeComboBox.Add("CBC");
//            _blockModeComboBox.Add("CTS");
//            _blockModeComboBox.Add("OFB");
//            _blockModeComboBox.Add("GCM");
//            _blockModeComboBox.Clear();

            _paddingComboBox = _controlFactory.Create<ComboBox>();
            _paddingComboBox.Title = "Padding";
            _paddingComboBox.Position.Width = 150;
//            _paddingComboBox.Add("NoPadding");
//            _paddingComboBox.Add("PKCS7");
//            _paddingComboBox.Add("ZeroByte");
            
            _digestComboBox = _controlFactory.Create<ComboBox>();
            _digestComboBox.Title = "Digest";
            _digestComboBox.Position.Width = 150;

            foreach (var option in SteMenu.IntegrityMenuTree["Digest"])
            {
                _digestComboBox.Add(option);
            }
            
            _signingComboBox = _controlFactory.Create<ComboBox>();
            _signingComboBox.Title = "Sign";
            _signingComboBox.Position.Width = 150;
            
            foreach (var option in SteMenu.IntegrityMenuTree["Digital Signature"])
            {
                _signingComboBox.Add(option);
            }
            
            _digestComboBox.PropertySelectedItem.PropertyChanged += HandleIntegrityOptions;
            _signingComboBox.PropertySelectedItem.PropertyChanged += HandleIntegrityOptions;
        }

        private void HandleIntegrityOptions(object sender, EventArgs e)
        {
            string[] digestOptions = SteMenu.IntegrityMenuTree["Digest"];
            string[] signOptions = SteMenu.IntegrityMenuTree["Digital Signature"];
            Console.WriteLine(digestOptions);
            Console.WriteLine(signOptions);
            
            var currentDigestItem = (_digestComboBox.SelectedItem as ComboBoxMenuItem)?.Title;
            var currentSignItem = (_signingComboBox.SelectedItem as ComboBoxMenuItem)?.Title;

            if (currentSignItem != null && signOptions.Contains(currentSignItem))
            {
                if(signOptions.Contains(currentSignItem))
                    _signingComboBox.SelectItem("None");
            }
            else if (currentDigestItem != null && digestOptions.Contains(currentDigestItem)) 
            {
                if(digestOptions.Contains(currentDigestItem))
                    _digestComboBox.SelectItem("None");
            }
        }

    }
    
    //DialogService.Hide(dialog) oder dialog.Hide()
    
    public class TextBlockRenderer : TextControlRendererBase<TextBlock>
    {
        private readonly SKPaint _backgroundPaint;

        public TextBlockRenderer(TextBlock control)
            : base(control)
        {
            _backgroundPaint = new SKPaint();
            _control.AffectRendering(control.PropertyBackground);
        }
        
        protected override void DrawTextControlBackground()
        {
            _backgroundPaint.Color = _control.Background.ToSKColor();
            _canvas.DrawRoundRect(_rect, 3, 3, _backgroundPaint);
        }

        protected override void Dispose(bool disposing)
        {
            _backgroundPaint.Dispose();
            base.Dispose(disposing);
        }
    }
}