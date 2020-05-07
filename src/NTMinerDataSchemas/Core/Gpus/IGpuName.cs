namespace NTMiner.Core.Gpus {
    public interface IGpuName {
        GpuType GpuType { get; }
        string Name { get; }
        /// <summary>
        /// 单位Byte
        /// </summary>
        ulong TotalMemory { get; set; }
        bool IsValid();
    }
}
