namespace NTMiner.Core.Gpus.Impl {
    public class AMDClockDeltaSet : IGpuClockDeltaSet {
        private readonly INTMinerRoot _root;

        public AMDClockDeltaSet(INTMinerRoot root) {
            _root = root;
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            data = null;
            return false;
        }
    }
}
