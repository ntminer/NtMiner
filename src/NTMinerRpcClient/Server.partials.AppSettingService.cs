using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IAppSettingController>();

            private AppSettingServiceFace() { }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AppSettingRequest request = new AppSettingRequest {
                            MessageId = Guid.NewGuid(),
                            Key = key
                        };
                        GetAppSettingResponse response = Request<GetAppSettingResponse>(s_controllerName, nameof(IAppSettingController.AppSetting), request);
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
                    GetAppSettingsResponse response = Request<GetAppSettingsResponse>(s_controllerName, nameof(IAppSettingController.AppSettings), request);
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
                        ResponseBase response = Request<ResponseBase>(s_controllerName, nameof(IAppSettingController.SetAppSetting), request);
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
