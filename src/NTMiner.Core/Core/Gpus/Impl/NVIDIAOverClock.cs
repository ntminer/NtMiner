using NTMiner.Gpus;
using System.Collections.Generic;

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
            base.SetTempLimit(gpuIndex, value, _nvapiHelper.SetTempLimit);
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            base.SetFanSpeed(gpuIndex, value, isAutoMode: value == 0, setFanSpeed: _nvapiHelper.SetFanSpeed);
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
        }

        protected override void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerRoot.GpuAllId) {
                return;
            }
            try {
                OverClockRange range = _nvapiHelper.GetClockRange(gpu.GetOverClockId());
                gpu.UpdateState(range);
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
        }
    }
}
