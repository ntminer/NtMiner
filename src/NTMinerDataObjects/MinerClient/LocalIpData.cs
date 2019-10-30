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

        public static bool operator==(LocalIpData left, LocalIpData right) {
            if (left == null) {
                if (right == null) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return left.Equals(right);
            }
        }

        public static bool operator !=(LocalIpData left, LocalIpData right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            if (!(obj is LocalIpData data)) {
                return false;
            }
            return this.ToString() == data.ToString();
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

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
