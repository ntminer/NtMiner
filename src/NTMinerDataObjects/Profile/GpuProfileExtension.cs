namespace NTMiner.Profile {
    public static class GpuProfileExtension {
        public static string ToOverClockString(this IGpuProfile gpuProfile) {
            return $"核心({gpuProfile.CoreClockDelta}),核心电压({gpuProfile.CoreVoltage}),显存({gpuProfile.MemoryClockDelta}),显存电压({gpuProfile.MemoryVoltage}),功耗({gpuProfile.PowerCapacity}),温度({gpuProfile.TempLimit}),风扇({gpuProfile.Cool})";
        }
    }
}
