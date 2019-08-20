using NTMiner.Gpus;
using System.Collections.Generic;

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
        }

        public void SetCoreClock(int gpuIndex, int value) {
            value = 1000 * value;
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.CoreClockDelta) {
                        continue;
                    }
                    _nvapiHelper.SetCoreClock(gpu.GetBusId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.CoreClockDelta) {
                    return;
                }
                _nvapiHelper.SetCoreClock(gpu.GetBusId(), value);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value) {
            value = 1000 * value;
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.MemoryClockDelta) {
                        continue;
                    }
                    _nvapiHelper.SetMemClock(gpu.GetBusId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.MemoryClockDelta) {
                    return;
                }
                _nvapiHelper.SetMemClock(gpu.GetBusId(), value);
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
                    if (value == gpu.PowerCapacity) {
                        continue;
                    }
                    _nvapiHelper.SetPowerLimit(gpu.GetBusId(), (uint)value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.PowerCapacity) {
                    return;
                }
                _nvapiHelper.SetPowerLimit(gpu.GetBusId(), (uint)value);
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
                    if (value == gpu.TempLimit) {
                        continue;
                    }
                    _nvapiHelper.SetThermal(gpu.GetBusId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.TempLimit) {
                    return;
                }
                _nvapiHelper.SetThermal(gpu.GetBusId(), value);
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
                    if (value == gpu.Cool) {
                        continue;
                    }
                    _nvapiHelper.SetCooler(gpu.GetBusId(), (uint)value, isAutoMode: false);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.Cool) {
                    return;
                }
                _nvapiHelper.SetCooler(gpu.GetBusId(), (uint)value, isAutoMode: false);
            }
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    RefreshGpuState(gpu);
                }
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
