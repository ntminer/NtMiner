namespace NTMiner.Core.Gpus.Impl {
    public class GpuAllOverClock : IOverClock {
        public GpuAllOverClock(IGpuOverClockData data) {
            this.Data = data;
        }

        public IGpuOverClockData Data { get; private set; }

        public void SetCool() {
            
        }

        public void SetCoreClock() {
            
        }

        public void SetMemoryClock() {
            
        }

        public void SetPowerCapacity() {
            
        }
    }
}
