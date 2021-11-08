using NTMiner.Controllers;
using NTMiner.Report;
using System;

namespace NTMiner.Services.Official {
    public class ReportService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IReportController>();

        internal ReportService() {
        }

        //public void ReportSpeedAsync(SpeedDto speedDto, Action<ReportResponse, Exception> callback) {
        //    RpcRoot.JsonRpc.PostAsync(
        //        _controllerName, 
        //        nameof(IReportController.ReportSpeed), 
        //        speedDto, 
        //        callback, 
        //        timeountMilliseconds: 5000);
        //}

        //public void ReportStateAsync(Guid clientId, bool isMining) {
        //    ReportState request = new ReportState {
        //        ClientId = clientId,
        //        IsMining = isMining
        //    };
        //    RpcRoot.JsonRpc.FirePostAsync(
        //        _controllerName, 
        //        nameof(IReportController.ReportState), 
        //        null, 
        //        request, 
        //        null, 
        //        5000);
        //}
    }
}