using NTMiner.Gpus;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : OverClockBase, IOverClock {
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

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            base.SetCoreClock(gpuIndex, value, voltage, _nvapiHelper.SetCoreClock);
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            base.SetMemoryClock(gpuIndex, value, voltage, _nvapiHelper.SetMemoryClock);
        }

        public void SetPowerLimit(int gpuIndex, int value) {
            base.SetPowerLimit(gpuIndex, value, _nvapiHelper.SetPowerLimit);
        }

        public void SetTempLimit(int gpuIndex, int value) {
            base.SetThermCapacity(gpuIndex, value, _nvapiHelper.SetTempLimit);
        }

        public void SetFanSpeed(int gpuIndex, int value) {
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
                    _nvapiHelper.SetCooler(gpu.GetOverClockId(), (uint)value, isAutoMode: false);
                    NTMinerRoot.Instance.GpuSet.LoadGpuState();
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.Cool) {
                    return;
                }
                _nvapiHelper.SetCooler(gpu.GetOverClockId(), (uint)value, isAutoMode: false);
                NTMinerRoot.Instance.GpuSet.LoadGpuState();
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
            SetCoreClock(NTMinerRoot.GpuAllId, 0, 0);
            SetMemoryClock(NTMinerRoot.GpuAllId, 0, 0);
            SetPowerLimit(NTMinerRoot.GpuAllId, 0);
            SetTempLimit(NTMinerRoot.GpuAllId, 0);
            SetFanSpeed(NTMinerRoot.GpuAllId, 0);
        }

        private void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                _nvapiHelper.GetClockRange(
                    gpu.GetOverClockId(),
                    out int coreClockDeltaMin, out int coreClockDeltaMax, out int coreClockDelta,
                    out int memoryClockDeltaMin, out int memoryClockDeltaMax, out int memoryClockDelta,
                    out int powerMin, out int powerMax, out int powerDefault, out int powerLimit,
                    out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault, out int tempLimit,
                    out uint fanSpeedCurr, out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault);
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
                if (gpu.FanSpeed != fanSpeedCurr) {
                    gpu.FanSpeed = fanSpeedCurr;
                    VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
