using NTMiner.Cryptography;
using System.Text;

namespace NTMiner.Core.Mq {
    public static class UserMqBodyUtil {
        #region LoginName
        public static byte[] GetLoginNameMqSendBody(string loginName) {
            return Encoding.UTF8.GetBytes(loginName);
        }
        public static string GetLoginNameMqReceiveBody(byte[] data) {
            return Encoding.UTF8.GetString(data);
        }
        #endregion

        #region UpdateUserRSAKey
        public static byte[] GetUpdateUserRSAKeyMqSendBody(RSAKey key) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(key));
        }
        public static RSAKey GetUpdateUserRSAKeyMqReceiveBody(byte[] data) {
            string json = Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(json)) {
                return null;
            }
            return VirtualRoot.JsonSerializer.Deserialize<RSAKey>(json);
        }
        #endregion
    }
}
