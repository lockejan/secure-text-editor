using System;
using System.IO;
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
    public class SteLoadDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private TextBox _passwordInput;
        private TextBlock _passwordLabel;

        private TextBlock _currentDirLabel;
        private readonly TextBlock _currentDir;
        
        private TextBlock _filenameInputLabel;
        private TextBox _filenameInput;

        private Button _confirmButton;
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
        public SteLoadDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _currentDir = CreateTextBlock(_workingDirectory);

            CreateButtons();
            CreateLabels();
            CreatInputs();
            RegisterEventHandler();

            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }

        private void CreatInputs()
        {
            _filenameInput = CreateInput();
            _passwordInput = CreateInput();
            _passwordInput.IsEnabled = false;
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
            for (int i = 0; i < 5; i++)
            {
                tablePanel.Rows.Add(new RowDefinition(rowHeight));
            }

            return FillTablePanel(tablePanel);
        }

        private TablePanel FillTablePanel(TablePanel grid)
        {
            grid.Children.Add(_passwordLabel);
            grid.Children.Add(_passwordInput);
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
            return textBlock;
        }

        private void CreateLabels()
        {
            _currentDirLabel = CreateTextBlock("Current DIR:");
            _filenameInputLabel = CreateTextBlock("Filename:");
            _passwordLabel = CreateTextBlock("Password:");
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
        
        private void RegisterEventHandler()
        {
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

    }

}