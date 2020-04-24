namespace NTMiner.ServerNode {
    public interface ICpuData {
        string ClockSpeed { get; }
        string Identifier { get; }
        string Name { get; }
        int NumberOfLogicalCores { get; }
        string ProcessorArchitecture { get; }
        string ProcessorLevel { get; }
        string VendorIdentifier { get; }
    }
}