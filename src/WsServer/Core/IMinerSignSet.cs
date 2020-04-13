using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Core {
    public interface IMinerSignSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        bool TryGetByClientId(Guid clientId, out MinerSign minerSign);
        bool TryGetByMinerId(string minerId, out MinerSign minerSign);
    }
}
