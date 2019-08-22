using System;

namespace NTMiner.Core.Gpus.Impl {
    public static class GpuExtensions {
        public static int GetOverClockId(this IGpu gpu) {
            if (NTMinerRoot.Instance.GpuSet.GpuType != GpuType.NVIDIA) {
                return gpu.Index;
            }
            if (int.TryParse(gpu.BusId, out int busId)) {
                return busId;
            }
            throw new FormatException("BusId的格式必须是数字");
        }
    }
}
