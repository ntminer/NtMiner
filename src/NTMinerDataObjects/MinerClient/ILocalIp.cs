using System;

namespace NTMiner.MinerClient {
    public interface ILocalIp {
        Guid SettingID { get; }
        string[] DefaultIPGateway { get; }
        bool DHCPEnabled { get; }
        string DHCPServer { get; }
        string[] IPAddress { get; }
        string[] DNSServerSearchOrder { get; }
    }
}
