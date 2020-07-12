using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Gpus {
    public interface IGpusSpeed {
        void IncreaseFoundShare(int gpuIndex);
        void IncreaseAcceptShare(int gpuIndex);
        void IncreaseRejectShare(int gpuIndex);
        void IncreaseIncorrectShare(int gpuIndex);
        void ResetShare();
        IGpuSpeed CurrentSpeed(int gpuIndex);
        AverageSpeed GetAverageSpeed(int gpuIndex);
        List<IGpuSpeed> GetGpuSpeedHistory(int index);
        void SetCurrentSpeed(int gpuIndex, double speed, bool isDual, DateTime now);
        IEnumerable<IGpuSpeed> AsEnumerable();
    }
}
