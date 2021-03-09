using NTMiner.Core;

namespace NTMiner.Gpus {
    public interface IGpu : IGpuStaticData, IOverClockInput {
        int Temperature { get; }
        int MemTemperature { get; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; }
    }
}
