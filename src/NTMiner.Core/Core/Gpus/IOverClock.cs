namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        IGpuOverClockData Data { get; }
        void SetCoreClock();
        void SetMemoryClock();
        void SetPowerCapacity();
        void SetCool();
    }
}
