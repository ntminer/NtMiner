using System;

namespace NTMiner.Gpus.Impl {
    public class GpuOverClock : IOverClock {
        private readonly IGpuHelper _gpuHelper;
        public GpuOverClock(IGpuHelper gpuHelper) {
            _gpuHelper = gpuHelper;
        }

        public void RefreshGpuState(IGpu gpu) {
            if (gpu.Index == NTMinerContext.GpuAllId) {
                return;
            }
            try {
                OverClockRange range = _gpuHelper.GetClockRange(gpu);
                gpu.UpdateState(range);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.RaiseEvent(new GpuStateChangedEvent(Guid.Empty, gpu));
        }

        public void OverClock(int gpuIndex, OverClockValue value) {
            if (gpuIndex == NTMinerContext.GpuAllId) {
                foreach (var gpu in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerContext.GpuAllId) {
                        continue;
                    }
                    _gpuHelper.OverClock(gpu, value);
                }
            }
            else {
                if (!NTMinerContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                _gpuHelper.OverClock(gpu, value);
            }
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            if (gpuIndex == NTMinerContext.GpuAllId) {
                foreach (var gpu in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerContext.GpuAllId) {
                        continue;
                    }
                    _gpuHelper.SetFanSpeed(gpu, value);
                }
            }
            else {
                if (!NTMinerContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    return;
                }
                _gpuHelper.SetFanSpeed(gpu, value);
            }
        }

        public void Restore() {
            OverClock(gpuIndex: NTMinerContext.GpuAllId, OverClockValue.RestoreValue);
            RefreshGpuState(NTMinerContext.GpuAllId);
        }

        public void RefreshGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerContext.GpuAllId) {
                foreach (var gpu in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                    if (gpu.Index == NTMinerContext.GpuAllId) {
                        continue;
                    }
                    RefreshGpuState(gpu);
                }
            }
            else {
                if (NTMinerContext.Instance.GpuSet.TryGetGpu(gpuIndex, out IGpu gpu)) {
                    RefreshGpuState(gpu);
                }
            }
        }
    }
}
