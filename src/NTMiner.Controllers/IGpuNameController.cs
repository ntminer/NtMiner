using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IGpuNameController {
        DataResponse<List<GpuName>> QueryGpuNames(QueryGpuNamesRequest request);
        DataResponse<List<GpuNameCount>> QueryGpuNameCounts(QueryGpuNameCountsRequest request);
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
