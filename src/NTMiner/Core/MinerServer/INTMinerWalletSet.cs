using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    // TODO:因为NTMinerWalletSet的数据源异步来自于服务器，考虑是否应该有个IsReady属性
    public interface INTMinerWalletSet : IEnumerable<INTMinerWallet> {
        bool TryGetNTMinerWallet(Guid id, out INTMinerWallet ntMinerWallet);
    }
}
