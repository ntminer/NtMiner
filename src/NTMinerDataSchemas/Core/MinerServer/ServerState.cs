using System;

namespace NTMiner.Core.MinerServer {
    public class ServerState {
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
        /// server.json版本
        /// </summary>
        public string JsonFileVersion { get; set; }
        /// <summary>
        /// 可升级到的最新的挖矿端客户端版本
        /// </summary>
        public string MinerClientVersion { get; set; }
        /// <summary>
        /// 服务器当前时间
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// 服务端消息时间戳
        /// </summary>
        public long MessageTimestamp { get; set; }
        /// <summary>
        /// 内核输出关键字时间戳
        /// </summary>
        public long OutputKeywordTimestamp { get; set; }
        /// <summary>
        /// <see cref="MinerServer.WsStatus"/>
        /// </summary>
        public WsStatus WsStatus { get; set; }
    }
}
