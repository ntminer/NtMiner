using NTMiner.Core.Gpus.Impl.Amd;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        private readonly AdlHelper _adlHelper;
        public AMDOverClock(AdlHelper adlHelper) {
            _adlHelper = adlHelper;
        }

        public void SetCoreClock(int gpuIndex, int value, ref HashSet<int> effectGpus) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.CoreClockDelta == value) {
                        continue;
                    }
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetSystemClockByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.CoreClockDelta == value) {
                    return;
                }
                effectGpus.Add(gpu.Index);
                _adlHelper.SetSystemClockByIndex(gpuIndex, value);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value, ref HashSet<int> effectGpus) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.MemoryClockDelta == value) {
                        continue;
                    }
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetMemoryClockByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.MemoryClockDelta == value) {
                    return;
                }
                effectGpus.Add(gpu.Index);
                _adlHelper.SetMemoryClockByIndex(gpuIndex, value);
            }
        }

        public void SetPowerCapacity(int gpuIndex, int value, ref HashSet<int> effectGpus) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.PowerCapacity == value) {
                        continue;
                    }
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetPowerLimitByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.PowerCapacity == value) {
                    return;
                }
                effectGpus.Add(gpu.Index);
                _adlHelper.SetPowerLimitByIndex(gpuIndex, value);
            }
        }

        public void SetThermCapacity(int gpuIndex, int value, ref HashSet<int> effectGpus) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.TempLimit == value) {
                        continue;
                    }
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetTempLimitByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.TempLimit == value) {
                    return;
                }
                effectGpus.Add(gpu.Index);
                _adlHelper.SetTempLimitByIndex(gpuIndex, value);
            }
        }

        public void SetCool(int gpuIndex, int value, ref HashSet<int> effectGpus) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    if (gpu.Cool == value) {
                        continue;
                    }
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetFunSpeedByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu) && gpu.Cool == value) {
                    return;
                }
                effectGpus.Add(gpu.Index);
                _adlHelper.SetFunSpeedByIndex(gpuIndex, value);
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

        private void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            gpu.PowerCapacity = _adlHelper.GetPowerLimitByIndex(gpu.Index);
            gpu.TempLimit = _adlHelper.GetTempLimitByIndex(gpu.Index);
            gpu.MemoryClockDelta = _adlHelper.GetMemoryClockByIndex(gpu.Index);
            gpu.CoreClockDelta = _adlHelper.GetSystemClockByIndex(gpu.Index);
            _adlHelper.GetClockRange(gpu.Index, out int coreClockDeltaMin, out int coreClockDeltaMax);
            gpu.CoreClockDeltaMin = coreClockDeltaMin;
            gpu.CoreClockDeltaMax = coreClockDeltaMax;
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
