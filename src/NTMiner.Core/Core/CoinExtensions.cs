using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core {
    public static class CoinExtensions {
        public static Guid GetKernelBrandId(this ICoin coin, GpuType targetGpuType) {
            if (coin == null || string.IsNullOrEmpty(coin.KernelBrand)) {
                return Guid.Empty;
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

        /// <summary>
        /// 选择默认内核
        /// </summary>
        /// <param name="coin"></param>
        /// <returns></returns>
        public static Guid GetDefaultCoinKernelId(this ICoin coin) {
            var root = NTMinerRoot.Instance;
            Guid coinKernelId = Guid.Empty;
            SupportedGpu gpuType;
            bool noneGpu = false;
            switch (root.GpuSet.GpuType) {
                case GpuType.Empty:
                    noneGpu = true;
                    gpuType = SupportedGpu.NVIDIA;
                    break;
                case GpuType.NVIDIA:
                    gpuType = SupportedGpu.NVIDIA;
                    break;
                case GpuType.AMD:
                    gpuType = SupportedGpu.AMD;
                    break;
                default:
                    gpuType = SupportedGpu.Both;
                    break;
            }
            List<ICoinKernel> coinKernels;
            if (noneGpu) {
                coinKernels = root.CoinKernelSet.Where(a => a.CoinId == coin.GetId()).ToList();
            }
            else {
                coinKernels = root.CoinKernelSet.Where(a => a.CoinId == coin.GetId() && (a.SupportedGpu == SupportedGpu.Both || a.SupportedGpu == gpuType)).ToList();
            }
            var items = new List<Tuple<Guid, IKernel>>(coinKernels.Count);
            foreach (var item in coinKernels) {
                if (root.KernelSet.TryGetKernel(item.KernelId, out IKernel kernel)) {
                    items.Add(new Tuple<Guid, IKernel>(item.GetId(), kernel));
                }
            }
            items = items.OrderBy(a => a.Item2.Code).ThenByDescending(a => a.Item2.Version).ToList();
            Guid kernelBrandId = GetKernelBrandId(coin, root.GpuSet.GpuType);
            if (kernelBrandId == Guid.Empty) {
                coinKernelId = items.Select(a => a.Item1).FirstOrDefault();
            }
            else {
                coinKernelId = items.Where(a => a.Item2.BrandId == kernelBrandId).Select(a => a.Item1).FirstOrDefault();
            }
            return coinKernelId;
        }
    }
}
