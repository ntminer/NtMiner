using System.Linq;

namespace NTMiner.Core {
    public static class CoinExtensions {
        public static bool IsSupported(this ICoin coin) {
            foreach (var coinKernel in NTMinerRoot.Current.CoinKernelSet.Where(a => a.CoinId == coin.GetId())) {
                if (coinKernel.SupportedGpu == SupportedGpu.Both) {
                    return true;
                }
                if (coinKernel.SupportedGpu == SupportedGpu.NVIDIA && NTMinerRoot.Current.GpuSet.GpuType == GpuType.NVIDIA) {
                    return true;
                }
                if (coinKernel.SupportedGpu == SupportedGpu.AMD && NTMinerRoot.Current.GpuSet.GpuType == GpuType.AMD) {
                    return true;
                }
            }

            return false;
        }
    }
}
