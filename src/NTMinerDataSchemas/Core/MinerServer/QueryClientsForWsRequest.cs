using System;

namespace NTMiner.Core.MinerServer {
    public class QueryClientsForWsRequest : QueryClientsRequest {
        public static QueryClientsForWsRequest Create(QueryClientsRequest request, string loginName, Guid studioId, string sessionId) {
            return new QueryClientsForWsRequest {
                Coin = request.Coin,
                GpuDriver = request.GpuDriver,
                GpuName = request.GpuName,
                GpuType = request.GpuType,
                GroupId = request.GroupId,
                Kernel = request.Kernel,
                MinerIp = request.MinerIp,
                MinerName = request.MinerName,
                MineState = request.MineState,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Pool = request.Pool,
                SortDirection = request.SortDirection,
                SortField = request.SortField,
                Version = request.Version,
                Wallet = request.Wallet,
                WorkId = request.WorkId,
                LoginName = loginName,
                StudioId = studioId,
                SessionId = sessionId
            };
        }

        public QueryClientsForWsRequest() { }

        public string LoginName { get; set; }
        public Guid StudioId { get; set; }
        public string SessionId { get; set; }
    }
}
