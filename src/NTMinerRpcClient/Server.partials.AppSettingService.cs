using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IAppSettingController>();

            private AppSettingServiceFace() { }

            public void GetTimeAsync(Action<DateTime, Exception> callback) {
                GetAsync(SControllerName, nameof(IAppSettingController.GetTime), null, callback);
            }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<DataResponse<AppSettingData>, Exception> callback) {
                AppSettingRequest request = new AppSettingRequest {
                    MessageId = Guid.NewGuid(),
                    Key = key
                };
                PostAsync(SControllerName, nameof(IAppSettingController.AppSetting), request, callback);
            }
            #endregion

            #region GetAppSettings
            public List<AppSettingData> GetAppSettings() {
                try {
                    AppSettingsRequest request = new AppSettingsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    DataResponse<List<AppSettingData>> response = Post<DataResponse<List<AppSettingData>>>(SControllerName, nameof(IAppSettingController.AppSettings), request);
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
                DataRequest<AppSettingData> request = new DataRequest<AppSettingData>() {
                    Data = entity,
                    LoginName = SingleUser.LoginName
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IAppSettingController.SetAppSetting), request, callback);
            }
            #endregion
        }
    }
}
