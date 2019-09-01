using NTMiner.Gpus;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : OverClockBase, IOverClock {
        private readonly AdlHelper _adlHelper;
        public AMDOverClock(AdlHelper adlHelper) {
            _adlHelper = adlHelper;
        }

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            base.SetCoreClock(gpuIndex, value, voltage, _adlHelper.SetCoreClock);
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            base.SetMemoryClock(gpuIndex, value, voltage, _adlHelper.SetMemoryClock);
        }

        public void SetPowerLimit(int gpuIndex, int value) {
            base.SetPowerLimit(gpuIndex, value, _adlHelper.SetPowerLimit);
        }

        public void SetTempLimit(int gpuIndex, int value) {
            base.SetTempLimit(gpuIndex, value, _adlHelper.SetTempLimit);
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            base.SetFanSpeed(gpuIndex, value, isAutoMode: value == 0, setFanSpeed: _adlHelper.SetFanSpeed);
        }

        public new void RefreshGpuState(int gpuIndex) {
            base.RefreshGpuState(gpuIndex);
        }

        public void Restore() {
            SetCoreClock(NTMinerRoot.GpuAllId, 0, 0);
            SetMemoryClock(NTMinerRoot.GpuAllId, 0, 0);
            SetPowerLimit(NTMinerRoot.GpuAllId, 0);
            SetTempLimit(NTMinerRoot.GpuAllId, 0);
            SetFanSpeed(NTMinerRoot.GpuAllId, 0);
            RefreshGpuState(NTMinerRoot.GpuAllId);
        }

        protected override void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                OverClockRange range = _adlHelper.GetClockRange(gpu.GetOverClockId());
                gpu.UpdateState(range);
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
