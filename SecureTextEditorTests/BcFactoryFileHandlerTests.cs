using System;
using System.IO;
using BcFactory;
using SecureTextEditor;
using Xunit;

namespace SecureTextEditorTests
{
    public class BcFactoryFileHandlerTests
    {
        private const string Filename = "test";

        [Fact]
        public void TestInvalidConfigSaveToDisk()
        {
            byte[] key = Convert.FromBase64String("i/+IEVK8AdH6A9Jlh+i2Bg==");
            
            var config = new CryptoConfig
            {
                IsPbeActive = false,
                Key = key,
            };

            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(
                () => FileHandler.SaveToDisk(Filename, config));
        }

        [Fact]
        public void TestValidSaveKeyByte()
        {
            byte[] key = Convert.FromBase64String("i/+IEVK8AdH6A9Jlh+i2Bg==");
            string cipher = "UiJB+lZnsHaNoevzsg8FCht==";
            var config = new CryptoConfig
            {
                IsPbeActive = false,
                Key = key,
                Cipher = cipher
            };

            FileHandler.SaveToDisk(Filename, config);

            config = FileHandler.LoadKeys(Filename, config);
            Assert.Equal(key, config.Key);
        }
        
        [Fact]
        public void TestValidSavePublicKeyString()
        {
            string publicKey = "MCwCFEceSBmYoh/mj6ivfDJPgwrxmB4EAhRgxragMUKULWDiF2oBKrlI5qC4IA==";
            string cipher = "UiJB+lZnsHaNoevzsg8FCht==";
            var config = new CryptoConfig
            {
                IsPbeActive = false,
                SignaturePublicKey = publicKey,
                Cipher = cipher
            };

            FileHandler.SaveToDisk(Filename, config);

            config = FileHandler.LoadKeys(Filename, config);
            Assert.Equal(publicKey, config.SignaturePublicKey);
        }
        
        [Fact]
        public void TestValidLoadSteFile()
        {
            
        }
        
        [Fact]
        public void TestInvalidLoadSteFile()
        {
            
        }

        [Fact]
        public void TestValidProcessConfigOnSave()
        {
            
        }
        
        [Fact]
        public void TestInvalidProcessConfigOnSave()
        {
            
        }
        [Fact]
        public void TestValidProcessConfigOnLoad()
        {
            
        }
        
        [Fact]
        public void TestInvalidProcessConfigOnLoad()
        {
            
        }

    }
}