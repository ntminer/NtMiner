namespace NTMiner.Core.Gpus {
    /// <summary>
    /// GpuNameCount是挖矿端上报到服务端的显卡的原始名。而<see cref="GpuName"/>是管理员人脑基于GpuNameCount集提取的特征名。
    /// </summary>
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
