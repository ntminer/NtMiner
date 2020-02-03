using NTMiner.Core.MinerClient;

namespace NTMiner.Report {
    public interface IReportDataProvider {
        SpeedData CreateSpeedData();
    }
}
