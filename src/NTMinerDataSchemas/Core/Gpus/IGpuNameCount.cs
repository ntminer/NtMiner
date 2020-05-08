namespace NTMiner.Core.Gpus {
    public interface IGpuNameCount {
        GpuType GpuType { get; }
        string Name { get; }
        /// <summary>
        /// 单位Byte
        /// </summary>
        ulong TotalMemory { get; set; }
        int Count { get; }
    }
}
