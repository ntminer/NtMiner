using System;
using System.Security.Cryptography;

namespace NTMiner {
    public static class HashUtil {
        public static string Sha1(byte[] data) {
            try {
                var sha1 = new SHA1CryptoServiceProvider();
                byte[] result = sha1.ComputeHash(data);
                return BitConverter.ToString(result).Replace("-", string.Empty).ToLowerInvariant();
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

        public static string EncDecInOne(string input) {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(input.Length.ToString());
            char[] output = new char[input.Length];

            for (int i = 0; i < input.Length; i++) {
                output[i] = (char)(input[i] ^ key[i % key.Length]);
            }

            return new string(output);
        }
    }
}
