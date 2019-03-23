using NTMiner.MinerClient;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public interface IReportController {
        void ReportSpeed(SpeedData speedData);
        void ReportState(ReportState request);
    }
}
