using NTMiner.Core.MinerClient;

namespace NTMiner.Core {
    public interface IReportDataProvider {
        SpeedData CreateSpeedData();
    }
}
