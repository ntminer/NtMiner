using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace NTMiner.Cryptography {
    public static partial class AESHelper {
        private const int keySize = 128;
        public const int KeyLen = keySize / 8;

        public static string ConvertToKey(string str) {
            if (string.IsNullOrEmpty(str)) {
                str = new string(Enumerable.Repeat('0', 16).ToArray());
            }
            if (str.Length < 16) {
                str+= new string(Enumerable.Repeat('0', 16 - str.Length).ToArray());
            }
            if (str.Length > 16) {
                return str.Substring(0, 16);
            }
            return str;
        }

        public static string Encrypt(string toEncrypt, string key) {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            using (var acsp = new AesCryptoServiceProvider { KeySize = keySize, BlockSize = keySize }) {
                acsp.GenerateIV();
                using (var aes = new AesCryptoServiceProvider { Key = keyArray, IV = acsp.IV, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 }) {
                    byte[] resultArray;
                    using (var cTransform = aes.CreateEncryptor()) {
                        resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    }
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }

        public static string Decrypt(string toDecrypt, string key) {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toDecryptArray = Encoding.UTF8.GetBytes(toDecrypt);
            using (var acsp = new AesCryptoServiceProvider { KeySize = keySize, BlockSize = keySize }) {
                acsp.GenerateIV();
                using (var aes = new AesCryptoServiceProvider { Key = keyArray, IV = acsp.IV, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 }) {
                    byte[] resultArray;
                    using (var cTransform = aes.CreateDecryptor()) {
                        resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                    }
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }
    }
}
