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
            if (range.CoreClockMin != 0) {
                gpu.CoreClockDeltaMin = range.CoreClockMin;
            }
            if (range.CoreClockMax != 0) {
                gpu.CoreClockDeltaMax = range.CoreClockMax;
            }
            if (range.CoreClockDelta != 0) {
                gpu.CoreClockDelta = range.CoreClockDelta;
            }
            if (range.CoreVoltage != 0) {
                gpu.CoreVoltage = range.CoreVoltage;
            }

            if (range.MemoryClockMin != 0) {
                gpu.MemoryClockDeltaMin = range.MemoryClockMin;
            }
            if (range.MemoryClockMax != 0) {
                gpu.MemoryClockDeltaMax = range.MemoryClockMax;
            }
            if (range.MemoryClockDelta != 0) {
                gpu.MemoryClockDelta = range.MemoryClockDelta;
            }
            if (range.MemoryVoltage != 0) {
                gpu.MemoryVoltage = range.MemoryVoltage;
            }

            if (range.TempLimitMin != 0) {
                gpu.TempLimitMin = range.TempLimitMin;
            }
            if (range.TempLimitMax != 0) {
                gpu.TempLimitMax = range.TempLimitMax;
            }
            if (range.TempCurr != 0) {
                gpu.TempLimit = range.TempCurr;
            }
            if (range.TempLimitDefault != 0) {
                gpu.TempLimitDefault = range.TempLimitDefault;
            }

            if (range.PowerMin != 0) {
                gpu.PowerMin = range.PowerMin;
            }
            if (range.PowerMax != 0) {
                gpu.PowerMax = range.PowerMax;
            }
            if (range.PowerCurr != 0) {
                gpu.PowerCapacity = range.PowerCurr;
            }
            if (range.PowerDefault != 0) {
                gpu.PowerDefault = range.PowerDefault;
            }

            if (range.FanSpeedMin != 0) {
                gpu.CoolMin = range.FanSpeedMin;
            }
            if (range.FanSpeedMax != 0) {
                gpu.CoolMax = range.FanSpeedMax;
            }
            NTMinerRoot.Instance.GpuSet.LoadGpuState(gpu.Index);
        }
    }
}
