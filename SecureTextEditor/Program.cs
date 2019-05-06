using System;
using System.IO;
using Medja;
using Medja.Controls;
using Medja.OpenTk;
using Medja.OpenTk.Rendering;
using Medja.Theming;

namespace SecureTextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var library = new MedjaOpenTkLibrary(new DarkBlueTheme());//new Medja.OpenTk.Themes.DarkBlue.DarkBlueTheme());
            library.RendererFactory = CreateRenderer;

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);

            var window = application.CreateWindow();
            window.CenterOnScreen(800, 600);
            //window.Background = Colors.Green;
            window.Content = CreateWindowContent(controlFactory);
            window.Title = "Editor";

            application.MainWindow = window;
            application.Run();
        }
        

        private static Control CreateWindowContent(IControlFactory controlFactory)
        {

            var editor = controlFactory.Create<TextEditor>();
            editor.SetText(File.ReadAllText("dummy.txt"));

            var btn = controlFactory.Create<Button>();
            btn.Text = "Button";

            var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
            buttonStackPanel.ChildrenWidth = 60;
            buttonStackPanel.Position.Height = btn.Position.Height;
            buttonStackPanel.Background = editor.Background;
            buttonStackPanel.Children.Add(btn);
            Console.WriteLine("editor background: " + editor.Background);

            var dockPanel = controlFactory.Create<DockPanel>();
            dockPanel.Add(Dock.Bottom, buttonStackPanel);
            dockPanel.Add(Dock.Fill, editor);
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