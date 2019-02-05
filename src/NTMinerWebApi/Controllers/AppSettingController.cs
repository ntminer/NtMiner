using NTMiner.AppSetting;
using System;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AppSettingController : ApiController {
        public GetAppSettingsResponse Get(Guid messageId) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSettings();
                return GetAppSettingsResponse.Ok(messageId, data.Select(a => AppSettingData.Create(a)).ToList());
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingResponse Get(Guid messageId, string key) {
            try {
                IAppSetting data = HostRoot.Current.AppSettingSet.GetAppSetting(key);
                return GetAppSettingResponse.Ok(messageId, AppSettingData.Create(data));
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingResponse>(messageId, e.Message);
            }
        }

        public ResponseBase Post([FromBody]SetAppSettingRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                Global.Execute(new SetAppSettingCommand(request.Data));
                Global.WriteDevLine($"{request.Data.Key} {request.Data.Value}");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
    }
}
