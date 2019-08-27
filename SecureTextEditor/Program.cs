using System;
using System.Collections.Generic;
using Medja;
using Medja.Controls;
using Medja.OpenTk;
using Medja.OpenTk.Themes.DarkBlue;
using SecureTextEditor.Views;

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
            window.AutoSetContentAlignment = true;
//            window.CenterOnScreen(800, 600);
            window.Title = "NotYetSecureTextEditor";
            var container = DialogService.CreateContainer(controlFactory, new SteMainView(controlFactory));
            container.DialogPadding.SetAll(10);
            container.DialogPadding.Bottom = 250;
            window.Content = container;
            //window.Content = new SteSaveDialog(controlFactory);

            application.MainWindow = window;
            application.Run();
        }
        
    }
}