namespace NTMiner.Core.MinerServer {
    public class QueryClientsForWsRequest : QueryClientsRequest {
        public QueryClientsForWsRequest() { }

        public string LoginName { get; set; }
    }
}
