using System;

namespace NTMiner.Core.MinerServer {
    public class QueryClientsResponseEx : QueryClientsResponse {
        public QueryClientsResponseEx() { }

        public static QueryClientsResponseEx Create(
            QueryClientsResponse response,
            string loginName, Guid studioId, string sessionId) {
            return new QueryClientsResponseEx() {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                Data = response.Data,
                Total = response.Total,
                LatestSnapshots = response.LatestSnapshots,
                TotalMiningCount = response.TotalMiningCount,
                TotalOnlineCount = response.TotalOnlineCount,
                LoginName = loginName,
                StudioId = studioId,
                SessionId = sessionId
            };
        }

        public string LoginName { get; set; }
        public Guid StudioId { get; set; }
        public string SessionId { get; set; }
    }
}
