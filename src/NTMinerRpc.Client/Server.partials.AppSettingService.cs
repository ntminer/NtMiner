using NTMiner.MinerServer;
using System;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private AppSettingServiceFace() { }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse> callback) {
                AppSettingRequest request = new AppSettingRequest {
                    MessageId = Guid.NewGuid(),
                    Key = key
                };
                GetAppSettingResponse response = Request<GetAppSettingResponse>("AppSetting", "AppSetting", request);
                callback?.Invoke(response);
            }
            #endregion

            #region GetAppSettingsAsync
            public void GetAppSettingsAsync(Action<GetAppSettingsResponse> callback) {
                AppSettingsRequest request = new AppSettingsRequest {
                    MessageId = Guid.NewGuid()
                };
                GetAppSettingsResponse response = Request<GetAppSettingsResponse>("AppSetting", "AppSettings", request);
                callback?.Invoke(response);
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                SetAppSettingRequest request = new SetAppSettingRequest() {
                    MessageId = messageId,
                    Data = entity,
                    LoginName = LoginName,
                    Timestamp = DateTime.Now
                };
                request.SignIt(PasswordSha1);
                ResponseBase response = Request<ResponseBase>("AppSetting", "SetAppSetting", request);
                callback?.Invoke(response);
            }
            #endregion
        }
    }
}
