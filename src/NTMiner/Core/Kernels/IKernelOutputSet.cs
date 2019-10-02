using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputSet : IEnumerable<IKernelOutput> {
        bool Contains(Guid id);
        bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput);
        void Pick(ref string input, IMineContext mineContext);
    }
}
