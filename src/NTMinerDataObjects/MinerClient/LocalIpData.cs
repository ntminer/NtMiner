namespace NTMiner.MinerClient {
    public class LocalIpData : ILocalIp {
        public LocalIpData() {
        }

        public string SettingID { get; set; }

        public string Name { get; set; }

        public string DefaultIPGateway { get; set; }

        public bool DHCPEnabled { get; set; }

        public string IPAddress { get; set; }

        public string IPSubnet { get; set; }

        public string DNSServer0 { get; set; }

        public string DNSServer1 { get; set; }

        public override string ToString() {
            return
$@"SettingID={SettingID}
Name={Name}
DefaultIPGateway={DefaultIPGateway}
DHCPEnabled={DHCPEnabled}
IPAddress={IPAddress}
IPSubnet={IPSubnet}
DNSServer0={DNSServer0}
DNSServer1={DNSServer1}";
        }
    }
}
