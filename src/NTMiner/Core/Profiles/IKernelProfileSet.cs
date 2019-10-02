using System;

namespace NTMiner.Core.Profiles {
    public interface IKernelProfileSet {
        IKernelProfile EmptyKernelProfile { get; }

        IKernelProfile GetKernelProfile(Guid kernelId);
    }
}
