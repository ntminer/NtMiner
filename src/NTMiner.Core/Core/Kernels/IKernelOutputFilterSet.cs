using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputFilterSet : IEnumerable<IKernelOutputFilter> {
        bool Contains(Guid kernelOutputFilterId);
        bool TryGetKernelOutputFilter(Guid kernelOutputFilterId, out IKernelOutputFilter kernelOutputFilter);
        IEnumerable<IKernelOutputFilter> GetKernelOutputFilters(Guid kernelId);
        void Filter(Guid kernelId, ref string input);
    }
}
