using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWsServerNodeSet {
        WsStatus WsStatus { get; }
        string GetTargetNode(Guid clientId);
        void SetNodeState(WsServerNodeState data);
        void RemoveNode(string address);
        IEnumerable<WsServerNodeState> AsEnumerable();
    }
}
