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
        enum BlockMode {ECB, CBC, GCM, OFB, CTS};

        [Flags]
        enum Padding{
            NoPadding = 1,
            ZeroBytePadding = 1 << 1,
            PKCS7 = 1 << 2
        }
        
        static void Main(string[] args)
        {
//            var library = new MedjaOpenTkLibrary(new DarkBlueTheme());
//
//            var controlFactory = library.ControlFactory;
//            var application = MedjaApplication.Create(library);
//
//            var window = application.CreateWindow();
//            window.AutoSetContentAlignment = true;
////            window.CenterOnScreen(800, 600);
//            window.Title = "NotYetSecureTextEditor";
//            var container = DialogService.CreateContainer(controlFactory, new SteMainView(controlFactory));
//            container.DialogPadding.SetAll(10);
//            window.Content = container;
////            window.Content = new SteSaveDialog(controlFactory);
//
//            application.MainWindow = window;
//            application.Run();
            
            var Dict = new Dictionary<BlockMode, Padding>()
            {
                {BlockMode.ECB, Padding.ZeroBytePadding | Padding.PKCS7},
                {BlockMode.CBC, Padding.ZeroBytePadding | Padding.PKCS7},
            };

            var value = Dict[BlockMode.ECB];
        
            if (value.HasFlag(Padding.NoPadding))
                Console.WriteLine("Treffer A");
        
            if(value.HasFlag(Padding.ZeroBytePadding))
                Console.WriteLine("Treffer B");
        }
        
    }
}