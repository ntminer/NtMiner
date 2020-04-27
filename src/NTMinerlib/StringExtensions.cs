using System;

namespace NTMiner {
    public static class StringExtensions {
        public static bool IgnoreCaseContains(this string source, string key) {
            if (source == null && key == null) {
                return true;
            }
            else if(source == null || key == null) {
                return false;
            }
            return source.IndexOf(key, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static string IgnoreCaseReplace(this string source, string oldValue, string newValue) {
            if (string.IsNullOrEmpty(source)) {
                return source;
            }
            if (string.IsNullOrEmpty(oldValue)) {
                return source;
            }
            int startIndex = 0;
            int index = source.IndexOf(oldValue, startIndex, StringComparison.OrdinalIgnoreCase);
            while (index != -1) {
                source = source.Replace(source.Substring(index, oldValue.Length), newValue);
                startIndex = index + newValue.Length;
                index = source.IndexOf(oldValue, startIndex, StringComparison.OrdinalIgnoreCase);
            }
            return source;
        }
    }
}
