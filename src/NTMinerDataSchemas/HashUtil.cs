using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NTMiner {
    public static class HashUtil {
        public static string Sha1(byte[] data) {
            try {
                using (var sha1 = new SHA1CryptoServiceProvider()) {
                    byte[] result = sha1.ComputeHash(data);
                    return BitConverter.ToString(result).Replace("-", string.Empty).ToLowerInvariant();
                }
            }
            catch {
                return string.Empty;
            }
        }

        public static string Sha1(string text) {
            if (string.IsNullOrEmpty(text)) {
                return string.Empty;
            }
            return Sha1(Encoding.UTF8.GetBytes(text));
        }

        public static bool IsBase64OrEmpty(string base64Str) {
            if (string.IsNullOrEmpty(base64Str)) {
                return true;
            }
            else if (base64Str.Length % 4 != 0) {
                return false;
            }
            else if (base64Str.Any(c => !_base64CodeArray.Contains(c))) {
                return false;
            }
            return true;
        }


        private static char[] _base64CodeArray = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '+', '/', '='
        };

        // 矿机上填写的Windows远程桌面的密码是加密返回的，先前的加解密方法引入了一个BUG因为
        // 加密结果有换行等特殊符号从而导致服务器端json序列化时CPU和内存使用率奇高，现已修复。
        public static byte[] TextEncrypt(string content, string secretKey) {
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            for (int i = 0; i < data.Length; i++) {
                data[i] ^= key[i % key.Length];
            }

            return data;
        }

        public static string TextDecrypt(byte[] data, string secretKey) {
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            for (int i = 0; i < data.Length; i++) {
                data[i] ^= key[i % key.Length];
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }
}
