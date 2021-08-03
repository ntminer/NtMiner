namespace NTMiner.Gpus {
    public static class GpuTypeExtensions {
        public static bool IsNvidia(this GpuType gpuType) {
            return gpuType == GpuType.NVIDIA;
        }

        public static bool IsAmd(this GpuType gpuType) {
            return gpuType == GpuType.AMD;
        }

        public static bool IsEmpty(this GpuType gpuType) {
            return gpuType == GpuType.Empty;
        }
    }
}
