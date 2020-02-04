using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    public interface IReportController {
        ReportResponse ReportSpeed(SpeedData speedData);
        void ReportState(ReportState request);
    }
}
