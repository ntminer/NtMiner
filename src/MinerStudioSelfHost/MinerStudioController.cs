using NTMiner.Controllers;
using System;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="Consts.MinerStudioPort"/>
    /// </summary>
    public class MinerStudioController : ApiController, IMinerStudioController {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: false));
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseMinerStudio([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                    VirtualRoot.Execute(new CloseNTMinerCommand());
                });
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
