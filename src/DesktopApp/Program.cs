using Medja;
using Medja.Controls;
using Medja.OpenTk;
using Medja.OpenTk.Themes.DarkBlue;
using SecureTextEditor.Views;

namespace SecureTextEditor
{
    class Program
    {
        private static void Main(string[] args)
        {
            var library = new MedjaOpenTkLibrary(new DarkBlueTheme());

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);

            var window = application.CreateWindow();
            //window.CenterOnScreen(800, 600);
            window.Title = "SecureTextEditor";
            window.AutoSetContentAlignment = true;

            var container = DialogService.CreateContainer(controlFactory, new SteMainView(controlFactory));
            container.DialogPadding.SetAll(5);
            window.Content = container;

            application.MainWindow = window;
            application.Run();
        }

    }
}