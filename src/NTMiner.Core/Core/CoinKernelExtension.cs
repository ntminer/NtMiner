using System;

namespace NTMiner.Core {
    public static class CoinKernelExtension {
        public static bool IsSupportDualMine(this ICoinKernel coinKernel) {
            IKernel kernel;
            if (!NTMinerRoot.Current.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                return false;
            }
            IKernelInput kernelInput;
            if (!NTMinerRoot.Current.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                return false;
            }
            if (!kernelInput.IsSupportDualMine) {
                return false;
            }
            if (coinKernel.DualCoinGroupId != Guid.Empty) {
                return true;
            }
            return coinKernel.DualCoinGroupId != Guid.Empty;
        }
    }
}
