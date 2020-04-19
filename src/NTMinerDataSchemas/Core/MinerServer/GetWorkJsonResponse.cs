namespace NTMiner.Core.MinerServer {
    public class GetWorkJsonResponse : ResponseBase {
        public GetWorkJsonResponse() {
        }

        public static GetWorkJsonResponse Ok(string localJson, string serverJson) {
            return new GetWorkJsonResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                LocalJson = localJson,
                ServerJson = serverJson
            };
        }

        public string LocalJson { get; set; }
        public string ServerJson { get; set; }
    }
}
