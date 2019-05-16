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
            library.RendererFactory = CreateRenderer;

            var controlFactory = library.ControlFactory;
            var application = MedjaApplication.Create(library);
            
//            TextEditorControl test = new TextEditorControl(controlFactory);
//            var path = Path.Combine(AssemblyDirectory, "../../../");
//
//            Console.WriteLine("Welche Datei soll geladen werden?");
//            var file = Console.ReadLine();
//            test.LoadText(path + file);
//            Console.WriteLine(test.Text);
            
              var window = application.CreateWindow();
              window.CenterOnScreen(800, 600);
              //window.Background = Colors.Green;
              window.Content = CreateWindowContent(controlFactory);
              window.Title = "NotYetSecureTextEditor";
  
              application.MainWindow = window;
              application.Run();
            
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
        
          private static Control CreateWindowContent(IControlFactory controlFactory)
          {
  
              var editor = controlFactory.Create<TextEditor>();
              editor.SetText(File.ReadAllText("dummy.txt"));
  
              var loadBtn = controlFactory.Create<Button>();
              loadBtn.Text = "Load";
              
              var btn = controlFactory.Create<Button>();
              
              var saveBtn = controlFactory.Create<Button>();
              saveBtn.Text = "Save";
  
              var paddingDrpDwn = controlFactory.Create<ComboBox<TabItem>();
              paddingDrpDwn.Text = "Padding";
              
              var blockModeDrpDwn = controlFactory.Create<Button>();
              blockModeDrpDwn.Text = "Blockmodi";
  
              var encryptDrpDwn = controlFactory.Create<Button>();
              encryptDrpDwn.Text = "Encryption";
  
  
              var buttonStackPanel = controlFactory.Create<HorizontalStackPanel>();
              buttonStackPanel.ChildrenWidth = 90;
              buttonStackPanel.Position.Height = saveBtn.Position.Height;
              
              buttonStackPanel.Children.Add(loadBtn);
              buttonStackPanel.Children.Add(btn);
              buttonStackPanel.Children.Add(saveBtn);
              buttonStackPanel.Children.Add(paddingDrpDwn);
              buttonStackPanel.Children.Add(blockModeDrpDwn);
              buttonStackPanel.Children.Add(encryptDrpDwn);
  
              buttonStackPanel.Background = editor.Background;
              //buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
              buttonStackPanel.Margin.SetTopAndBottom(10);
              buttonStackPanel.Margin.SetLeftAndRight(10);
              
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