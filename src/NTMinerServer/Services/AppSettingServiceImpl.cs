using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.Services {
    public class AppSettingServiceImpl : IAppSettingService {
        public GetAppSettingResponse GetAppSetting(Guid messageId, string key) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSetting(key);
                return GetAppSettingResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingsResponse GetAppSettings(Guid messageId, string[] keys) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSettings(keys);
                return GetAppSettingsResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingsResponse GetAllAppSettings(Guid messageId) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAllAppSettings();
                return GetAppSettingsResponse.Ok(messageId, data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public ResponseBase SetAppSetting(SetAppSettingRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(out response)) {
                    return response;
                }
                HostRoot.Current.AppSettingSet.SetAppSetting(request.Data);
                Global.DebugLine($"{request.Data.Key} {request.Data.Value}");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        public void Dispose() {
            // nothing need to do
        }
    }
}
