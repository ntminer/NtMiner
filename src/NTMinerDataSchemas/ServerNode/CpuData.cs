namespace NTMiner.ServerNode {
    public class CpuData : ICpuData {
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
