using System;
using System.IO;
using Medja;
using Medja.Controls;
using Medja.OpenTk;
using Medja.OpenTk.Themes.DarkBlue;
using Medja.OpenTk.Rendering;
using Medja.Theming;

namespace SecureTextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var library = new MedjaOpenTkLibrary(new DarkBlueTheme());
            library.RendererFactory = CreateRenderer;

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);

            var window = application.CreateWindow();
            window.CenterOnScreen(800, 600);
            //window.Background = Colors.Green;
            window.Content = CreateWindowContent(controlFactory);
            window.Title = "NotYetSecureTextEditor";

            application.MainWindow = window;
            application.Run();
        }
        

        private static Control CreateWindowContent(IControlFactory controlFactory)
        {

            var editor = controlFactory.Create<TextEditor>();
            editor.SetText(File.ReadAllText("dummy.txt"));

            var loadBtn = controlFactory.Create<Button>();
            loadBtn.Text = "Load";
            
            var saveBtn = controlFactory.Create<Button>();
            saveBtn.Text = "Save";

            var paddingDrpDwn = controlFactory.Create<Button>();
            paddingDrpDwn.Text = "Padding";

            var encryptDrpDwn = controlFactory.Create<Button>();
            encryptDrpDwn.Text = "Encryption";

            
            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 90;
            buttonStackPanel.Position.Height = saveBtn.Position.Height;
            
            buttonStackPanel.Children.Add(loadBtn);
            buttonStackPanel.Children.Add(saveBtn);
            buttonStackPanel.Children.Add(paddingDrpDwn);
            buttonStackPanel.Children.Add(encryptDrpDwn);

            buttonStackPanel.Background = editor.Background;
            //buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonStackPanel.Margin.SetTopAndBottom(5);
            buttonStackPanel.Margin.SetLeftAndRight(5);
            
            var dockPanel = controlFactory.Create<DockPanel>();
            dockPanel.Add(Dock.Bottom, buttonStackPanel);
            dockPanel.Add(Dock.Fill, editor);
            dockPanel.Background = editor.Background;
            FocusManager.Default.SetFocus(editor);

            return dockPanel;
        }

        private static IRenderer CreateRenderer()
        {
            var openTkRenderer = new OpenTkRenderer();
            return openTkRenderer;
        }
    }
}