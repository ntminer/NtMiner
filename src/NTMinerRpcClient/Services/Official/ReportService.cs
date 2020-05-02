using NTMiner.Controllers;
using NTMiner.Report;
using System;

namespace NTMiner.Services.Official {
    public class ReportService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<IReportController>();

        public ReportService() {
        }

        public void ReportSpeedAsync(SpeedData data, Action<ReportResponse, Exception> callback) {
            JsonRpcRoot.PostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IReportController.ReportSpeed), data, callback, timeountMilliseconds: 5000);
        }

        public void ReportStateAsync(Guid clientId, bool isMining) {
            ReportState request = new ReportState {
                ClientId = clientId,
                IsMining = isMining
            };
            JsonRpcRoot.FirePostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IReportController.ReportState), null, request, null, 5000);
        }
    }
}