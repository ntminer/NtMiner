using System;

namespace NTMiner.ServiceContracts {
    public interface IReportService : IDisposable {
        void Login(LoginData message);

        void ReportState(Guid clientId, bool isMining);

        void ReportSpeed(SpeedData message);
    }
}
