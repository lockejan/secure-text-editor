﻿using Medja;
using Medja.OpenTk;
using Medja.OpenTk.Themes.DarkBlue;

namespace SecureTextEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var library = new MedjaOpenTkLibrary(new DarkBlueTheme());

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);

            var window = application.CreateWindow();
            window.CenterOnScreen(800, 600);
            window.Title = "NotYetSecureTextEditor";
            window.Content = new SecureTextEditorView(controlFactory);

            application.MainWindow = window;
            application.Run();
        }
        
    }
}