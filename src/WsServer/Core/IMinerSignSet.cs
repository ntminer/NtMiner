using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core {
    public interface IMinerSignSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        // 只能根据ClientId无法根据MinerId，因为MinerId是服务端的Id不在客户端。
        bool TryGetByClientId(Guid clientId, out MinerSign minerSign);
    }
}
