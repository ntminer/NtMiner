using NTMiner.MinerServer;
using System;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AppSettingController : ApiController {
        [HttpPost]
        public GetAppSettingResponse AppSetting([FromBody]AppSettingRequest request) {
            try {
                IAppSetting data = HostRoot.Current.AppSettingSet[request.Key];
                return GetAppSettingResponse.Ok(request.MessageId, AppSettingData.Create(data));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingResponse>(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public GetAppSettingsResponse AppSettings([FromBody]AppSettingsRequest request) {
            try {
                var data = HostRoot.Current.AppSettingSet;
                return GetAppSettingsResponse.Ok(request.MessageId, data.Select(a => AppSettingData.Create(a)).ToList());
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetAppSetting([FromBody]SetAppSettingRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                VirtualRoot.Execute(new ChangeAppSettingCommand(request.Data));
                Write.DevLine($"{request.Data.Key} {request.Data.Value}");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
    }
}
