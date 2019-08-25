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
        /// <param name="controlFactory"></param>
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
            buttonStackPanel.ChildrenWidth = 100;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Add(_loadBtn);
            buttonStackPanel.Add(_saveBtn);
            buttonStackPanel.Margin.SetAll(5);

            return buttonStackPanel;
        }
   
        private void CreateButtons()
        {
            void ConfigButton(Button btn,string label)
            {
                btn.Position.Width = 100;
                btn.Text = label;
            }
            
            ConfigButton(_loadBtn=_controlFactory.Create<Button>(), "Load");
            ConfigButton(_saveBtn=_controlFactory.Create<Button>(), "Save");
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
        }

        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
            Text = "blub";
            FocusManager.Default.SetFocus(_textBox);
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SteSaveCli.SaveDialog(Text);
//            DialogService.Show(
//                _controlFactory.Create<Dialog>(
//                    dialog =>
//                    {
//                        dialog.Content = new SteSaveDialog(_controlFactory);
//                    }));
        }
        
    }
}