using NTMiner.Controllers;
using NTMiner.Report;
using System;

namespace NTMiner.Services.Official {
    public class ReportBinaryService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IReportBinaryController>();

        internal ReportBinaryService() {
        }

        public void ReportSpeedAsync(SpeedDto speedDto, Action<ReportResponse, Exception> callback) {
            BinaryRequestJsonResponseRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IReportBinaryController.ReportSpeed), query: null, speedDto, callback, timeountMilliseconds: 5000);
        }
    }
}
