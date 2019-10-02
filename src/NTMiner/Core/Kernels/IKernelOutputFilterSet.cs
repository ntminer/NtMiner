using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputFilterSet : IEnumerable<IKernelOutputFilter> {
        bool Contains(Guid kernelOutputFilterId);
        bool TryGetKernelOutputFilter(Guid kernelOutputFilterId, out IKernelOutputFilter kernelOutputFilter);
        IEnumerable<IKernelOutputFilter> GetKernelOutputFilters(Guid kernelOutputId);
        void Filter(Guid kernelOutputId, ref string input);
    }
}
