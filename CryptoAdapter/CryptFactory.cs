//namespace CryptoAdapter
//{
//
//        public class CryptFactory
//        {
//            public static Crypt Create(CryptSettings settings)
//            {
//                if(settings.Cipher == Cipher.AES && ...)
//                    return AESBlaBlaCipher(settings);
//                else if(....)
//                    else
//                throw new NotSupportedException("The given settings combination is not supported");
//            }
//        }
//
//        abstract class Cipher // oder interface => abhängig davon ob es gemeinsam genutzten code in der Basisklasse gibt: abstract: ja, interface: nein
//        {
//            protected readonly Settings _settings;
//
//            public Cipher(Settings settings)
//            {   _settings = settings ?? throw new ArgumentNullException(nameof(settings));
//            }
//
//            public abstract string Encrypt(string content); // nicht file name; schreibe ich gleich noch was zu
//            public abstract string Decrypt(string content);
//        }
//
//        class AesThisAndThatCipher : Cipher
//        {
//            // ctor (Abkürzung für Constructor)...
//
//            public override string Encrypt(string content)
//            {
//                // do it
//            }
//
//            // Decrypt genauso
//        }
// 
//}