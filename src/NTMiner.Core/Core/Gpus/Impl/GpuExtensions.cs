using System;

namespace NTMiner.Core.Gpus.Impl {
    public static class GpuExtensions {
        public static int GetBusId(this IGpu gpu) {
            if (int.TryParse(gpu.BusId, out int busId)) {
                return busId;
            }
            throw new FormatException("BusId的格式必须是数字");
        }
    }
}
