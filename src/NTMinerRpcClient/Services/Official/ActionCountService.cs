using NTMiner.Controllers;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class ActionCountService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IActionCountController>();

        internal ActionCountService() { }

        public void QueryActionCountsAsync(QueryActionCountsRequest request, Action<QueryActionCountsResponse, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(IActionCountController.QueryActionCounts),
                request,
                callback,
                timeountMilliseconds: 5 * 1000);
        }
    }
}
