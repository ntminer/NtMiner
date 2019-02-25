namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        private readonly int _gpuIndex;

        public NVIDIAOverClock(int gpuIndex) {
            _gpuIndex = gpuIndex;
        }

        public void SetCoreClock(int deltaValue) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{_gpuIndex} gclk:{deltaValue}");
        }

        public void SetMemoryClock(int deltaValue) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{_gpuIndex} mclk:{deltaValue}");
        }

        public void SetPowerCapacity(int nn) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{_gpuIndex} pcap:{nn}");
        }

        public void SetCool(int nn) {
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{_gpuIndex} cool:{nn}");
        }
    }
}
