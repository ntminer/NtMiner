using NTMiner.Controllers;
using NTMiner.ServerNode;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class WsServerNodeService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IWsServerNodeController>();

        internal WsServerNodeService() {
        }

        public void GetNodesAsync(Action<DataResponse<List<WsServerNodeState>>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IWsServerNodeController.Nodes), new object(), callback);
        }

        public void GetNodeAddressesAsync(Action<DataResponse<string[]>, Exception> callback) {
            RpcRoot.JsonRpc.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IWsServerNodeController.NodeAddresses), new object(), callback);
        }

        public void GetNodeAddressAsync(Guid clientId, string outerUserId, Action<DataResponse<string>, Exception> callback) {
            var data = new GetWsServerNodeAddressRequest {
                ClientId = clientId,
                UserId = outerUserId
            };
            RpcRoot.JsonRpc.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IWsServerNodeController.GetNodeAddress), data, callback, timeountMilliseconds: 8000);
        }
    }
}
