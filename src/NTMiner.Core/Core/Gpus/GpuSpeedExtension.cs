using System.Collections.Generic;

namespace NTMiner.Core.Gpus {

    public static class GpuSpeedExtension {
        public static List<IGpuSpeed> GetGpuSpeedHistory(this IGpu gpu) {
            return NTMinerRoot.Instance.GpusSpeed.GetGpuSpeedHistory(gpu.Index);
        }
    }
}
