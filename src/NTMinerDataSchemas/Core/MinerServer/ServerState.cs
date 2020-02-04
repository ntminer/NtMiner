namespace NTMiner.Core.MinerServer {
    public class ServerState {
        public static readonly ServerState Empty = new ServerState {
            JsonFileVersion = string.Empty,
            MinerClientVersion = string.Empty,
            Time = 0,
            MessageTimestamp = 0,
            OutputKeywordTimestamp = 0
        };

        public ServerState() { }

        public string JsonFileVersion { get; set; }
        public string MinerClientVersion { get; set; }
        /// <summary>
        /// 服务器当前时间
        /// </summary>
        public ulong Time { get; set; }
        /// <summary>
        /// 服务端消息时间戳
        /// </summary>
        public ulong MessageTimestamp { get; set; }
        /// <summary>
        /// 内核输出关键字时间戳
        /// </summary>
        public ulong OutputKeywordTimestamp { get; set; }
    }
}
