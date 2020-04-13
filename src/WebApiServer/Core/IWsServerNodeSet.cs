using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWsServerNodeSet {
        string GetTargetNode(Guid clientId);
        void SetNodeState(WsServerNodeState data);
        void RemoveNode(string address);
        IEnumerable<WsServerNodeState> AsEnumerable();
    }
}
