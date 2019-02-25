namespace NTMiner.Core.Gpus.Impl {
    public class NVIDIAOverClock : IOverClock {
        private readonly int _gpuIndex;

        public NVIDIAOverClock(int gpuIndex) {
            _gpuIndex = gpuIndex;
        }

        public void SetCool(int value) {
            
        }

        public void SetCoreClock(int deltaValue) {
            
        }

        public void SetMemoryClock(int deltaValue) {
            
        }

        public void SetPowerCapacity(int value) {
            
        }
    }
}
