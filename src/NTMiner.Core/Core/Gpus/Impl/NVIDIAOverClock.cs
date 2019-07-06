using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public class ValueItem {
            public string MethodName;
            public int GpuIndex;
            public int Value;
        }

        private readonly List<ValueItem> _values = new List<ValueItem>();
        private readonly object _locker = new object();

        public NVIDIAOverClock() {
            VirtualRoot.On<Per1SecondEvent>("应用N卡的超频设置", LogEnum.None,
                action: message => {
                    lock (_locker) {
                        var valueItem = _values.FirstOrDefault();
                        if (valueItem == null) {
                            return;
                        }
                        Process[] processes = Process.GetProcessesByName("NTMinerOverClock");
                        if (processes.Length != 0) {
                            return;
                        }
                        _values.Remove(valueItem);
                        switch (valueItem.MethodName) {
                            case nameof(SetCoreClock):
                                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{valueItem.GpuIndex} gclk:{valueItem.Value}", waitForExit: true);
                                break;
                            case nameof(SetMemoryClock):
                                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{valueItem.GpuIndex} mclk:{valueItem.Value}", waitForExit: true);
                                break;
                            case nameof(SetPowerCapacity):
                                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{valueItem.GpuIndex} pcap:{valueItem.Value}", waitForExit: true);
                                break;
                            case nameof(SetThermCapacity):
                                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{valueItem.GpuIndex} tcap:{valueItem.Value}", waitForExit: true);
                                break;
                            case nameof(SetCool):
                                Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{valueItem.GpuIndex} cool:{valueItem.Value}", waitForExit: true);
                                break;
                            default:
                                break;
                        }
                        this.RefreshGpuState(valueItem.GpuIndex);
                    }
                });
        }

        private void Queue(string methodName, int gpuIndex, int value) {
            lock (_locker) {
                var valueItem = _values.FirstOrDefault(a => a.MethodName == methodName && a.GpuIndex == gpuIndex);
                if (valueItem != null) {
                    valueItem.Value = value;
                }
                else {
                    _values.Add(new ValueItem {
                        MethodName = methodName,
                        GpuIndex = gpuIndex,
                        Value = value
                    });
                }
            }
        }

        public void SetCoreClock(int gpuIndex, int value) {
            value = 1000 * value;
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.CoreClockDelta == value) {
                        continue;
                    }
                    Queue(nameof(SetCoreClock), gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.CoreClockDelta == value) {
                    return;
                }
                Queue(nameof(SetCoreClock), gpu.Index, value);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value) {
            value = 1000 * value;
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.MemoryClockDelta == value) {
                        continue;
                    }
                    Queue(nameof(SetMemoryClock), gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.MemoryClockDelta == value) {
                    return;
                }
                Queue(nameof(SetMemoryClock), gpu.Index, value);
            }
        }

        public void SetPowerCapacity(int gpuIndex, int value) {
            if (value == 0) {
                value = 100;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.PowerCapacity == value) {
                        continue;
                    }
                    Queue(nameof(SetPowerCapacity), gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.PowerCapacity == value) {
                    return;
                }
                Queue(nameof(SetPowerCapacity), gpu.Index, value);
            }
        }

        public void SetThermCapacity(int gpuIndex, int value) {
            if (value == 0) {
                value = 83;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.TempLimit == value) {
                        continue;
                    }
                    Queue(nameof(SetThermCapacity), gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.TempLimit == value) {
                    return;
                }
                Queue(nameof(SetThermCapacity), gpu.Index, value);
            }
        }

        public void SetCool(int gpuIndex, int value) {
            if (value == 0) {
                value = 90;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.Cool == value) {
                        continue;
                    }
                    Queue(nameof(SetCool), gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.Cool == value) {
                    return;
                }
                Queue(nameof(SetCool), gpu.Index, value);
            }
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                return;
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    RefreshGpuState(gpu);
                }
            }
        }

        public void Restore() {
            SetCoreClock(NTMinerRoot.GpuAllId, 0);
            SetMemoryClock(NTMinerRoot.GpuAllId, 0);
            SetPowerCapacity(NTMinerRoot.GpuAllId, 0);
            SetThermCapacity(NTMinerRoot.GpuAllId, 0);
            SetCool(NTMinerRoot.GpuAllId, 0);
        }

        private void RefreshGpuState(IGpu gpu) {
            const string coreClockDeltaMinMaxPattern = @"c\[0\]\.freqDelta     = (-?\d+) kHz \[(-?\d+) .. (\d+)\]";
            const string memoryClockDeltaMinMaxPattern = @"c\[1\]\.freqDelta     = (-?\d+) kHz \[(-?\d+) .. (\d+)\]";
            const string coolSpeedMinMaxPattern = @"cooler\[0\]\.speed   = (\d+) % \[(-?\d+) .. (\d+)\]";
            const string powerMinPattern = @"policy\[0\]\.pwrLimitMin     = (\d+\.?\d*) %";
            const string powerMaxPattern = @"policy\[0\]\.pwrLimitMax     = (\d+\.?\d*) %";
            const string powerDefaultPattern = @"policy\[0\]\.pwrLimitDefault = (\d+\.?\d*) %";
            const string powerLimitCurrentPattern = @"policy\[0\]\.pwrLimitCurrent = (\d+\.?\d*) %";
            const string tempLimitMinPattern = @"policy\[0\]\.tempLimitMin     = (\d+\.?\d*)C";
            const string tempLimitMaxPattern = @"policy\[0\]\.tempLimitMax     = (\d+\.?\d*)C";
            const string tempLimitDefaultPattern = @"policy\[0\]\.tempLimitDefault = (\d+\.?\d*)C";
            const string tempLimitPattern = @"policy\[0\]\.tempLimitCurrent = (\d+\.?\d*)C";
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            int exitCode = -1;
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} ps20e", ref exitCode, out string output);
            int coreClockDeltaMin = 0;
            int coreClockDeltaMax = 0;
            int memoryClockDeltaMin = 0;
            int memoryClockDeltaMax = 0;
            int cool = 0;
            int coolMin = 0;
            int coolMax = 0;
            double powerMin = 50;
            double powerMax = 100;
            double powerDefault = 100;
            double powerCapacity = 0;
            double tempLimitMin = 0;
            double tempLimitMax = 0;
            double tempLimitDefault = 0;
            double tempLimit = 0;
            if (exitCode == 0) {
                Match match = Regex.Match(output, coreClockDeltaMinMaxPattern, RegexOptions.Compiled);
                if (match.Success) {
                    int.TryParse(match.Groups[1].Value, out int coreClockDelta);
                    gpu.CoreClockDelta = coreClockDelta;
                    int.TryParse(match.Groups[2].Value, out coreClockDeltaMin);
                    int.TryParse(match.Groups[3].Value, out coreClockDeltaMax);
                }
                match = Regex.Match(output, memoryClockDeltaMinMaxPattern, RegexOptions.Compiled);
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
                Match match = Regex.Match(output, coolSpeedMinMaxPattern, RegexOptions.Compiled);
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
                Match match = Regex.Match(output, powerMinPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerMin);
                }
                match = Regex.Match(output, powerMaxPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerMax);
                }
                match = Regex.Match(output, powerLimitCurrentPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerCapacity);
                }
                match = Regex.Match(output, powerDefaultPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out powerDefault);
                }
                gpu.PowerMin = powerMin;
                gpu.PowerMax = powerMax;
                gpu.PowerDefault = powerDefault;
                gpu.PowerCapacity = (int)powerCapacity;
            }
            Windows.Cmd.RunClose(SpecialPath.NTMinerOverClockFileFullName, $"gpu:{gpu.Index} therminfo", ref exitCode, out output);
            if (exitCode == 0) {
                Match match = Regex.Match(output, tempLimitMinPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out tempLimitMin);
                }
                match = Regex.Match(output, tempLimitMaxPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out tempLimitMax);
                }
                match = Regex.Match(output, tempLimitDefaultPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out tempLimitDefault);
                }
                match = Regex.Match(output, tempLimitPattern, RegexOptions.Compiled);
                if (match.Success) {
                    double.TryParse(match.Groups[1].Value, out tempLimit);
                }
                gpu.TempLimitMin = (int)Math.Ceiling(tempLimitMin);
                gpu.TempLimitMax = (int)Math.Floor(tempLimitMax);
                gpu.TempLimitDefault = (int)tempLimitDefault;
                gpu.TempLimit = (int)tempLimit;
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
