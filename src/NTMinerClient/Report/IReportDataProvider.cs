using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Report {
    public interface IReportDataProvider {
        DateTime WsGetSpeedOn { get; set; }
        SpeedData CreateSpeedData();
    }
}
