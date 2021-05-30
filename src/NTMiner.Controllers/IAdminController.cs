using NTMiner.Gpus;
using NTMiner.ServerNode;

namespace NTMiner.Controllers {
    public interface IAdminController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetClientTestId(DataRequest<ClientTestIdData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        QueryActionCountsResponse QueryActionCounts(QueryActionCountsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        QueryGpuNameCountsResponse QueryGpuNameCounts(QueryGpuNameCountsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<MqCountData[]> MqCounts();
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<MqCountData> MqCount(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<MqAppIds> MqAppIds();
        /// <summary>
        /// 需签名
        /// </summary>
        TopNRemoteIpsResponse TopNRemoteIps(DataRequest<int> request);
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<WebApiServerState> GetServerState(object request);
    }
}
