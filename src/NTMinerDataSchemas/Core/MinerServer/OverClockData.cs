using NTMiner.Gpus;
using System;

namespace NTMiner.Core.MinerServer {
    [DataSchemaId("5E026328-8B23-467A-BC26-31E0BB0DB1FB")]
    public class OverClockData : IOverClockData, IDbEntity<Guid> {
        public OverClockData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public GpuType GpuType { get; set; }

        public string Name { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int CoreVoltage { get; set; }
        public int MemoryVoltage { get; set; }

        public int PowerCapacity { get; set; }

        public int TempLimit { get; set; }

        public bool IsAutoFanSpeed { get; set; }

        public int Cool { get; set; }

        public int CurrentMemoryTimingLevel { get; set; }
    }
}
