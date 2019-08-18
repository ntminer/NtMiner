using NTMiner.Gpus.Nvapi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        public class ValueItem {
            public string MethodName;
            public int GpuIndex;
            public int Value;
        }

        private readonly List<ValueItem> _values = new List<ValueItem>();
        private readonly object _locker = new object();

        private readonly NvapiHelper _nvapiHelper;
        public NVIDIAOverClock(NvapiHelper nvapiHelper) {
            _nvapiHelper = nvapiHelper;
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
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                _nvapiHelper.GetClockRangeByIndex(
                    gpu.GetBusId(),
                    out int coreClockDeltaMin, out int coreClockDeltaMax, out int coreClockDelta,
                    out int memoryClockDeltaMin, out int memoryClockDeltaMax, out int memoryClockDelta,
                    out int powerMin, out int powerMax, out int powerDefault, out int powerLimit,
                    out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault, out int tempLimit,
                    out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault);
                gpu.PowerCapacity = powerLimit;
                gpu.TempLimit = tempLimit;
                gpu.MemoryClockDelta = memoryClockDelta;
                gpu.CoreClockDelta = coreClockDelta;
                gpu.CoreClockDeltaMin = coreClockDeltaMin;
                gpu.CoreClockDeltaMax = coreClockDeltaMax;
                gpu.MemoryClockDeltaMin = memoryClockDeltaMin;
                gpu.MemoryClockDeltaMax = memoryClockDeltaMax;
                gpu.PowerMin = powerMin;
                gpu.PowerMax = powerMax;
                gpu.PowerDefault = powerDefault;
                gpu.TempLimitMin = tempLimitMin;
                gpu.TempLimitMax = tempLimitMax;
                gpu.TempLimitDefault = tempLimitDefault;
                gpu.CoolMin = fanSpeedMin;
                gpu.CoolMax = fanSpeedMax;
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
