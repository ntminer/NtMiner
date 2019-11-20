using System.Net.NetworkInformation;

namespace NTMiner.Net {
    public class DeviceScanner {
        public static bool IsHostAccessible(string hostNameOrAddress) {
            using (Ping ping = new Ping()) {
                PingReply reply = ping.Send(hostNameOrAddress, 100);

                return reply != null && reply.Status == IPStatus.Success;
            }
        }
    }
}
