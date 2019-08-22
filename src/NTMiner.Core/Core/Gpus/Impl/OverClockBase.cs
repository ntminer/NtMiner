using System;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl {
    public abstract class OverClockBase {
        protected void SetCoreClock(int gpuIndex, int value, int voltage, Func<int, int, int, bool> setCoreClock) {
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
                    setCoreClock(gpu.GetOverClockId(), value, voltage);
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
                setCoreClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        protected void SetMemoryClock(int gpuIndex, int value, int voltage, Func<int, int, int, bool> setMemoryClock) {
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
                    setMemoryClock(gpu.GetOverClockId(), value, voltage);
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
                setMemoryClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        protected void SetPowerCapacity(int gpuIndex, int value, Func<int, int, bool> setPowerLimit) {
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
                    setPowerLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.PowerCapacity) {
                    return;
                }
                setPowerLimit(gpu.GetOverClockId(), value);
            }
        }

        protected void SetThermCapacity(int gpuIndex, int value, Func<int, int, bool> setTempLimit) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value == gpu.TempLimit) {
                        continue;
                    }
                    setTempLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || value == gpu.TempLimit) {
                    return;
                }
                setTempLimit(gpu.GetOverClockId(), value);
            }
        }
    }
}
