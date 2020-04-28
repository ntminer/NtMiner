namespace NTMiner.Core.MinerServer {
    public class GetWorkJsonResponse : ResponseBase {
        public GetWorkJsonResponse() {
        }

        public static GetWorkJsonResponse Ok(string localJson, string serverJson, string workerName) {
            return new GetWorkJsonResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                WorkerName = workerName,
                LocalJson = localJson,
                ServerJson = serverJson
            };
        }

        public string WorkerName { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }
    }
}
