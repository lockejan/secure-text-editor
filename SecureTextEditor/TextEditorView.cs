using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;

namespace SecureTextEditor
{
    public class TextEditorView
    {
        public TextEditorView(IControlFactory controlFactory)
        {
//            TextEditorControl STE = new TextEditorControl();
//            ButtonFactory invoke load
//            ButtonFactory invoke save


            //PREPARE BUTTONS

            var loadBtn = controlFactory.Create<Button>();
            loadBtn.Text = "Load";
//            loadBtn.InputState.Clicked += OnLoadButtonClicked;
            
            var saveBtn = controlFactory.Create<Button>();
            saveBtn.Text = "Save";
//            saveBtn.InputState.Clicked += OnSaveButtonClicked;

            
            //POSITION BUTTONS
            
            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 60;
            buttonStackPanel.Position.Height = loadBtn.Position.Height;
//            buttonStackPanel.Background = editor.Background;
            buttonStackPanel.Children.Add(loadBtn);
            buttonStackPanel.Children.Add(saveBtn);
            buttonStackPanel.Position.Width = 2 * buttonStackPanel.ChildrenWidth.Value;
            buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.Right = 25;
            buttonStackPanel.Margin.SetTopAndBottom(5);

            var dockPanel = controlFactory.Create<DockPanel>();
//            dockPanel.Add(Dock.Bottom, buttonStackPanel);
//            dockPanel.Add(Dock.Fill, editor);
//            dockPanel.Background = editor.Background;
//            FocusManager.Default.SetFocus(editor);

//            return dockPanel;
        }
    }
}