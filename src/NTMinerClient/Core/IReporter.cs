using NTMiner.MinerClient;

namespace NTMiner.Core {
    public interface IReporter {
        SpeedData CreateSpeedData();
    }
}
