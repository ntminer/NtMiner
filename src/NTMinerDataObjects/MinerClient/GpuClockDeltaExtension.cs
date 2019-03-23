namespace NTMiner.MinerClient {
    public static class GpuClockDeltaExtension {
        public static string CoreClockDeltaMinMText(this IGpuClockDelta gpuClockDelta) {
            return (gpuClockDelta.CoreClockDeltaMin / 1000).ToString();
        }

        public static string CoreClockDeltaMaxMText(this IGpuClockDelta gpuClockDelta) {
            return (gpuClockDelta.CoreClockDeltaMax / 1000).ToString();
        }

        public static string MemoryClockDeltaMinMText(this IGpuClockDelta gpuClockDelta) {
            return (gpuClockDelta.MemoryClockDeltaMin / 1000).ToString();
        }

        public static string MemoryClockDeltaMaxMText(this IGpuClockDelta gpuClockDelta) {
            return (gpuClockDelta.MemoryClockDeltaMax / 1000).ToString();
        }
    }
}
