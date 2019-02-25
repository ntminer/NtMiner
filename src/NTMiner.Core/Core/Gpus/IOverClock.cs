namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        void SetCoreClock(int deltaValue);
        void SetMemoryClock(int deltaValue);
        void SetPowerCapacity(int value);
        void SetCool(int value);
    }
}
