using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.Services {
    public class AppSettingService : IAppSettingService {
        public GetAppSettingResponse GetAppSetting(Guid messageId, string key) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSetting(key);
                return new GetAppSettingResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingsResponse GetAppSettings(Guid messageId, string[] keys) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSettings(keys);
                return new GetAppSettingsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingsResponse GetAllAppSettings(Guid messageId) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAllAppSettings();
                return new GetAppSettingsResponse(data);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public ResponseBase SetAppSetting(SetAppSettingRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                if (string.IsNullOrEmpty(request.LoginName)) {
                    return ResponseBase.InvalidInput(request.MessageId, "登录名不能为空");
                }
                if (!HostRoot.Current.UserSet.TryGetKey(request.LoginName, out IUser key)) {
                    return ResponseBase.Forbidden(request.MessageId);
                }
                if (!request.Timestamp.IsInTime()) {
                    return ResponseBase.Expired(request.MessageId);
                }
                if (request.Data == null) {
                    return ResponseBase.InvalidInput(request.MessageId);
                }
                if (request.Sign != request.GetSign(key.Password)) {
                    return ResponseBase.Forbidden(request.MessageId, "签名验证未通过");
                }
                HostRoot.Current.AppSettingSet.SetAppSetting(request.Data);
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
