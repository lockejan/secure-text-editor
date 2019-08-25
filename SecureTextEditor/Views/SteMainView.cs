using System;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;
using Medja.Utils;

namespace SecureTextEditor
{
    /// <summary>
    /// Main view containing TextEditorControl, Save and Load Buttons
    /// </summary>
    public class SteMainView : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private Button _loadBtn;
        private Button _saveBtn;

//        private SecureTextEditorModel _cryptoFabric;

        private TextEditor _textBox;
        
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get => _textBox.GetText();
            set => _textBox.SetText(value);
        }

        /// <summary>
        /// Creates mainView Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteMainView(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _textBox = _controlFactory.Create<TextEditor>();
//            _cryptoFabric = new SecureTextEditorModel();

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
            buttonStackPanel.Margin.SetLeftAndRight(5);
            buttonStackPanel.Margin.SetTopAndBottom(5);

            return buttonStackPanel;
        }
        
        private void CreateButtons()
        {
            _loadBtn = _controlFactory.Create<Button>();
            _loadBtn.Position.Width = 100;
            _loadBtn.Text = "Load";
            
            _saveBtn = _controlFactory.Create<Button>();
            _saveBtn.Position.Width = 100;
            _saveBtn.Text = "Save";
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
        }

        private void OnLoadButtonClicked(object sender, EventArgs e)
        {
//            Text = _cryptoFabric.LoadTextfile();
//            Text = "blub";
//            FocusManager.Default.SetFocus(_textBox);

            DialogService.Show(
                _controlFactory.Create<Dialog>(
                    d =>
                    {
                        d.Content = new SteSaveDialog(_controlFactory);
                    }));
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SteSaveCli.SaveDialog();
        }
        
        
    }
}