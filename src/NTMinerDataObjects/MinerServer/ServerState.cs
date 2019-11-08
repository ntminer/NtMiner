using System;

namespace NTMiner.MinerServer {
    public class ServerState {
        public string JsonFileVersion { get; set; }
        public string MinerClientVersion { get; set; }
        public ulong Time { get; set; }
        public ulong MessageTimestamp { get; set; }

        public DateTime GetTime() {
            return Timestamp.FromTimestamp(Time);
        }

        public DateTime GetMessageTImestamp {
            get {
                return Timestamp.FromTimestamp(MessageTimestamp);
            }
        }
    }
}
