namespace NTMiner.Cryptography {
    public static partial class AESUtil {
        public static string GetRandomPassword() {
            return VirtualRoot.GetRandomString(16);
        }
    }
}
