using System.Collections.Generic;

namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给群控客户端的服务端状态模型，是只有admin才能看到的服务端状态信息。
    /// </summary>
    public class WebApiServerState : IWebApiServerState {
        public WebApiServerState() {
            this.WsServerNodes = new List<WsServerNodeState>();
            this.Cpu = new CpuData();// 注意这里不能指向一个静态对象，因为反序列化貌似不会赋值整个复杂类型的属性而是赋值该属性的属性
        }

        public string Address { get; set; }

        public string Description { get; set; }

        public List<WsServerNodeState> WsServerNodes { get; set; }

        public string OSInfo { get; set; }
        public CpuData Cpu { get; set; }

        public ulong TotalPhysicalMemory { get; set; }

        public double CpuPerformance { get; set; }

        public ulong AvailablePhysicalMemory { get; set; }

        public double ProcessMemoryMb { get; set; }
        public long ThreadCount { get; set; }
        public long HandleCount { get; set; }
        public string AvailableFreeSpaceInfo { get; set; }
        public int CaptchaCount { get; set; }
    }
}
