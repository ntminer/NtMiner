using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IAppSettingController>();

            private AppSettingServiceFace() { }

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
                    e = e.GetInnerException();
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

            #region SetAppSettingsAsync
            public void SetAppSettingsAsync(List<AppSettingData> entities, Action<ResponseBase, Exception> callback) {
                DataRequest<List<AppSettingData>> request = new DataRequest<List<AppSettingData>>() {
                    Data = entities,
                    LoginName = SingleUser.LoginName
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IAppSettingController.SetAppSettings), request, callback);
            }
            #endregion
        }
    }
}
