namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public NVIDIAOverClock() {
        }

        public void SetCoreClock(IGpuOverClockData data) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} gclk:{data.CoreClockDelta}");
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} mclk:{data.MemoryClockDelta}");
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} pcap:{data.PowerCapacity}");
        }

        public void SetCool(IGpuOverClockData data) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} cool:{data.Cool}");
        }
    }
}
