using NTMiner.Controllers;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class WebApiServerNodeService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IWebApiServerNodeController>();

        public WebApiServerNodeService() {
        }

        public void GetServerStateAsync(Action<DataResponse<WebApiServerState>, Exception> callback) {
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IWebApiServerNodeController.GetServerState), new SignRequest(), callback);
        }
    }
}
