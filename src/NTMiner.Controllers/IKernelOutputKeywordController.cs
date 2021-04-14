using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IKernelOutputKeywordController {
        KernelOutputKeywordsResponse KernelOutputKeywords(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase ClearByExceptedOutputIds(DataRequest<Guid[]> request);
    }
}
