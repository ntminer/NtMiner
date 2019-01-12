using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputSet : IEnumerable<IKernelOutput> {
        List<IKernelOutputPicker> GetKernelOutputPickers(Guid kernelOutputId);
        List<IKernelOutputTranslater> GetKernelOutputTranslaters(Guid kernelOutputId);
        List<IKernelOutputFilter> GetKernelOutputFilters(Guid kernelOutputId);
    }
}
