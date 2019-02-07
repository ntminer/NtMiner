using System;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private AppSettingServiceFace() { }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse> callback) {
                GetAppSettingResponse response = Request<GetAppSettingResponse>("AppSetting", "AppSetting", new { messageId = Guid.NewGuid(), key });
                callback?.Invoke(response);
            }
            #endregion

            #region GetAppSettingsAsync
            public void GetAppSettingsAsync(Action<GetAppSettingsResponse> callback) {
                GetAppSettingsResponse response = Request<GetAppSettingsResponse>("AppSetting", "AppSettings", new { messageId = Guid.NewGuid() });
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
