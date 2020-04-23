using System.Collections.Generic;

namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给群控客户端的服务端状态模型，是只有admin才能看到的服务端状态信息。
    /// </summary>
    public class WebApiServerState : IServerState {
        public WebApiServerState() {
            this.WsServerNodes = new List<WsServerNodeState>();
        }

        public string Address { get; set; }

        public string Description { get; set; }

        public int CpuPerformance { get; set; }

        public int TotalPhysicalMemoryMb { get; set; }

        public int AvailablePhysicalMemoryMb { get; set; }

        public List<WsServerNodeState> WsServerNodes { get; set; }
    }
}
