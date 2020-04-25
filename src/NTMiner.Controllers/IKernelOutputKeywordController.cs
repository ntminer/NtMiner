using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IKernelOutputKeywordController {
        KernelOutputKeywordsResponse KernelOutputKeywords(object request);
        ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
        ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request);
    }
}
