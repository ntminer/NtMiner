using NTMiner.Controllers;
using System;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="NTKeyword.MinerStudioPort"/>
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
        public ResponseBase CloseMinerStudio([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                100.MillisecondsDelay().ContinueWith(t => {
                    VirtualRoot.Execute(new CloseNTMinerCommand("群控客户端升级成功后关闭旧版客户端"));
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
