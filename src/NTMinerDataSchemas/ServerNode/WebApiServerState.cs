using System.Collections.Generic;

namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给群控客户端的服务端状态模型，是只有admin才能看到的服务端状态信息。
    /// </summary>
    public class WebApiServerState : IWebApiServerState {
        public WebApiServerState() {
            this.WsServerNodes = new List<WsServerNodeState>();
            this.Cpu = CpuData.Empty;
        }

        public string Address { get; set; }

        public string Description { get; set; }

        public List<WsServerNodeState> WsServerNodes { get; set; }

        public string OSInfo { get; set; }
        public CpuData Cpu { get; set; }

        public ulong TotalPhysicalMemory { get; set; }

        public double CpuPerformance { get; set; }

        public ulong AvailablePhysicalMemory { get; set; }
    }
}
