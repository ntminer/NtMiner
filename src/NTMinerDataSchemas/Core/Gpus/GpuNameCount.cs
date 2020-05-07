namespace NTMiner.Core.Gpus {
    public class GpuNameCount : GpuName {
        public static GpuNameCount Create(GpuName gpuName) {
            return new GpuNameCount {
                GpuType = gpuName.GpuType,
                Count = 0,
                Name = gpuName.Name,
                TotalMemory = gpuName.TotalMemory
            };
        }

        public GpuNameCount() { }

        public int Count { get; set; }
    }
}
