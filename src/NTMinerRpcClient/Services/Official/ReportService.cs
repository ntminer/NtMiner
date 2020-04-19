using NTMiner.Controllers;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class ReportService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IReportController>();

        public ReportService() {
        }

        public void ReportSpeedAsync(SpeedData data, Action<ReportResponse, Exception> callback) {
            RpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IReportController.ReportSpeed), data, callback, timeountMilliseconds: 5000);
        }

        public void ReportStateAsync(Guid clientId, bool isMining) {
            ReportState request = new ReportState {
                ClientId = clientId,
                IsMining = isMining
            };
            RpcRoot.FirePostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IReportController.ReportState), null, request, null, 5000);
        }
    }
}