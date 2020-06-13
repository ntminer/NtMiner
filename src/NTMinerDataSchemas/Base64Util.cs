using System.Linq;

namespace NTMiner {
    /// <summary>
    /// 1.标准base64只有64个字符（英文大小写、数字和+、/）以及用作后缀等号；
    /// 2.base64是把3个字节变成4个可打印字符，所以base64编码后的字符串一定能被4整除（不算用作后缀的等号）；
    /// 3.等号一定用作后缀，且数目一定是0个、1个或2个。这是因为如果原文长度不能被3整除，base64要在后面添加\0凑齐3n位。为了正确还原，添加了几个\0就加上几个等号。显然添加等号的数目只能是0、1或2；
    /// 4.严格来说base64不能算是一种加密，只能说是编码转换。使用base64的初衷。是为了方便把含有不可见字符串的信息用可见字符串表示出来，以便复制粘贴；
    /// </summary>
    public static class Base64Util {
        private static readonly char[] _base64CodeArray = new char[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '+', '/', '='// 65个
        };

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

        public static bool IsBase64Char(char c) {
            return _base64CodeArray.Contains(c);
        }
    }
}
