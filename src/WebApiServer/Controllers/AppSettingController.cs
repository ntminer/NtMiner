using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.ServerNode;
using System;
using System.Linq;
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

        // 这个要保留，向后兼容
        [Role.Public]
        [HttpPost]
        public string GetJsonFileVersion([FromBody]JsonFileVersionRequest request) {
            return GetServerState(request).ToLine();
        }

        [Role.Public]
        [HttpPost]
        public ServerStateResponse GetServerState([FromBody]JsonFileVersionRequest request) {
            ServerStateResponse serverState = ServerStateResponse.Empty;
            if (request != null) {
                serverState = AppRoot.GetServerStateResponse(request.Key);
                if (request.ClientId != Guid.Empty) {
                    var clientData = AppRoot.ClientDataSet.GetByClientId(request.ClientId);
                    if (clientData != null && !string.IsNullOrEmpty(clientData.MACAddress)) {
                        serverState.NeedReClientId = request.MACAddress.All(a => !clientData.MACAddress.Contains(a));
                        NTMinerConsole.UserWarn($"重复的网卡地址：{string.Join(",", request.MACAddress)}");
                    }
                }
            }
            return serverState;
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
