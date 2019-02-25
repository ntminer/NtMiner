namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public IGpuOverClockData Data { get; private set; }

        public NVIDIAOverClock(IGpuOverClockData data) {
            this.Data = data;
        }

        public void SetCoreClock() {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{this.Data.Index} gclk:{this.Data.CoreClockDelta}");
        }

        public void SetMemoryClock() {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{this.Data.Index} mclk:{this.Data.MemoryClockDelta}");
        }

        public void SetPowerCapacity() {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{this.Data.Index} pcap:{this.Data.PowerCapacity}");
        }

        public void SetCool() {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{this.Data.Index} cool:{this.Data.Cool}");
        }
    }
}
