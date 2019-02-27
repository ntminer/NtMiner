namespace NTMiner.Core.Gpus.Impl {
    public class EmptyClockDeltaSet : IGpuClockDeltaSet {
        private readonly INTMinerRoot _root;

        public EmptyClockDeltaSet(INTMinerRoot root) {
            _root = root;
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            data = null;
            return false;
        }
    }
}
