namespace NTMiner.MinerServer {
    public class ServerState {
        public static readonly ServerState Empty = new ServerState {
            JsonFileVersion = string.Empty,
            MinerClientVersion = string.Empty,
            Time = 0,
            MessageTimestamp = 0
        };

        public ServerState() { }

        public string JsonFileVersion { get; set; }
        public string MinerClientVersion { get; set; }
        public ulong Time { get; set; }
        public ulong MessageTimestamp { get; set; }
    }
}
