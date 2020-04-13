using NTMiner.Core.MinerServer;
using System;
using System.Text;

namespace NTMiner.Core.Mq {
    public static class MinerClientMqBodyUtil {
        public static byte[] GetClientIdMqSendBody(Guid clientId) {
            return Encoding.UTF8.GetBytes(clientId.ToString());
        }
        public static Guid GetClientIdMqReciveBody(byte[] body) {
            string str = Encoding.UTF8.GetString(body);
            if (string.IsNullOrEmpty(str)) {
                return Guid.Empty;
            }
            if (Guid.TryParse(str, out Guid clientId)) {
                return clientId;
            }
            return Guid.Empty;
        }

        public static byte[] GetMinerIdMqSendBody(string minerId) {
            return Encoding.UTF8.GetBytes(minerId);
        }
        public static string GetMinerIdMqReciveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }

        public static byte[] GetChangeMinerSignMqSendBody(MinerSign minerSign) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(minerSign));
        }

        public static MinerSign GetChangeMinerSignMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<MinerSign>(json);
        }
    }
}
