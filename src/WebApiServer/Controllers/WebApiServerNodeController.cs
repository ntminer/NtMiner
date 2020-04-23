using NTMiner.ServerNode;
using System;

namespace NTMiner.Controllers {
    public class WebApiServerNodeController : IWebApiServerNodeController {
        public WebApiServerNodeController() { }

        public DataResponse<WebApiServerState> GetServerState(SignRequest request) {
            throw new NotImplementedException();
        }
    }
}
