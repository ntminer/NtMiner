using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.ServerNode;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class AppSettingController : ApiControllerBase, IAppSettingController {
        [Role.Public]
        [HttpGet]
        [HttpPost]
        public DateTime GetTime() {
            return DateTime.Now;
        }

        [Role.Public]
        [HttpPost]
        public string GetJsonFileVersion([FromBody]AppSettingRequest request) {
            ServerStateResponse serverState = WebApiRoot.GetServerStateResponse(request.Key);
            return serverState.ToLine();
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase SetAppSetting([FromBody]DataRequest<AppSettingData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.Execute(new SetLocalAppSettingCommand(request.Data));
                Logger.InfoDebugLine($"{nameof(SetAppSetting)}({request.Data.Key}, {request.Data.Value})");
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
