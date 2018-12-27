using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelSet : IEnumerable<IKernel> {
        int Count { get; }
        bool Contains(Guid kernelId);
        bool TryGetKernel(Guid kernelId, out IKernel kernel);
        void Pick(Guid kernelId, ref string input, IMineContext mineContext);
    }
}
