using System.Collections.Generic;

namespace NTMiner.ServerNode {
    public interface IWebApiServerState : IServerState {
        List<WsServerNodeState> WsServerNodes { get; }
    }
}
