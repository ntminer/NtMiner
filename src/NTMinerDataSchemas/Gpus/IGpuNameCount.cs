namespace NTMiner.Gpus {
    public interface IGpuNameCount : ICountSet {
        GpuType GpuType { get; }
        string Name { get; }
        /// <summary>
        /// 单位Byte
        /// </summary>
        ulong TotalMemory { get; set; }
    }
}
