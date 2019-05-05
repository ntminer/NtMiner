using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        int Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }
        GpuStatus State { get; set; }
    }
}
