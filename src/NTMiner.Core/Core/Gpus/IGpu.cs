namespace NTMiner.Core.Gpus {
    public interface IGpu {
        int Index { get; }
        string Name { get; }
        uint Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }
    }
}
