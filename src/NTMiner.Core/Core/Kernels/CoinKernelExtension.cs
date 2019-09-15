using System;

namespace NTMiner.Core.Kernels {
    public static class CoinKernelExtension {
        public static bool GetIsSupportDualMine(this ICoinKernel coinKernel) {
            if (!NTMinerRoot.Instance.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                return false;
            }
            if (!NTMinerRoot.Instance.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                return false;
            }
            if (!kernelInput.IsSupportDualMine) {
                return false;
            }
            return coinKernel.DualCoinGroupId != Guid.Empty && NTMinerRoot.Instance.GroupSet.TryGetGroup(coinKernel.DualCoinGroupId, out IGroup _);
        }
    }
}
