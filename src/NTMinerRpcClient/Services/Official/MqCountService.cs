using NTMiner.Controllers;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class MqCountService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IMqCountController>();

        internal MqCountService() { }

        public void GetMqCountsAsync(Action<DataResponse<MqCountData[]>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(IMqCountController.MqCounts),
                null,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void GetMqCountAsync(string appId, Action<DataResponse<MqCountData>, Exception> callback) {
            DataRequest<string> request = new DataRequest<string> {
                Data = appId
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(IMqCountController.MqCount),
                request,
                callback,
                timeountMilliseconds: 5 * 1000);
        }
    }
}
