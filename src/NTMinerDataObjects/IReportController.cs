using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;

namespace NTMiner {
    public interface IReportController {
        DateTime GetTime();
        void ReportSpeed(SpeedData speedData);
        void ReportState(ReportStateRequest request);
    }
}
