namespace NTMiner.Core.Gpus {
    public interface IGpuName {
        string Name { get; }
        /// <summary>
        /// 单位Byte
        /// </summary>
        ulong TotalMemory { get; set; }
    }
}
