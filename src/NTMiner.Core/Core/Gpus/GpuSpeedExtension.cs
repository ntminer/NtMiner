using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {

    public static class GpuSpeedExtension {
        public static List<IGpuSpeed> GetGpuSpeedHistory(this IGpu gpu) {
            return NTMinerRoot.Current.GpusSpeed.GetGpuSpeedHistory(gpu.Index);
        }
    }
}
