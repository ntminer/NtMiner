using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.KernelOutputKeyword {
    // TODO:因为ServerKernelOutputKeywordSet的数据源异步来自于服务器，考虑是否应该有个IsReady属性
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        bool Contains(Guid kernelOutputId, string keyword);
        IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword);
    }
}
