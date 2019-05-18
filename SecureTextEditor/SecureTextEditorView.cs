using System;
using System.IO;
using System.Reflection;
using System.Text;
using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;

namespace SecureTextEditor
{
    public class SecureTextEditorView : ContentControl
    {

//        private readonly ControlFactory _controlFactory;

        public DockPanel dockPanel;
        
        private string _path;

        private Button _loadBtn;
        private Button _saveBtn;
        
        private TextEditor _textBox;
        public string Text
        {
            get { return _textBox.GetText(); }
            set { _textBox.SetText(value); }
        }

        public SecureTextEditorView(IControlFactory controlFactory)
        {
            _textBox = controlFactory.Create<TextEditor>();
            
            CreateButtons(controlFactory);
            RegisterButtonEvents();
            
//            var loadBtn = controlFactory.Create<Button>();
//            loadBtn.Text = "Load";
//            loadBtn.InputState.Clicked += OnLoadButtonClicked;
            
//            var saveBtn = controlFactory.Create<Button>();
//            saveBtn.Text = "Save";
//            saveBtn.InputState.Clicked += OnSaveButtonClicked;

//            var buttonStackPanel = CreateButtonPanel(controlFactory);
//            CreateDockPanel(buttonStackPanel, controlFactory);
            CreateDockPanel(CreateButtonPanel(controlFactory), controlFactory);
            FocusManager.Default.SetFocus(_textBox);
            
            //POSITION BUTTONS
//            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
//            buttonStackPanel.ChildrenWidth = 60;
//            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
//            buttonStackPanel.Background = _textBox.Background;
//            buttonStackPanel.Children.Add(_loadBtn);
//            buttonStackPanel.Children.Add(_saveBtn);
//            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
//            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
//            buttonStackPanel.Margin.Right = 25;
//            buttonStackPanel.Margin.SetTopAndBottom(5);

//            dockPanel = controlFactory.Create<DockPanel>();
//            dockPanel.Add(Dock.Bottom, buttonStackPanel);
//            dockPanel.Add(Dock.Fill, _textBox);
//            dockPanel.Background = _textBox.Background;
//            FocusManager.Default.SetFocus(_textBox);

//            return dockPanel;
        }

        private void CreateDockPanel(HorizontalStackPanel buttonStackPanel, IControlFactory controlFactory)
        {
            dockPanel = controlFactory.Create<DockPanel>();
            dockPanel.Add(Dock.Bottom, buttonStackPanel);
            dockPanel.Add(Dock.Fill, _textBox);
            dockPanel.Background = _textBox.Background;
        }

        private HorizontalStackPanel CreateButtonPanel(IControlFactory controlFactory)
        {
            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 60;
            buttonStackPanel.Position.Height = _loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Children.Add(_loadBtn);
            buttonStackPanel.Children.Add(_saveBtn);
            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.Right = 25;
            buttonStackPanel.Margin.SetTopAndBottom(5);

            return buttonStackPanel;
        }

        private void CreateButtons(IControlFactory controlFactory)
        {
            _loadBtn = controlFactory.Create<Button>();
            _loadBtn.Text = "Load";
            
            _saveBtn = controlFactory.Create<Button>();
            _saveBtn.Text = "Save";
        }

        private void RegisterButtonEvents()
        {
            _loadBtn.InputState.Clicked += OnLoadButtonClicked;
            _saveBtn.InputState.Clicked += OnSaveButtonClicked;
        }


        public void LoadTextfile(String path)
        {
            if (File.Exists(path))
            {
                Text = File.ReadAllText(path);
                _path = AssemblyDirectory + "/../../../" + path;
            }
            FocusManager.Default.SetFocus(_textBox);
        }

        public void SaveTextfile()
        {
            File.WriteAllText(_path, Text, Encoding.UTF8);
            FocusManager.Default.SetFocus(_textBox);
        }
        
        public void OnLoadButtonClicked(object sender, EventArgs e)
        {
            LoadTextfile("dummy.txt");
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveTextfile();
        }
        
        private String AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}