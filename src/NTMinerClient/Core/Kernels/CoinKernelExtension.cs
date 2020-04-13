using System;

namespace NTMiner.Core.Kernels {
    public static class CoinKernelExtension {
        public static bool GetIsSupportDualMine(this ICoinKernel coinKernel) {
            if (!NTMinerContext.Instance.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                return false;
            }
            if (!NTMinerContext.Instance.ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                return false;
            }
            if (!kernelInput.IsSupportDualMine) {
                return false;
            }
            return coinKernel.DualCoinGroupId != Guid.Empty && NTMinerContext.Instance.ServerContext.GroupSet.TryGetGroup(coinKernel.DualCoinGroupId, out IGroup _);
        }
    }
}
