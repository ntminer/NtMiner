using NTMiner.Gpus;

namespace NTMiner.Controllers {
    public interface IGpuNameController {
        /// <summary>
        /// 需签名
        /// </summary>
        QueryGpuNameCountsResponse QueryGpuNameCounts(QueryGpuNameCountsRequest request);
    }
}
