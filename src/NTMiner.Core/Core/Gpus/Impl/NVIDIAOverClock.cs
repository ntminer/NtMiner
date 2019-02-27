namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public NVIDIAOverClock() {
        }

        public void SetCoreClock(IGpuOverClockData data) {
            int value = data.CoreClockDelta;
            if (value < -400) {
                value = -400;
            }
            else if (value > 400) {
                value = 400;
            }
            value = 1000 * value;
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} gclk:{value}");
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            int value = data.MemoryClockDelta;
            if (value < -1000) {
                value = -1000;
            }
            else if (value > 1000) {
                value = 1000;
            }
            value = 1000 * value;
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} mclk:{value}");
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            int value = data.PowerCapacity;
            if (value == 0) {
                return;
            }
            if (value < 50) {
                value = 50;
            }
            else if (value > 110) {
                value = 110;
            }
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} pcap:{value}");
        }

        public void SetCool(IGpuOverClockData data) {
            int value = data.Cool;
            if (value == 0) {
                return;
            }
            if (value < 38) {
                value = 38;
            }
            else if (value > 100) {
                value = 100;
            }
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} cool:{value}");
        }
    }
}
