using System.Linq;

namespace NTMiner {
    public static class Base64Util {
        private static char[] _base64CodeArray = new char[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '+', '/'// 刚好64个
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
