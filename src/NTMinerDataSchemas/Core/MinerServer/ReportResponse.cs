using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public class ReportResponse : ResponseBase {
        public ReportResponse() {
            this.ServerState = ServerState.Empty;
            this.NewServerMessages = new List<ServerMessageData>();
        }

        public static ReportResponse Ok(ServerState serverState) {
            return new ReportResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                ServerState = serverState
            };
        }

        public ServerState ServerState { get; set; }

        // 基于请求携带的LocalServerMessageTimestamp，当新消息条数较少时直接在响应中携带给请求端
        public List<ServerMessageData> NewServerMessages { get; set; }
    }
}
