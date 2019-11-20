using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Net {
    public interface ILocalIpSet {
        void InitOnece();
        IEnumerable<ILocalIp> AsEnumerable();
    }
}
