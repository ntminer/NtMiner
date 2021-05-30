using NTMiner.Controllers;
using NTMiner.Gpus;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class AdminService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IAdminController>();

        internal AdminService() { }

        public void QueryActionCountsAsync(QueryActionCountsRequest request, Action<QueryActionCountsResponse, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.QueryActionCounts),
                request,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void QueryGpuNameCountsAsync(QueryGpuNameCountsRequest request, Action<QueryGpuNameCountsResponse, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.QueryGpuNameCounts),
                request,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void GetMqCountsAsync(Action<DataResponse<MqCountData[]>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.MqCounts),
                null,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void GetMqCountAsync(string appId, Action<DataResponse<MqCountData>, Exception> callback) {
            DataRequest<string> request = new DataRequest<string> {
                Data = appId
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.MqCount),
                request,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void GetMqAppIdsAsync(Action<DataResponse<MqAppIds>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.MqAppIds),
                null,
                callback,
                timeountMilliseconds: 5 * 1000);
        }

        public void GetServerStateAsync(Action<DataResponse<WebApiServerState>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(IAdminController.GetServerState),
                new object(),
                callback);
        }
    }
}
