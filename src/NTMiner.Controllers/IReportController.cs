using NTMiner.Report;

namespace NTMiner.Controllers {
    public interface IReportController {
        ReportResponse ReportSpeed(SpeedData speedData);
        void ReportState(ReportState request);
    }
}
