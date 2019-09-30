using System;
using Medja.Controls;
using Medja.Theming;
using SecureTextEditor.Views;
using Xunit;

namespace SecureTextEditorTests
{
    public class SteMainViewTests
    {
        private IControlFactory _controlFactory;

        [Fact]
        public void createMainView()
        {
            _controlFactory = new ControlFactory();
            var mainView = new SteMainView(_controlFactory);

            //Assert.Throws<MissingMethodException>(
              //  () => new SteMainView(_controlFactory));
        }
        
//        [Fact]
//        public void GetText()
//        {
//            _controlFactory = new ControlFactory();
//            var mainView = new SteMainView(_controlFactory);
//            
//            
//            //FocusManager.SetFocus(mainView.);
//            
//            mainView.Text = "Hallo Welt";
//            var result = mainView.Text;
//            
//            Assert.Equal("Hallo Welt", result);
//        }


    }
}