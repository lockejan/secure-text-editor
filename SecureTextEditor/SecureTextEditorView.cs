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

        private readonly ControlFactory _controlFactory;

        public DockPanel dockPanel;
        
        private TextEditor _textBox;
        public string Text
        {
            get { return _textBox.GetText(); }
            set { _textBox.SetText(value); }
        }

        public SecureTextEditorView(IControlFactory controlFactory)
        {
            _textBox = controlFactory.Create<TextEditor>();
            
            //PREPARE BUTTONS
            var loadBtn = controlFactory.Create<Button>();
            loadBtn.Text = "Load";
            loadBtn.InputState.Clicked += OnLoadButtonClicked;
            
            var saveBtn = controlFactory.Create<Button>();
            saveBtn.Text = "Save";
            saveBtn.InputState.Clicked += OnSaveButtonClicked;

            
            //POSITION BUTTONS
            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 60;
            buttonStackPanel.Position.Height = loadBtn.Position.Height;
            buttonStackPanel.Background = _textBox.Background;
            buttonStackPanel.Children.Add(loadBtn);
            buttonStackPanel.Children.Add(saveBtn);
            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.Right = 25;
            buttonStackPanel.Margin.SetTopAndBottom(5);

            dockPanel = controlFactory.Create<DockPanel>();
            dockPanel.Add(Dock.Bottom, buttonStackPanel);
            dockPanel.Add(Dock.Fill, _textBox);
            dockPanel.Background = _textBox.Background;
            FocusManager.Default.SetFocus(_textBox);

//            return dockPanel;
        }
        
        public void LoadText(String path)
        {
            if (File.Exists(path))
                Text = File.ReadAllText(path);
        }

        public void SaveText()
        {
            var path = "dummy.txt";
//            var combinePath = AssemblyDirectory(); 
            File.WriteAllText(path, Text, Encoding.UTF8);
        }
        
        public void OnLoadButtonClicked(object sender, EventArgs e)
        {
            //var inputState = (InputState)sender;
            //var loadBtn = (Button)inputState.Control;
   
            this.LoadText("dummy.txt");
            Console.WriteLine("Load-Button clicked!!!!");
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            this.SaveText();
            Console.WriteLine("Save-Button clicked!!!!");
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