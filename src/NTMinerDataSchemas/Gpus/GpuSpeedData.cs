namespace NTMiner.Gpus {
    public class GpuSpeedData : IGpuSpeedData {
        public GpuSpeedData() {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 单位byte
        /// </summary>
        public ulong TotalMemory { get; set; }

        public double MainCoinSpeed { get; set; }

        public int FoundShare { get; set; }

        public int AcceptShare { get; set; }

        public int RejectShare { get; set; }

        public int IncorrectShare { get; set; }

        public double DualCoinSpeed { get; set; }

        public int Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
        public int CoreClockDelta { get; set; }
        public int MemoryClockDelta { get; set; }
        public int Cool { get; set; }
        public double PowerCapacity { get; set; }
        public int TempLimit { get; set; }
        public int CoreVoltage { get; set; }
        public int MemoryVoltage { get; set; }
        public bool IsRejectOneGpuShare { get; set; }
        public bool IsFoundOneGpuShare { get; set; }
        public bool IsGotOneIncorrectGpuShare { get; set; }

    }
}
