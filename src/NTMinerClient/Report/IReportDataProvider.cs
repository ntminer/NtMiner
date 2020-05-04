using System;

namespace NTMiner.Report {
    public interface IReportDataProvider {
        DateTime WsGetSpeedOn { get; set; }
        SpeedDto CreateSpeedDto();
    }
}
