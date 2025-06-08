using Medja.Controls;
using Medja.Primitives;
using Medja.Theming;

namespace SecureTextEditor.Views
{
    /// <summary>
    /// SaveDialog view class to gather all options which then should be forwarded to the cipherEngines.
    /// Gets all menu possibilities from static dict in SteMenu Class.
    /// </summary>
    public class SteLoadDialog : ContentControl
    {
        private readonly IControlFactory _controlFactory;

        private TextBox _passwordInput;
        /// <summary>
        /// Public interface to password field.
        /// necessary for parent window.
        /// </summary>
        public TextBox Password => _passwordInput;
        
        private TextBox _filenameInput;
        /// <summary>
        /// Public interface to filename field.
        /// necessary for parent window. 
        /// </summary>
        public TextBox Filename => _filenameInput;

        private readonly VerticalStackPanel _firstColumnStack;
        private readonly VerticalStackPanel _secColumnStack;

        /// <summary>
        /// Creates saveDialog Component. Expects ControlFactory for component creation. 
        /// </summary>
        /// <param name="controlFactory"></param>
        public SteLoadDialog(IControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
            _firstColumnStack = GetStackPanel(120);
            _secColumnStack = GetStackPanel(150);

            CreateInputs();
            FillStackPanels();

            Content = CreateDockPanel();
            FocusManager.Default.SetFocus(_filenameInput);
        }
        
        private void CreateInputs()
        {
            TextBox CreateTextInput()
            {
                var textBox = _controlFactory.Create<TextBox>();
                return textBox;
            }

            _filenameInput = CreateTextInput();
            _passwordInput = CreateTextInput();
            //_passwordInput.IsEnabled = false;
        }

        private Control CreateDockPanel()
        {
            var dockPanel = _controlFactory.Create<DockPanel>();
            dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            dockPanel.VerticalAlignment = VerticalAlignment.Stretch;
            dockPanel.Add(Dock.Left, _firstColumnStack);
            dockPanel.Add(Dock.Fill, _secColumnStack);

            return dockPanel;
        }
        
        private VerticalStackPanel GetStackPanel(int width)
        {
            var vertStack = _controlFactory.Create<VerticalStackPanel>();
            vertStack.ChildrenHeight = 30;
            vertStack.Position.Width = width;
            vertStack.Padding.SetAll(5);
            return vertStack;
        }
        
        private TextBlock GetLabel(string text)
        {
            return _controlFactory.CreateTextBlock(text);
        }

        private void FillStackPanels()
        {
            void FillRow(Control first, Control sec)
            {
                _firstColumnStack.Add(first ?? new Control());
                _secColumnStack.Add(sec ?? new Control());
            }
            
            FillRow(GetLabel("Current DIR"), GetLabel(SteHelper.WorkingDirectory));
            FillRow(GetLabel("Filename"), _filenameInput);
            FillRow(GetLabel("Password"), _passwordInput);
        }


    }

}