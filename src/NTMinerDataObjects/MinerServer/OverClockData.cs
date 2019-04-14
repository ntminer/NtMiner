using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class OverClockData : IOverClockData, IDbEntity<Guid>, IGetSignData {
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

        public int PowerCapacity { get; set; }

        public int TempLimit { get; set; }

        public bool IsAutoFanSpeed { get; set; }

        public int Cool { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(GpuType)).Append(GpuType)
                .Append(nameof(CoreClockDelta)).Append(CoreClockDelta)
                .Append(nameof(MemoryClockDelta)).Append(MemoryClockDelta)
                .Append(nameof(PowerCapacity)).Append(PowerCapacity)
                .Append(nameof(TempLimit)).Append(TempLimit)
                .Append(nameof(IsAutoFanSpeed)).Append(IsAutoFanSpeed)
                .Append(nameof(Cool)).Append(Cool);
            return sb;
        }
    }
}
