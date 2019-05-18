using Medja;
using Medja.OpenTk;
using Medja.OpenTk.Themes.DarkBlue;
using Medja.OpenTk.Rendering;

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

            var test = new SecureTextEditorView(controlFactory);
            window.Content = test.dockPanel;//new SecureTextEditorView(controlFactory);
            window.Title = "NotYetSecureTextEditor";

            application.MainWindow = window;
            application.Run();
        }

        private static IRenderer CreateRenderer()
        {
            var openTkRenderer = new OpenTkRenderer();
            return openTkRenderer;
        }

    }
}