using NTMiner.Controllers;
using NTMiner.Gpus;
using System;

namespace NTMiner.Services.Official {
    public class GpuNameService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IGpuNameController>();

        internal GpuNameService() { }

        public void QueryGpuNameCountsAsync(QueryGpuNameCountsRequest request, Action<QueryGpuNameCountsResponse, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IGpuNameController.QueryGpuNameCounts), request, callback, timeountMilliseconds: 5 * 1000);
        }
    }
}
