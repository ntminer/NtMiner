using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {

    public static class GpuSpeedExtension {
        public static List<IGpuSpeed> GetGpuSpeedHistory(this IGpu gpu) {
            return NTMinerRoot.Instance.GpusSpeed.GetGpuSpeedHistory(gpu.Index);
        }

        public static GpuSpeedData ToGpuSpeedData(this IGpuSpeed data) {
            return new GpuSpeedData {
                Index = data.Gpu.Index,
                Name = data.Gpu.Name,
                TotalMemory = data.Gpu.TotalMemory,
                MainCoinSpeed = data.MainCoinSpeed.Value,
                DualCoinSpeed = data.DualCoinSpeed.Value,
                AcceptShare = data.MainCoinSpeed.AcceptShare,
                FoundShare = data.MainCoinSpeed.FoundShare,
                RejectShare = data.MainCoinSpeed.RejectShare,
                IncorrectShare = data.MainCoinSpeed.IncorrectShare,
                FanSpeed = data.Gpu.FanSpeed,
                Temperature = data.Gpu.Temperature,
                PowerUsage = data.Gpu.PowerUsage,
                Cool = data.Gpu.Cool,
                PowerCapacity = data.Gpu.PowerCapacity,
                CoreClockDelta = data.Gpu.CoreClockDelta,
                MemoryClockDelta = data.Gpu.MemoryClockDelta,
                TempLimit = data.Gpu.TempLimit,
                CoreVoltage = data.Gpu.CoreVoltage,
                MemoryVoltage = data.Gpu.MemoryVoltage
            };
        }
    }
}
