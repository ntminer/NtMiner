using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelInputSet {
        bool Contains(Guid id);
        bool TryGetKernelInput(Guid id, out IKernelInput kernelInput);
        IEnumerable<IKernelInput> AsEnumerable();
    }
}
