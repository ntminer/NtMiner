using System;
using System.Security.Cryptography;

namespace NTMiner {
    public static class CryptoUtil {
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
    }
}
