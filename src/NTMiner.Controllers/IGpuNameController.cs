using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IGpuNameController {
        DataResponse<List<GpuName>> GpuNames(object request);
        DataResponse<List<GpuNameCount>> GpuNameCounts(object request);
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
