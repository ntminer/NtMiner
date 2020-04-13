namespace NTMiner.Core.MinerClient {
    public class LocalIpInput : LocalIpDto, ILocalIp {
        public static LocalIpInput Create(ILocalIp data, bool isAutoDNSServer) {
            return new LocalIpInput {
                DefaultIPGateway = data.DefaultIPGateway,
                DHCPEnabled = data.DHCPEnabled,
                DNSServer0 = data.DNSServer0,
                DNSServer1 = data.DNSServer1,
                IPAddress = data.IPAddress,
                IPSubnet = data.IPSubnet,
                MACAddress = data.MACAddress,
                Name = data.Name,
                SettingID = data.SettingID,
                IsAutoDNSServer = isAutoDNSServer
            };
        }

        public LocalIpInput() { }

        public bool IsAutoDNSServer { get; set; }
    }
}
