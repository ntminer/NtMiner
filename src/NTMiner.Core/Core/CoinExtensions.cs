using System;

namespace NTMiner.Core {
    public static class CoinExtensions {
        public static Guid GetKernelBrandId(this ICoin coin, GpuType targetGpuType) {
            if (coin == null || string.IsNullOrEmpty(coin.KernelBrand)) {
                return Guid.Empty;
            }
            if (NTMinerRoot.IsKernelBrand) {
                return NTMinerRoot.KernelBrandId;
            }
            string[] items = coin.KernelBrand.Split(';');
            foreach (var item in items) {
                string[] kv = item.Split(':');
                if (kv.Length == 2) {
                    if (kv[0].TryParse(out GpuType gpuType) && gpuType == targetGpuType) {
                        if (Guid.TryParse(kv[1], out Guid kernelBrandId)) {
                            return kernelBrandId;
                        }
                    }
                }
            }
            return Guid.Empty;
        }
    }
}
