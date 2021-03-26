using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputSet {
        bool Contains(Guid id);
        bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput);
        IEnumerable<IKernelOutput> AsEnumerable();
    }
}
