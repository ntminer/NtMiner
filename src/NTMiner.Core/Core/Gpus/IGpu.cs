using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        uint Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }
    }
}
