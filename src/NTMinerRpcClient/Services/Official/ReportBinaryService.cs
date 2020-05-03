using NTMiner.Controllers;
using NTMiner.Report;
using System;

namespace NTMiner.Services.Official {
    public class ReportBinaryService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IReportBinaryController>();

        public ReportBinaryService() {
        }

        public void ReportSpeedAsync(SpeedData data, Action<ReportResponse, Exception> callback) {
            BinaryRequestJsonResponseRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IReportBinaryController.ReportSpeed), query: null, data, callback, timeountMilliseconds: 5000);
        }
    }
}
