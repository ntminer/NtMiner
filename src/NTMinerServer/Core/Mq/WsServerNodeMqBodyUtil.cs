using System.Text;

namespace NTMiner.Core.Mq {
    public static class WsServerNodeMqBodyUtil {
        public static byte[] GetWsServerNodeAddressMqSendBody(string wsServerNodeAddress) {
            return Encoding.UTF8.GetBytes(wsServerNodeAddress);
        }
        public static string GetWsServerNodeAddressMqReceiveBody(byte[] body) {
            return Encoding.UTF8.GetString(body);
        }
    }
}
