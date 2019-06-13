using NTMiner.Core.Gpus.Impl.Amd;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        private readonly AdlHelper _adlHelper;
        public AMDOverClock(AdlHelper adlHelper) {
            _adlHelper = adlHelper;
        }

        public void SetCoreClock(int gpuIndex, int value) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _adlHelper.SetSystemClockByIndex(gpu.Index, value);
                    this.RefreshGpuState(gpu.Index);
                }
            }
            else {
                _adlHelper.SetSystemClockByIndex(gpuIndex, value);
                this.RefreshGpuState(gpuIndex);
            }
        }

        public void SetMemoryClock(int gpuIndex, int value) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _adlHelper.SetMemoryClockByIndex(gpu.Index, value);
                    this.RefreshGpuState(gpu.Index);
                }
            }
            else {
                _adlHelper.SetMemoryClockByIndex(gpuIndex, value);
                this.RefreshGpuState(gpuIndex);
            }
        }

        public void SetPowerCapacity(int gpuIndex, int value) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _adlHelper.SetPowerLimitByIndex(gpu.Index, value);
                    this.RefreshGpuState(gpu.Index);
                }
            }
            else {
                _adlHelper.SetPowerLimitByIndex(gpuIndex, value);
                this.RefreshGpuState(gpuIndex);
            }
        }

        public void SetThermCapacity(int gpuIndex, int value) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _adlHelper.SetTempLimitByIndex(gpu.Index, value);
                    this.RefreshGpuState(gpu.Index);
                }
            }
            else {
                _adlHelper.SetTempLimitByIndex(gpuIndex, value);
                this.RefreshGpuState(gpuIndex);
            }
        }

        public void SetCool(int gpuIndex, int value) {
            if (value == 0) {
                return;
            }
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                foreach (var gpu in NTMinerRoot.Instance.GpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    _adlHelper.SetFunSpeedByIndex(gpu.Index, value);
                    this.RefreshGpuState(gpu.Index);
                }
            }
            else {
                _adlHelper.SetFunSpeedByIndex(gpuIndex, value);
                this.RefreshGpuState(gpuIndex);
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
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
