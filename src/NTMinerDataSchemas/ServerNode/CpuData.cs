namespace NTMiner.ServerNode {
    public class CpuData : ICpuData {
        public static readonly CpuData Empty = new CpuData {
            ClockSpeed = string.Empty,
            Identifier = string.Empty,
            Name = string.Empty,
            NumberOfLogicalCores = 0,
            ProcessorArchitecture = string.Empty,
            ProcessorLevel = string.Empty,
            VendorIdentifier = string.Empty
        };

        public CpuData() {
        }

        public string Name { get; set; }
        public int NumberOfLogicalCores { get; set; }
        public string ClockSpeed { get; set; }
        public string Identifier { get; set; }
        public string VendorIdentifier { get; set; }
        public string ProcessorLevel { get; set; }
        public string ProcessorArchitecture { get; set; }
    }
}
