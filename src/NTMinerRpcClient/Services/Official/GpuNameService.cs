using NTMiner.Controllers;
using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class GpuNameService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IGpuNameController>();

        public GpuNameService() { }

        public void QueryGpuNamesAsync(QueryGpuNamesRequest request, Action<QueryGpuNamesResponse, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.QueryGpuNames), request, callback, timeountMilliseconds: 5 * 1000);
        }

        public void QueryGpuNameCountsAsync(QueryGpuNameCountsRequest request, Action<QueryGpuNameCountsResponse, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.QueryGpuNameCounts), request, callback, timeountMilliseconds: 5 * 1000);
        }

        public void SetGpuNameAsync(GpuName gpuName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.SetGpuName), gpuName, callback, timeountMilliseconds: 5 * 1000);
        }

        public void RemoveGpuNameAsync(GpuName gpuName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.RemoveGpuName), gpuName, callback, timeountMilliseconds: 5 * 1000);
        }
    }
}
