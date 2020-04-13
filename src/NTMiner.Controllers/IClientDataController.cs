using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    /// <summary>
    /// 群控客户端的矿机列表接口
    /// </summary>
    public interface IClientDataController {
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        ResponseBase UpdateClient(UpdateClientRequest request);
        ResponseBase UpdateClients(UpdateClientsRequest request);
        ResponseBase RemoveClients(MinerIdsRequest request);
    }
}
