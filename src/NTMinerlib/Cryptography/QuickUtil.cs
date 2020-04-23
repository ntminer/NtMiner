using System.Text;

namespace NTMiner.Cryptography {
    public static class QuickUtil {
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
