using System;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public abstract class OverClockBase {
        protected abstract void RefreshGpuState(IGpu gpu);

        protected void SetCoreClock(int gpuIndex, int value, int voltage, Func<int, int, int, bool> setCoreClock) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage) {
                            continue;
                        }
                    }
                    setCoreClock(gpu.GetOverClockId(), value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage)) {
                    return;
                }
                setCoreClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        protected void SetMemoryClock(int gpuIndex, int value, int voltage, Func<int, int, int, bool> setMemoryClock) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage) {
                            continue;
                        }
                    }
                    setMemoryClock(gpu.GetOverClockId(), value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage)) {
                    return;
                }
                setMemoryClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        protected void SetPowerLimit(int gpuIndex, int value, Func<int, int, bool> setPowerLimit) {
            if (value == 0) {
                value = 100;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.PowerCapacity) {
                            continue;
                        }
                    }
                    setPowerLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.PowerCapacity)) {
                    return;
                }
                setPowerLimit(gpu.GetOverClockId(), value);
            }
        }

        protected void SetTempLimit(int gpuIndex, int value, Func<int, int, bool> setTempLimit) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.TempLimit) {
                            continue;
                        }
                    }
                    setTempLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.TempLimit)) {
                    return;
                }
                setTempLimit(gpu.GetOverClockId(), value);
            }
        }

        protected void SetFanSpeed(int gpuIndex, int value, bool isAutoMode, Func<int, int, bool, bool> setFanSpeed) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    setFanSpeed(gpu.GetOverClockId(), value, isAutoMode);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                setFanSpeed(gpu.GetOverClockId(), value, isAutoMode);
            }
        }

        protected void RefreshGpuState(int gpuIndex) {
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
    }
}
