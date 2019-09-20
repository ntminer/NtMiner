using System;

namespace NTMiner.MinerClient {
    public class LocalIpData : ILocalIp {
        public LocalIpData() {
            this.DefaultIPGateway = new string[0];
            this.IPAddress = new string[0];
            this.DNSServerSearchOrder = new string[0];
        }

        public Guid SettingID { get; set; }

        public string[] DefaultIPGateway { get; set; }

        public bool DHCPEnabled { get; set; }

        public string DHCPServer { get; set; }

        public string[] IPAddress { get; set; }

        public string[] DNSServerSearchOrder { get; set; }

        public override string ToString() {
            return
$@"SettingID={SettingID}
DefaultIPGateway={string.Join(",", DefaultIPGateway)}
DHCPEnabled={DHCPEnabled}
DHCPServer={DHCPServer}
IPAddress={string.Join(",", IPAddress)}
DNSServerSearchOrder={string.Join(",", DNSServerSearchOrder)}";
        }
    }
}
