using NTMiner.Core;
using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IKernelOutputKeywordController {
        string GetVersion();
        DataResponse<List<KernelOutputKeywordData>> KernelOutputKeywords(KernelOutputKeywordsRequest request);
        ResponseBase SetKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
    }
}
