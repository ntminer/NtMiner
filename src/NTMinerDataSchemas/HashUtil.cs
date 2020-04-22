using System;
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
            return Sha1(System.Text.Encoding.UTF8.GetBytes(text));
        }

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
