using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        string Description { get; }
        int Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }
    }
}
