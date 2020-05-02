using NTMiner.Controllers;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class WebApiServerNodeService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<IWebApiServerNodeController>();

        public WebApiServerNodeService() {
        }

        public void GetServerStateAsync(Action<DataResponse<WebApiServerState>, Exception> callback) {
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IWebApiServerNodeController.GetServerState), new object(), callback);
        }
    }
}
