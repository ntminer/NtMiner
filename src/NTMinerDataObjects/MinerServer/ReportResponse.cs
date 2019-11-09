namespace NTMiner.MinerServer {
    public class ReportResponse : ResponseBase {
        public ReportResponse() { }

        public static ReportResponse Ok(ServerState serverState) {
            return new ReportResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                ServerState = serverState
            };
        }

        public ServerState ServerState { get; set; }
    }
}
