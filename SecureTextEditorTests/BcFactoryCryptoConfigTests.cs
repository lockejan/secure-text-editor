using System;
using System.Collections.Generic;
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
        // Extension Methods   # 
        //######################

        [Fact]
        public void TestGetValidAlgorithms()
        {
            var config = new CryptoConfig();
            var result = config.GetValidAlgorithms();

            Assert.Collection(result,
             algorithm => Assert.Equal(CipherAlgorithm.AES, algorithm),
                algorithm => Assert.Equal(CipherAlgorithm.RC4, algorithm));
        }
        
        [Fact]
        public void TestGetKeySizesAES()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES
            };
            var result = config.GetKeySizes();

            Assert.Collection(result,
                keySize => Assert.Equal(128, keySize),
                keySize => Assert.Equal(192, keySize),
                keySize => Assert.Equal(256, keySize));
        }
        
        [Fact]
        public void TestGetKeySizesRC4()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.RC4
            };
            var result = config.GetKeySizes();

            Assert.Collection(result,
                keySize => Assert.Equal(40, keySize),
                keySize => Assert.Equal(128, keySize),
                keySize => Assert.Equal(256, keySize),
                keySize => Assert.Equal(512, keySize),
                keySize => Assert.Equal(1024, keySize),
                keySize => Assert.Equal(2048, keySize));
        }
        
        [Fact]
        public void TestGetValidBlockModesForAesNoPbe()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES
            };
            var result = config.GetValidBlockModes();

            Assert.Collection(result,
                blockmode => Assert.Equal(BlockMode.ECB, blockmode),
                blockmode => Assert.Equal(BlockMode.CBC, blockmode),
                blockmode => Assert.Equal(BlockMode.GCM, blockmode),
                blockmode => Assert.Equal(BlockMode.OFB, blockmode),
                blockmode => Assert.Equal(BlockMode.CTS, blockmode));
        }

        [Fact]
        public void TestGetValidBlockModesForAesPbkdf2()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                IsPbeActive = true,
                PbeAlgorithm = PbeAlgorithm.PBKDF2
            };
            var result = config.GetValidBlockModes();

            Assert.Collection(result,
                blockmode => Assert.Equal(BlockMode.CBC, blockmode));
        } 
        
        [Fact]
        public void TestGetValidBlockModesForAesScrypt()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                IsPbeActive = true,
                PbeAlgorithm = PbeAlgorithm.SCRYPT
            };
            var result = config.GetValidBlockModes();

            Assert.Collection(result,
                blockmode => Assert.Equal(BlockMode.GCM, blockmode));
        } 
       
        [Fact]
        public void TestGetValidBlockModesForRc4Pbkdf2()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.RC4,
                IsPbeActive = true,
                PbeAlgorithm = PbeAlgorithm.SCRYPT
            };
            var result = config.GetValidBlockModes();

            Assert.Collection(result,
                blockmode => Assert.Equal(BlockMode.None, blockmode));
        } 
        
        [Fact]
        public void TestGetValidBlockModesForRc4NoPbe()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.RC4
            };
            var result = config.GetValidBlockModes();

            Assert.Collection(result,
                blockmode => Assert.Equal(BlockMode.None, blockmode));
        }
        
        [Fact]
        public void TestGetValidPaddings()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                BlockMode = BlockMode.CBC
            };
            var result = config.GetValidPaddings();

            Assert.Collection(result,
                p => Assert.Equal(Padding.Pkcs7, p),
                p => Assert.Equal(Padding.ZeroByte, p));
        }
        
        [Fact]
        public void TestGetIntegrityOptionsForDigest()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                BlockMode = BlockMode.CBC,
                Integrity = Integrity.Digest
            };
            var result = config.GetIntegrityOptions();

            Assert.Collection(result,
                option => Assert.Equal(IntegrityOptions.Sha256, option),
                option => Assert.Equal(IntegrityOptions.AesCmac, option),
                option => Assert.Equal(IntegrityOptions.HmacSha256, option));
        }
        
        [Fact]
        public void TestGetIntegrityOptionsForDsa()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                BlockMode = BlockMode.CBC,
                Integrity = Integrity.Dsa
            };
            var result = config.GetIntegrityOptions();

            Assert.Collection(result,
                option => Assert.Equal(IntegrityOptions.Sha256, option));
        }
        
        [Fact]
        public void TestGetDigestForAesPbkdf2()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                PbeAlgorithm = PbeAlgorithm.PBKDF2
            };
            var result = config.GetDigest();

            Assert.Equal(PbeDigest.SHA256,result);
        }
        [Fact]
        public void TestGetDigestForAesSScrypt()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.AES,
                PbeAlgorithm = PbeAlgorithm.SCRYPT
            };
            var result = config.GetDigest();

            Assert.Equal(PbeDigest.GCM,result);
        }
        [Fact]
        public void TestGetDigestForRc4Pbkdf2()
        {
            var config = new CryptoConfig
            {
                CipherAlgorithm = CipherAlgorithm.RC4,
                PbeAlgorithm = PbeAlgorithm.PBKDF2
            };
            var result = config.GetDigest();

            Assert.Equal(PbeDigest.SHA1,result);
        }
        
    }
}