using System.Linq;

namespace NTMiner.Core.Gpus {
    public static class GpuSetExtension {
        public static bool Has20NCard(this IGpuSet gpuSet) {
            if (gpuSet.GpuType == GpuType.NVIDIA) {
                if (gpuSet.Any(a => Is20NCard(a.Name))) {
                    return true;
                }
            }
            return false;
        }
        private static bool Is20NCard(string cardName) {
            if (string.IsNullOrEmpty(cardName)) {
                return false;
            }
            string[] nv20Cards = new string[] { "2060", "2070", "2080" };
            foreach (var nv20 in nv20Cards) {
                if (cardName.Contains(nv20)) {
                    return true;
                }
            }
            return false;
        }
    }
}
