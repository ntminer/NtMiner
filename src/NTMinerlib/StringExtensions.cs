namespace NTMiner {
    public static class StringExtensions {
        public static bool IgnoreCaseContains(this string source, string key) {
            if (string.IsNullOrEmpty(source)) {
                if (string.IsNullOrEmpty(key)) {
                    return true;
                }
                return false;
            }
            if (string.IsNullOrEmpty(key)) {
                return true;
            }
            return source.ToLower().Contains(key.ToLower());
        }
    }
}
