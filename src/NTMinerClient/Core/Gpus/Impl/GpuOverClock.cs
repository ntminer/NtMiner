using NTMiner.Gpus;
using System;

namespace NTMiner.Core.Gpus.Impl {
    public class GpuOverClock : IOverClock {
        private readonly IGpuHelper _gpuHelper;
        public GpuOverClock(IGpuHelper gpuHelper) {
            _gpuHelper = gpuHelper;
        }

        public void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                OverClockRange range = _gpuHelper.GetClockRange(gpu.GetOverClockId());
                gpu.UpdateState(range);
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
        }

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage) {
                            continue;
                        }
                    }
                    _gpuHelper.SetCoreClock(gpu.GetOverClockId(), value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.CoreClockDelta && voltage == gpu.CoreVoltage)) {
                    return;
                }
                _gpuHelper.SetCoreClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage) {
                            continue;
                        }
                    }
                    _gpuHelper.SetMemoryClock(gpu.GetOverClockId(), value, voltage);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.MemoryClockDelta && voltage == gpu.MemoryVoltage)) {
                    return;
                }
                _gpuHelper.SetMemoryClock(gpu.GetOverClockId(), value, voltage);
            }
        }

        public void SetPowerLimit(int gpuIndex, int value) {
            if (value == 0) {
                value = 100;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.PowerCapacity) {
                            continue;
                        }
                    }
                    _gpuHelper.SetPowerLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.PowerCapacity)) {
                    return;
                }
                _gpuHelper.SetPowerLimit(gpu.GetOverClockId(), value);
            }
        }

        public void SetTempLimit(int gpuIndex, int value) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (value != 0) {
                        if (value == gpu.TempLimit) {
                            continue;
                        }
                    }
                    _gpuHelper.SetTempLimit(gpu.GetOverClockId(), value);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) || (value != 0 && value == gpu.TempLimit)) {
                    return;
                }
                _gpuHelper.SetTempLimit(gpu.GetOverClockId(), value);
            }
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            bool isAutoModel = value == 0;
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _gpuHelper.SetFanSpeed(gpu.GetOverClockId(), value, isAutoModel);
                }
            }
            else {
                if (!NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                _gpuHelper.SetFanSpeed(gpu.GetOverClockId(), value, isAutoModel);
            }
        }

        public void Restore() {
            SetCoreClock(NTMinerRoot.GpuAllId, 0, 0);
            SetMemoryClock(NTMinerRoot.GpuAllId, 0, 0);
            SetPowerLimit(NTMinerRoot.GpuAllId, 0);
            SetTempLimit(NTMinerRoot.GpuAllId, 0);
            SetFanSpeed(NTMinerRoot.GpuAllId, 0);
            RefreshGpuState(NTMinerRoot.GpuAllId);
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet.AsEnumerable()) {
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
