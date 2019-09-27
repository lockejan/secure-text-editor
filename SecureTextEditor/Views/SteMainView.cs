using System;
using BcFactory;
using Medja.Controls;
using Medja.Theming;
using SecureTextEditor.FileHandler;

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

            CreateButtons();
            RegisterButtonEvents();
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

        private void CreateButtons()
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
        }

        private void RegisterButtonEvents()
        {
            _newBtn.InputState.Clicked += (s, e) =>
            {
                _textBox.SetText("");
                _textBox.SetCaretPosition(0,0);
                FocusManager.Default.SetFocus(_textBox);
            };
            
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
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
                    _config = SteCryptoHandler.LoadSteFile(content.Filename.Text);
                    Text = SteCryptoHandler.ProcessConfigToLoad(_config);
                }
                FocusManager.Default.SetFocus(_textBox);
            });
            
            DialogService.Show(dialog);
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var content = new SteSaveDialog(_controlFactory);
            var dialog = _controlFactory.Create<ConfirmableDialog>();
            var dockPanel = dialog.Content as DockPanel;
            
            dockPanel?.Add(Dock.Fill, content);
            
            ExecuteOnceOnClose(dialog, () =>
            {
                if (dialog.IsConfirmed)
                {
                    SteCryptoHandler.ProcessConfigToSave(content.Filename.Text, Text, _config);
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
                // self deregistration prevents a memory leak (at least if close is called once)
                dialog.Closed -= Handler;
                actionOnClose();
            }

            dialog.Closed += Handler;
        }
    }
}