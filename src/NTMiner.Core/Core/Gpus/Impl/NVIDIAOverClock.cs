using System.Text.RegularExpressions;

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
            if (data.Index == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} gclk:{value}");
                }
            }
            else {
                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} gclk:{value}");
            }            
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
            if (data.Index == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} mclk:{value}");
                }
            }
            else {
                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} mclk:{value}");
            }
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
            if (data.Index == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} pcap:{value}");
                }
            }
            else {
                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} pcap:{value}");
            }
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
            if (data.Index == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} cool:{value}");
                }
            }
            else {
                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{data.Index} cool:{value}");
            }
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    RefreshGpuState(gpu);
                }
            }
            else {
                IGpu gpu;
                if (NTMinerRoot.Current.GpuSet.TryGetGpu(gpuIndex, out gpu)) {
                    RefreshGpuState(gpu);
                }
            }
        }

        private static void RefreshGpuState(IGpu gpu) {
            const string coreClockDeltaPatter = @"c\[0\]\.freqDelta     = (\d+) kHz";
            const string memoryClockDeltaPatter = @"c\[1\]\.freqDelta     = (\d+) kHz";
            int exitCode = -1;
            string output;
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out output);
            if (exitCode == 0) {
                Match match = Regex.Match(output, coreClockDeltaPatter);
                if (match.Success) {
                    int coreClockDelta;
                    int.TryParse(match.Groups[1].Value, out coreClockDelta);
                    gpu.CoreClockDelta = coreClockDelta;
                }
                match = Regex.Match(output, memoryClockDeltaPatter);
                if (match.Success) {
                    int memoryClockDelta;
                    int.TryParse(match.Groups[1].Value, out memoryClockDelta);
                    gpu.MemoryClockDelta = memoryClockDelta;
                }
            }
        }
    }
}
