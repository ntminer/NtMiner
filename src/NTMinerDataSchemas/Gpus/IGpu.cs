using NTMiner.Core;

namespace NTMiner.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        int Temperature { get; }
        int MemoryTemperature { get; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; }
    }
}
