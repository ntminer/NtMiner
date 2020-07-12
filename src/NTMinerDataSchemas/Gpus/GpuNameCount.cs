namespace NTMiner.Gpus {
    /// <summary>
    /// GpuNameCount是挖矿端上报到服务端的显卡的原始名。而<see cref="GpuName"/>是管理员人脑基于GpuNameCount集提取的特征名。
    /// </summary>
    public class GpuNameCount : IGpuNameCount {
        public GpuNameCount() { }

        public GpuType GpuType { get; set; }

        /// <summary>
        /// 注意：匹配名称的时候注意按照名称长短的顺序由长到短运算，就是说先用5700关键字匹配再用570关键字匹配。
        /// </summary>
        public string Name { get; set; }

        public ulong TotalMemory { get; set; }

        public int Count { get; set; }
    }
}
