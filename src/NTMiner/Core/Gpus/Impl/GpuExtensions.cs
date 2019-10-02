using NTMiner.Gpus;
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

        public static void UpdateState(this IGpu gpu, OverClockRange range) {
            gpu.CoreClockDeltaMin = range.CoreClockMin;
            gpu.CoreClockDeltaMax = range.CoreClockMax;
            gpu.CoreClockDelta = range.CoreClockDelta;
            gpu.CoreVoltage = range.CoreVoltage;

            gpu.MemoryClockDeltaMin = range.MemoryClockMin;
            gpu.MemoryClockDeltaMax = range.MemoryClockMax;
            gpu.MemoryClockDelta = range.MemoryClockDelta;
            gpu.MemoryVoltage = range.MemoryVoltage;

            gpu.TempLimitMin = range.TempLimitMin;
            gpu.TempLimitMax = range.TempLimitMax;
            gpu.TempLimit = range.TempCurr;
            gpu.TempLimitDefault = range.TempLimitDefault;

            gpu.PowerMin = range.PowerMin;
            gpu.PowerMax = range.PowerMax;
            gpu.PowerCapacity = range.PowerCurr;
            gpu.PowerDefault = range.PowerDefault;

            gpu.CoolMin = range.FanSpeedMin;
            gpu.CoolMax = range.FanSpeedMax;
            NTMinerRoot.Instance.GpuSet.LoadGpuState(gpu.Index);
        }
    }
}
