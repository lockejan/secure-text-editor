using System.Text;
using CryptoEngine;
using DesktopApp.Views;
using SecureTextEditor.Views;
using Xunit;

namespace DesktopApp.Tests;

public class SteViewTests
{
    private IControlFactory _controlFactory = new ControlFactory();

    [Fact]
    public void TestCreateMainView()
    {
        var mainView = new SteMainView(_controlFactory);
        Assert.IsType<SteMainView>(mainView);

        //Assert.Throws<MissingMethodException>(
        //  () => new SteMainView(_controlFactory));
    }
        
    [Fact]
    public void TestLoadDialogFilenameField()
    {
        var saveView = new SteLoadDialog(_controlFactory);
        saveView.Filename.Text = "datei";
        var content = saveView.Filename.Text;
            
        Assert.Equal("datei",content);
    }
        
    [Fact]
    public void TestLoadDialogPasswordField()
    {
        var saveView = new SteLoadDialog(_controlFactory);
        saveView.Password.Text = "secret1234";
        var content = saveView.Password.Text;
            
        Assert.Equal("secret1234", content);
    }
        
    [Fact]
    public void TestSaveDialogCryptoConfig()
    {
        int byteCount = Encoding.UTF8.GetByteCount("my secret note");

        var saveView = new SteSaveDialog(_controlFactory, byteCount);

        var cryptoConfig = saveView.Config;
        Assert.IsType<CryptoConfig>(cryptoConfig);
    }
        
    [Fact]
    public void TestSaveDialogFilenameField()
    {
        int byteCount = Encoding.UTF8.GetByteCount("my secret note");
            
        var loadView = new SteSaveDialog(_controlFactory, byteCount);
        loadView.Filename.Text = "datei";
        var content = loadView.Filename.Text;
            
        Assert.Equal("datei",content);
    }

    [Fact]
    public void TestSaveDialogPasswordField()
    {
        int byteCount = Encoding.UTF8.GetByteCount("my secret note");
            
        var loadView = new SteSaveDialog(_controlFactory, byteCount);
        loadView.Password.Text = "secret1234";
        var content = loadView.Password.Text;
            
        Assert.Equal("secret1234",content);
    }

}