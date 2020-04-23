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
            return Sha1(Encoding.UTF8.GetBytes(text));
        }
    }
}
