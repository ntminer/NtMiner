using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Ip {
    public interface ILocalIpSet {
        void InitOnece();
        IEnumerable<ILocalIp> AsEnumerable();
    }
}
