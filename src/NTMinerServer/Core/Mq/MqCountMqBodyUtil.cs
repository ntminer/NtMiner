using NTMiner.ServerNode;
using System.Text;

namespace NTMiner.Core.Mq {
    public static class MqCountMqBodyUtil {
        public static byte[] GetMqCountMqSendBody(MqCountData mqCount) {
            return Encoding.UTF8.GetBytes(VirtualRoot.JsonSerializer.Serialize(mqCount));
        }

        public static MqCountData GetMqCountMqReceiveBody(byte[] body) {
            string json = Encoding.UTF8.GetString(body);
            return VirtualRoot.JsonSerializer.Deserialize<MqCountData>(json);
        }
    }
}
