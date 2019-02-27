namespace NTMiner.Core.Gpus {
    public interface IGpuClockDeltaSet {
        bool TryGetValue(int gpuIndex, out IGpuClockDelta data);
    }
}
