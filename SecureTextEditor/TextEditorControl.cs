using System;
using System.IO;
using System.Text;
using Medja.Controls;
using Medja.Theming;

namespace SecureTextEditor
{
    public class TextEditorControl : ControlFactory
    {
        private readonly ControlFactory _controlFactory;
        
        private TextEditor _textBox;
        public string Text
        {
            get { return _textBox.GetText(); }
            set { _textBox.SetText(value); }
        }

        public TextEditorControl(IControlFactory controlFactory)
        {
            _textBox = controlFactory.Create<TextEditor>();
        }

        public void LoadText(String path)
        {
            if (File.Exists(path))
                Text = File.ReadAllText(path);
        }

        public void SaveText()
        {
            var path = "./dummy.txt"; 
            File.WriteAllText(path, Text, Encoding.UTF8);
        }
        
        public void OnLoadButtonClicked(object sender, EventArgs e)
        {
            //var inputState = (InputState)sender;
            //var loadBtn = (Button)inputState.Control;
            
            //editor.SetText(File.ReadAllText("dummy.txt"));
            
            this.LoadText("dummy.txt");
            Console.WriteLine("Load-Button clicked!!!!");
        }
//
//        private void OnSaveButtonClicked(object sender, EventArgs e)
//        {
//            //editor.SetText(File.ReadAllText("dummy.txt"));
//            
//            this.SaveText();
//            Console.WriteLine("Save-Button clicked!!!!");
//        }
    }
}