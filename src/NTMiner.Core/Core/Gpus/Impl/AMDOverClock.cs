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
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetSystemClockByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
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
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetMemoryClockByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
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
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetPowerLimitByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
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
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetTempLimitByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
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
                    effectGpus.Add(gpu.Index);
                    _adlHelper.SetFunSpeedByIndex(gpu.Index, value);
                }
            }
            else {
                if (NTMinerRoot.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
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
            _adlHelper.GetClockRangeByIndex(
                gpu.Index, 
                out int coreClockDeltaMin, out int coreClockDeltaMax, 
                out int memoryClockDeltaMin, out int memoryClockDeltaMax,
                out int powerMin, out int powerMax,
                out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault);
            gpu.CoreClockDeltaMin = coreClockDeltaMin;
            gpu.CoreClockDeltaMax = coreClockDeltaMax;
            gpu.MemoryClockDeltaMin = memoryClockDeltaMin;
            gpu.MemoryClockDeltaMax = memoryClockDeltaMax;
            gpu.PowerMin = powerMin;
            gpu.PowerMax = powerMax;
            gpu.TempLimitMin = tempLimitMin;
            gpu.TempLimitMax = tempLimitMax;
            gpu.TempLimitDefault = tempLimitDefault;
            gpu.CoolMin = 50;
            gpu.CoolMax = 100;
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
