using NTMiner.Core.Gpus;

namespace NTMiner.Controllers {
    public interface IGpuNameController {
        QueryGpuNamesResponse QueryGpuNames(QueryGpuNamesRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        QueryGpuNameCountsResponse QueryGpuNameCounts(QueryGpuNameCountsRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetGpuName(DataRequest<GpuName> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveGpuName(DataRequest<GpuName> request);
    }
}
