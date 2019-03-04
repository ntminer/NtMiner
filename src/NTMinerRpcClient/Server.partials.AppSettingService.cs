using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private AppSettingServiceFace() { }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AppSettingRequest request = new AppSettingRequest {
                            MessageId = Guid.NewGuid(),
                            Key = key
                        };
                        GetAppSettingResponse response = Request<GetAppSettingResponse>("AppSetting", "AppSetting", request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetAppSettingsAsync
            public List<AppSettingData> GetAppSettings() {
                try {
                    AppSettingsRequest request = new AppSettingsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    GetAppSettingsResponse response = Request<GetAppSettingsResponse>("AppSetting", "AppSettings", request);
                    return response.Data;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<AppSettingData>();
                }
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetAppSettingRequest request = new SetAppSettingRequest() {
                            Data = entity,
                            LoginName = SingleUser.LoginName
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("AppSetting", "SetAppSetting", request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion
        }
    }
}
