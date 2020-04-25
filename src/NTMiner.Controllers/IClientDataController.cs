using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    /// <summary>
    /// 群控客户端的矿机列表接口
    /// </summary>
    public interface IClientDataController {
        /// <summary>
        /// 需签名
        /// </summary>
        QueryClientsResponse QueryClients(QueryClientsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase UpdateClient(UpdateClientRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase UpdateClients(UpdateClientsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveClients(MinerIdsRequest request);
    }
}
