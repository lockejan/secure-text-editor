using System;
using System.IO;
using System.Reflection;
using Medja;
using Medja.Primitives;
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
//            library.RendererFactory = CreateRenderer;

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);
            
            TextEditorControl test = new TextEditorControl(controlFactory);
            var path = Path.Combine(AssemblyDirectory, "../../../");

            Console.WriteLine("Welche Datei soll geladen werden?");
            var file = Console.ReadLine();
            test.LoadText(path + file);
            Console.WriteLine(test.Text);
        }

        public static string AssemblyDirectory
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