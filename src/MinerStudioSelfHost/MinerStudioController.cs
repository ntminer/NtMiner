using System;
using System.Web.Http;

namespace NTMiner {
    public class MinerStudioController : ApiController, IShowMainWindow {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand());
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseMinerStudio([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.Execute(new CloseNTMinerCommand());
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
