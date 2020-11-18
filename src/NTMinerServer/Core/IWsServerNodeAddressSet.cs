using NTMiner.ServerNode;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWsServerNodeAddressSet {
        void Init(Action callback = null);
        WsStatus WsStatus { get; }
        string GetTargetNode(Guid clientId);
        IEnumerable<string> AsEnumerable();
    }
}
