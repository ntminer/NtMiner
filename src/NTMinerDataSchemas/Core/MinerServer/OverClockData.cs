using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class OverClockData : IOverClockData, IDbEntity<Guid>, ISignableData {
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

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
