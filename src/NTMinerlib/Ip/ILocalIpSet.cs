using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Ip {
    public interface ILocalIpSet : IEnumerable<ILocalIp> {
        void Refresh();
        void SetIp(ILocalIp data, bool isAutoDNSServer);
    }
}
