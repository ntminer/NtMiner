using System;

namespace NTMiner.ServerNode {
    /// <summary>
    /// 这是返回给挖矿端的服务端状态模型
    /// </summary>
    public class ServerStateResponse : IServerStateResponse {
        public static readonly ServerStateResponse Empty = new ServerStateResponse {
            JsonFileVersion = string.Empty,
            MinerClientVersion = string.Empty,
            Time = 0,
            MessageTimestamp = 0,
            OutputKeywordTimestamp = 0,
            WsStatus = WsStatus.Undefined,
            NeedReClientId = false
        };

        public static ServerStateResponse FromLine(string line) {
            string jsonFileVersion = string.Empty;
            string minerClientVersion = string.Empty;
            long time = Timestamp.GetTimestamp();
            long messageTimestamp = 0;
            long outputKeywordTimestamp = 0;
            WsStatus wsStatus = WsStatus.Undefined;
            bool needReClientId = false;
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
                    long.TryParse(parts[4], out outputKeywordTimestamp);
                }
                if (parts.Length > 5) {
                    Enum.TryParse(parts[5], out wsStatus);
                }
                if (parts.Length > 6) {
                    bool.TryParse(parts[6], out needReClientId);
                }
            }
            return new ServerStateResponse {
                JsonFileVersion = jsonFileVersion,
                MinerClientVersion = minerClientVersion,
                Time = time,
                MessageTimestamp = messageTimestamp,
                OutputKeywordTimestamp = outputKeywordTimestamp,
                WsStatus = wsStatus,
                NeedReClientId = needReClientId
            };
        }

        public ServerStateResponse() { }

        public string ToLine() {
            return $"{JsonFileVersion}|{MinerClientVersion}|{Time.ToString()}|{MessageTimestamp.ToString()}|{OutputKeywordTimestamp.ToString()}|{WsStatus.ToString()}|{NeedReClientId.ToString()}";
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
        /// <see cref="ServerNode.WsStatus"/>
        /// </summary>
        public WsStatus WsStatus { get; set; }
        /// <summary>
        /// <see cref="IServerStateResponse.NeedReClientId"/>
        /// </summary>
        public bool NeedReClientId { get; set; }
    }
}
