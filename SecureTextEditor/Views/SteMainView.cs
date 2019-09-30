using System;
using System.Text;
using BcFactory;
using Medja.Controls;
using Medja.Theming;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// Main view containing TextEditorControl, Save and Load Buttons.
    /// </summary>
    public class SteMainView : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private Button _loadBtn;
        private Button _saveBtn;
        private Button _newBtn;

        private CryptoConfig _config;

        private readonly TextEditor _textBox;

        /// <summary>
        /// private interface to simplify working with textEditor inside class.
        /// </summary>
        private string Text
        {
            get => _textBox.GetText();
            set => _textBox.SetText(value);
        }

        /// <summary>
        /// Creates mainView Component.
        /// Expects ControlFactory to create component. 
        /// </summary>
        /// <param name="controlFactory">Factory coming from medja.UI which is needed for control creation</param>
        public SteMainView(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _textBox = _controlFactory.Create<TextEditor>();

            CreateAndRegisterButtons();
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
            buttonStackPanel.ChildrenWidth = 90;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Add(_newBtn);
            buttonStackPanel.Add(_loadBtn);
            buttonStackPanel.Add(_saveBtn);
            buttonStackPanel.Margin.Left = 5;

            return buttonStackPanel;
        }

        private void CreateAndRegisterButtons()
        {
            void ConfigButton(Button btn, string label)
            {
                btn.Text = label;
                btn.Margin.SetTopAndBottom(5);
                btn.Padding.Top = 5;
            }

            //theme - overload class to change button height
            //alternative - set padding manually at btn-control
            ConfigButton(_newBtn = _controlFactory.Create<Button>(), "New");
            ConfigButton(_loadBtn = _controlFactory.Create<Button>(), "Load");
            ConfigButton(_saveBtn = _controlFactory.Create<Button>(), "Save");
            _newBtn.InputState.Clicked += OnNewButtonClicked;
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
        }

        private void OnNewButtonClicked(object sender, EventArgs e)
        {
            _textBox.SetText("");
            _textBox.SetCaretPosition(0,0);
            FocusManager.Default.SetFocus(_textBox);
        }
        
        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            var content = new SteLoadDialog(_controlFactory);
            var dialog = _controlFactory.Create<ConfirmableDialog>();
            var dockPanel = dialog.Content as DockPanel;
            
            dockPanel?.Add(Dock.Fill, content);
            
            ExecuteOnceOnClose(dialog, () =>
            {
                if (dialog.IsConfirmed)
                {
                    _config = FileHandler.LoadSteFile(content.Filename.Text);
                    _config = FileHandler.LoadKeys(content.Filename.Text, _config);
                    _config.PbePassword = content.Password.Text.ToCharArray();
                    Text = FileHandler.ProcessConfigOnLoad(_config);
                }
                FocusManager.Default.SetFocus(_textBox);
            });
            
            DialogService.Show(dialog);
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var content = new SteSaveDialog(_controlFactory, Encoding.UTF8.GetByteCount(Text));
            var dialog = _controlFactory.Create<ConfirmableDialog>();
            var dockPanel = dialog.Content as DockPanel;
            
            dockPanel?.Add(Dock.Fill, content);
            
            ExecuteOnceOnClose(dialog, () =>
            {
                if (dialog.IsConfirmed)
                {
                    if (content.Config.IsPbeActive)
                        content.Config.PbePassword = content.Password.Text.ToCharArray();
                    
                    var config = FileHandler.ProcessConfigOnSave(Text, content.Config);
                    FileHandler.SaveToDisk(content.Filename.Text, config);
                }
                FocusManager.Default.SetFocus(_textBox);
            });

            DialogService.Show(dialog);
        }
        
        private static void ExecuteOnceOnClose(Dialog dialog, Action actionOnClose)
        {
            if (actionOnClose == null)
                return;

            void Handler(object s, EventArgs e)
            {
                // self unregister prevents a memory leak (at least if close is called once)
                dialog.Closed -= Handler;
                actionOnClose();
            }

            dialog.Closed += Handler;
        }
    }
}