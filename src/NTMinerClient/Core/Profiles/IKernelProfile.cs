using NTMiner.Core.Kernels;
using System;

namespace NTMiner.Core.Profiles {
    public interface IKernelProfile {
        Guid KernelId { get; }
        InstallStatus InstallStatus { get; }
    }
}
