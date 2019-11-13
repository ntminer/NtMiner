using NTMiner.Core;
using NTMiner.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IKernelOutputKeywordController {
        KernelOutputKeywordsResponse KernelOutputKeywords(KernelOutputKeywordsRequest request);
        ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
        ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request);
    }
}
