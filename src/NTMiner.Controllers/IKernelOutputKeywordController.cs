using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IKernelOutputKeywordController {
        DataResponse<List<KernelOutputKeywordData>> KernelOutputKeywords(KernelOutputKeywordsRequest request);
        ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
        ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request);
    }
}
