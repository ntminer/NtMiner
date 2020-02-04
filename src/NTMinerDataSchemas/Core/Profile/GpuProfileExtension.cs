namespace NTMiner.Core.Profile {
    public static class GpuProfileExtension {
        public static string ToOverClockString(this IGpuProfile gpuProfile) {
            return $"核心({gpuProfile.CoreClockDelta.ToString()}),核心电压({gpuProfile.CoreVoltage.ToString()}),显存({gpuProfile.MemoryClockDelta.ToString()}),显存电压({gpuProfile.MemoryVoltage.ToString()}),功耗({gpuProfile.PowerCapacity.ToString()}),温度({gpuProfile.TempLimit.ToString()}),风扇({gpuProfile.Cool.ToString()})";
        }
    }
}
