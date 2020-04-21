namespace NTMiner.Cryptography {
    public static partial class AESHelper {
        public static string GetRandomPassword() {
            return VirtualRoot.GetRandomString(KeyLen);
        }

    }
}
