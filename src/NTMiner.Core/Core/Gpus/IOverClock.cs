using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        void SetCoreClock(int gpuIndex, int value, ref HashSet<int> effectGpus);
        void SetMemoryClock(int gpuIndex, int value, ref HashSet<int> effectGpus);
        void SetPowerCapacity(int gpuIndex, int value, ref HashSet<int> effectGpus);
        void SetThermCapacity(int gpuIndex, int value, ref HashSet<int> effectGpus);
        void SetCool(int gpuIndex, int value, ref HashSet<int> effectGpus);
        void RefreshGpuState(int gpuIndex);
    }
}
