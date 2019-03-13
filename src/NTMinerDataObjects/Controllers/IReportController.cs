using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IReportController {
        DateTime GetTime();
        void ReportSpeed(SpeedData speedData);
        void ReportState(ReportState request);
    }
}
