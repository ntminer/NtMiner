namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给群控客户端的服务端状态模型，是只有admin才能看到的服务端状态信息。
    /// </summary>
    public class WebApiServerState : IServerState, IServerStateResponse {
        public WebApiServerState() {
        }

        public string Address { get; set; }

        public string Description { get; set; }

        public int CpuPerformance { get; set; }

        public int TotalPhysicalMemoryMb { get; set; }

        public int AvailablePhysicalMemoryMb { get; set; }

        /// <summary>
        /// <see cref="IServerStateResponse.JsonFileVersion"/>
        /// </summary>
        public string JsonFileVersion { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.MinerClientVersion"/>
        /// </summary>
        public string MinerClientVersion { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.Time"/>
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.MessageTimestamp"/>
        /// </summary>
        public long MessageTimestamp { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.OutputKeywordTimestamp"/>
        /// </summary>
        public long OutputKeywordTimestamp { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.WsStatus"/>
        /// </summary>
        public WsStatus WsStatus { get; set; }
    }
}
