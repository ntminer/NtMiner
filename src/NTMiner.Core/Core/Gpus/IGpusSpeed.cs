using System;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpusSpeed : IEnumerable<IGpuSpeed> {
        void IncreaseAcceptShare(int gpuIndex, bool isDual);
        void IncreaseRejectShare(int gpuIndex, bool isDual);
        IGpuSpeed CurrentSpeed(int gpuIndex);
        AverageSpeed GetAverageSpeed(int gpuIndex);
        List<IGpuSpeed> GetGpuSpeedHistory(int index);
        void SetCurrentSpeed(int gpuIndex, double speed, bool isDual, DateTime now);
    }
}
