using System;

namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给挖矿端的服务端状态模型
    /// </summary>
    public class ServerState : IServerStateResponse {
        public static readonly ServerState Empty = new ServerState {
            JsonFileVersion = string.Empty,
            MinerClientVersion = string.Empty,
            Time = 0,
            MessageTimestamp = 0,
            OutputKeywordTimestamp = 0,
            WsStatus = WsStatus.Undefined
        };

        public static ServerState FromLine(string line) {
            string jsonFileVersion = string.Empty;
            string minerClientVersion = string.Empty;
            long time = Timestamp.GetTimestamp();
            long messageTimestamp = 0;
            long kernelOutputKeywordTimestamp = 0;
            WsStatus wsStatus = WsStatus.Undefined;
            if (!string.IsNullOrEmpty(line)) {
                line = line.Trim();
                string[] parts = line.Split(new char[] { '|' });
                if (parts.Length > 0) {
                    jsonFileVersion = parts[0];
                }
                if (parts.Length > 1) {
                    minerClientVersion = parts[1];
                }
                if (parts.Length > 2) {
                    long.TryParse(parts[2], out time);
                }
                if (parts.Length > 3) {
                    long.TryParse(parts[3], out messageTimestamp);
                }
                if (parts.Length > 4) {
                    long.TryParse(parts[4], out kernelOutputKeywordTimestamp);
                }
                if (parts.Length > 5) {
                    Enum.TryParse(parts[5], out wsStatus);
                }
            }
            return new ServerState {
                JsonFileVersion = jsonFileVersion,
                MinerClientVersion = minerClientVersion,
                Time = time,
                MessageTimestamp = messageTimestamp,
                OutputKeywordTimestamp = kernelOutputKeywordTimestamp,
                WsStatus = wsStatus
            };
        }

        public ServerState() { }

        public string ToLine() {
            return $"{JsonFileVersion}|{MinerClientVersion}|{Time.ToString()}|{MessageTimestamp.ToString()}|{OutputKeywordTimestamp.ToString()}|{WsStatus.ToString()}";
        }

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
