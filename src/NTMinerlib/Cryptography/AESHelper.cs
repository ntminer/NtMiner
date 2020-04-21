using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace NTMiner.Cryptography {
    public static partial class AESHelper {
        private const int keySize = 128;
        public const int KeyLen = keySize / 8;

        public static string ConvertToKey(string str) {
            if (string.IsNullOrEmpty(str)) {
                str = new string(Enumerable.Repeat('0', KeyLen).ToArray());
            }
            if (str.Length < 16) {
                str += new string(Enumerable.Repeat('0', KeyLen - str.Length).ToArray());
            }
            if (str.Length > 16) {
                return str.Substring(0, 16);
            }
            return str;
        }

        /// <summary>
        /// 返回经过base64编码的字符串
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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
                    return System.Convert.ToBase64String(resultArray);
                }
            }
        }

        public static string Decrypt(string base64String, string key) {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toDecryptArray = System.Convert.FromBase64String(base64String);
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
