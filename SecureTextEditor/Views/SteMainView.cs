using System;
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
        private Button _closeBtn;

        private TextEditor _textBox;

        private string Text
        {
            get => _textBox.GetText();
            set => _textBox.SetText(value);
        }

        /// <summary>
        /// Creates mainView Component.
        /// Expects ControlFactory to create component. 
        /// </summary>
        /// <param name="controlFactory">Factory coming from medja.UI which is needed to provide control creation</param>
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
            buttonStackPanel.ChildrenWidth = 80;
//            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Add(_newBtn);
            buttonStackPanel.Add(_loadBtn);
            buttonStackPanel.Add(_saveBtn);
            buttonStackPanel.Add(_closeBtn);
            buttonStackPanel.Margin.SetAll(5);

            return buttonStackPanel;
        }

        private void CreateButtons()
        {
            void ConfigButton(Button btn, string label)
            {
                btn.Text = label;
            }

            ConfigButton(_newBtn = _controlFactory.Create<Button>(), "New");
            ConfigButton(_loadBtn = _controlFactory.Create<Button>(), "Load");
            ConfigButton(_saveBtn = _controlFactory.Create<Button>(), "Save");
            ConfigButton(_closeBtn = _controlFactory.Create<Button>(), "Exit");
        }

        private void RegisterButtonEvents()
        {
            _newBtn.InputState.Clicked += OnNewButtonClicked;
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
            _closeBtn.InputState.Clicked += OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Editor will close...sometime...soon...ish");
        }

        private void OnNewButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("This should clear the current view or present another tab");
        }

        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            //            Text = SteLoadCli.LoadTextDialog();
            DialogService.Show(
                _controlFactory.Create<Dialog>(
                    dialog =>
                        dialog.Content = new SteLoadDialog(_controlFactory)));

//            FocusManager.Default.SetFocus(_textBox);
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
        //            SteSaveCli.SaveDialog(Text);
        //            DialogService.Show(
        //                  _controlFactory.Create<Dialog>(
        //                    dialog =>
        //                        dialog.Content = new SteSaveDialog(_controlFactory)));

            var content = new SteSaveDialog(_controlFactory);
            var dialog = _controlFactory.Create<Dialog>(
                d => { d.Content = content; });


            ExecuteOnceOnClose(dialog, () =>
            {
                // ...
//                var settings = content.Settings;
//                CryptoFile.Save(settings, fileName, fileContent);
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