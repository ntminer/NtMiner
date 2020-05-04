using NTMiner.Report;

namespace NTMiner.Controllers {
    public interface IReportController {
        ReportResponse ReportSpeed(SpeedDto speedDto);
        void ReportState(ReportState request);
    }
}
