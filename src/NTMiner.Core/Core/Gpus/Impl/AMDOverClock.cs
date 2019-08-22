using NTMiner.Gpus;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        private readonly AdlHelper _adlHelper;
        public AMDOverClock(AdlHelper adlHelper) {
            _adlHelper = adlHelper;
        }

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                int minVoltage = NTMinerRoot.Instance.GpuSet.Where(a => a.Index != NTMinerRoot.GpuAllId).Min(a => a.VoltMin);
                int maxVoltage = NTMinerRoot.Instance.GpuSet.Where(a => a.Index != NTMinerRoot.GpuAllId).Max(a => a.VoltMax);
                if (voltage < minVoltage) {
                    voltage = minVoltage;
                }
                if (voltage > maxVoltage) {
                    voltage = maxVoltage;
                }
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage) {
                        continue;
                    }
                    _adlHelper.SetCoreClock(gpu.Index, value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage)) {
                    return;
                }
                if (voltage < gpu.VoltMin) {
                    voltage = gpu.VoltMin;
                }
                if (voltage > gpu.VoltMax) {
                    voltage = gpu.VoltMax;
                }
                _adlHelper.SetCoreClock(gpuIndex, value, voltage);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                int minVoltage = NTMinerRoot.Instance.GpuSet.Where(a => a.Index != NTMinerRoot.GpuAllId).Min(a => a.VoltMin);
                int maxVoltage = NTMinerRoot.Instance.GpuSet.Where(a => a.Index != NTMinerRoot.GpuAllId).Max(a => a.VoltMax);
                if (voltage < minVoltage) {
                    voltage = minVoltage;
                }
                if (voltage > maxVoltage) {
                    voltage = maxVoltage;
                }
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage) {
                        continue;
                    }
                    _adlHelper.SetMemoryClock(gpu.Index, value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage)) {
                    return;
                }
                if (voltage < gpu.VoltMin) {
                    voltage = gpu.VoltMin;
                }
                if (voltage > gpu.VoltMax) {
                    voltage = gpu.VoltMax;
                }
                _adlHelper.SetMemoryClock(gpuIndex, value, voltage);
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
                    _adlHelper.SetPowerLimit(gpu.Index, value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.PowerCapacity) {
                    return;
                }
                _adlHelper.SetPowerLimit(gpuIndex, value);
            }
        }

        public void SetThermCapacity(int gpuIndex, int value) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.TempLimit) {
                        continue;
                    }
                    _adlHelper.SetTempLimit(gpu.Index, value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.TempLimit) {
                    return;
                }
                _adlHelper.SetTempLimit(gpuIndex, value);
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
                    _adlHelper.SetFunSpeed(gpu.Index, value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.Cool) {
                    return;
                }
                _adlHelper.SetFunSpeed(gpuIndex, value);
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
            SetPowerCapacity(NTMinerRoot.GpuAllId, 0);
            SetThermCapacity(NTMinerRoot.GpuAllId, 0);
            SetCool(NTMinerRoot.GpuAllId, 0);
            RefreshGpuState(NTMinerRoot.GpuAllId);
        }

        private void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                gpu.PowerCapacity = _adlHelper.GetPowerLimit(gpu.Index);
                gpu.TempLimit = _adlHelper.GetTempLimit(gpu.Index);
                if (_adlHelper.GetMemoryClock(gpu.Index, out int memoryClock, out int iVddc)) {
                    gpu.MemoryClockDelta = memoryClock;
                    gpu.MemoryVoltage = iVddc;
                }
                if (_adlHelper.GetCoreClock(gpu.Index, out int coreClock, out iVddc)) {
                    gpu.CoreClockDelta = coreClock;
                    gpu.CoreVoltage = iVddc;
                }
                _adlHelper.GetClockRange(
                    gpu.Index,
                    out int coreClockDeltaMin, out int coreClockDeltaMax,
                    out int memoryClockDeltaMin, out int memoryClockDeltaMax,
                    out int voltMin, out int voltMax, out int voltDefault,
                    out int powerMin, out int powerMax, out int powerDefault,
                    out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault,
                    out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault);
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
