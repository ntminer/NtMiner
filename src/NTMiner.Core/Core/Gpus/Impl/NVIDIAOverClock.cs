using NTMiner.MinerClient;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public NVIDIAOverClock() {
        }

        public void SetCoreClock(IGpuProfile data) {
            int value = data.CoreClockDelta;
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

        public void SetMemoryClock(IGpuProfile data) {
            int value = data.MemoryClockDelta;
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

        public void SetPowerCapacity(IGpuProfile data) {
            int value = data.PowerCapacity;
            if (value == 0) {
                return;
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

        public void SetCool(IGpuProfile data) {
            int value = data.Cool;
            if (value == 0) {
                return;
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
                if (NTMinerRoot.Current.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    RefreshGpuState(gpu);
                }
            }
        }

        public static void RefreshGpuState(IGpu gpu) {
            const string coreClockDeltaMinMaxPattern = @"c\[0\]\.freqDelta     = (\d+) kHz \[(-?\d+) .. (\d+)\]";
            const string memoryClockDeltaMinMaxPattern = @"c\[1\]\.freqDelta     = (\d+) kHz \[(-?\d+) .. (\d+)\]";
            const string coolSpeedMinMaxPattern = @"cooler\[0\]\.speed   = (\d+) % \[(-?\d+) .. (\d+)\]";
            const string powerMinPattern = @"policy\[0\]\.pwrLimitMin     = (\d+\.?\d*) %";
            const string powerMaxPattern = @"policy\[0\]\.pwrLimitMax     = (\d+\.?\d*) %";
            const string powerLimitCurrentPattern = @"policy\[0\]\.pwrLimitCurrent = (\d+\.?\d*) %";
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            int exitCode = -1;
            string output;
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out output);
            int coreClockDeltaMin = 0;
            int coreClockDeltaMax = 0;
            int memoryClockDeltaMin = 0;
            int memoryClockDeltaMax = 0;
            int cool = 0;
            int coolMin = 0;
            int coolMax = 0;
            double powerMin = 0;
            double powerMax = 0;
            double power = 0;
            if (exitCode == 0) {
                Match match = Regex.Match(output, coreClockDeltaMinMaxPattern);
                if (match.Success) {
                    int.TryParse(match.Groups[1].Value, out int coreClockDelta);
                    gpu.CoreClockDelta = coreClockDelta;
                    int.TryParse(match.Groups[2].Value, out coreClockDeltaMin);
                    int.TryParse(match.Groups[3].Value, out coreClockDeltaMax);
                }
                match = Regex.Match(output, memoryClockDeltaMinMaxPattern);
                if (match.Success) {
                    int.TryParse(match.Groups[1].Value, out int memoryClockDelta);
                    gpu.MemoryClockDelta = memoryClockDelta;
                    int.TryParse(match.Groups[2].Value, out memoryClockDeltaMin);
                    int.TryParse(match.Groups[3].Value, out memoryClockDeltaMax);
                }
                gpu.CoreClockDeltaMin = coreClockDeltaMin;
                gpu.CoreClockDeltaMax = coreClockDeltaMax;
                gpu.MemoryClockDeltaMin = memoryClockDeltaMin;
                gpu.MemoryClockDeltaMax = memoryClockDeltaMax;
            }
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} coolers", ref exitCode, out output);
            if (exitCode == 0) {
                Match match = Regex.Match(output, coolSpeedMinMaxPattern);
                if (match.Success) {
                    int.TryParse(match.Groups[1].Value, out cool);
                    int.TryParse(match.Groups[2].Value, out coolMin);
                    int.TryParse(match.Groups[3].Value, out coolMax);
                }
                gpu.Cool = cool;
                gpu.CoolMin = coolMin;
                gpu.CoolMax = coolMax;
            }
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} pwrinfo", ref exitCode, out output);
            if (exitCode == 0) {
                Match match = Regex.Match(output, powerMinPattern);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerMin);
                }
                match = Regex.Match(output, powerMaxPattern);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerMax);
                }
                match = Regex.Match(output, powerLimitCurrentPattern);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out power);
                }
                gpu.PowerMin = powerMin;
                gpu.PowerMax = powerMax;
                gpu.Power = power;
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
