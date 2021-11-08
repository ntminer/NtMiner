using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core {
    public interface IMinerSignSet : IRedisLazySet {
        // 只能根据ClientId无法根据MinerId，因为MinerId是服务端的Id不在客户端。
        bool TryGetByClientId(Guid clientId, out MinerSign minerSign);
        void SetMinerSign(MinerSign minerSign);
    }
}
