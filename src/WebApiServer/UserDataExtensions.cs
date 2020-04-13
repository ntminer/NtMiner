using NTMiner.User;

namespace NTMiner {
    public static class UserDataExtensions {
        public static void Update(this UserData userData, Cryptography.RSAKey key) {
            if (userData == null || key == null) {
                return;
            }
            userData.PrivateKey = key.PrivateKey;
            userData.PublicKey = key.PublicKey;
        }
    }
}
