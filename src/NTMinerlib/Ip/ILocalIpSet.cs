using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Ip {
    public interface ILocalIpSet : IEnumerable<ILocalIp> {
        void Refresh();
    }
}
