using System;
using System.Text;
using BcFactory;
using BcFactory.Resources;
using Xunit;
using CryptoConfig = BcFactory.CryptoConfig;

namespace SecureTextEditorTests
{
    public class BcFactoryCryptoConfigTests
    {
        
        [Fact]
        public void TestInvalidCertCreateCall()
        {
            var config = new CryptoConfig();
            
            Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateCert(config));
        }
        
        [Fact]
        public void TestInvalidCertCreateConfig()
        {
            var config = new CryptoConfig
            {
                IsIntegrityActive = true,
                Integrity = Integrity.Digest
            };
            
            Assert.Throws<ArgumentException>(
                () => CryptoFactory.CreateCert(config));
        }

        //######################
        //Dsa Cert Combinations 
        //######################

        [Fact]
        public void TestGetValidAlgorithms()
        {
            
        }
        
        [Fact]
        public void TestGetKeySizes()
        {
            
        }
        
        [Fact]
        public void TestGetValidBlockModes()
        {
            
        }
        
        [Fact]
        public void TestGetValidPaddings()
        {
            
        }
        
        [Fact]
        public void TestGetIntegrityOptions()
        {
            
        }
        
        [Fact]
        public void TestGetDigest()
        {
            
        }

        [Fact]
        public void TestToString()
        {
            
        }
        
        [Fact]
        public void TestEnumExtensions()
        {
            
        }
        
        

    }
}