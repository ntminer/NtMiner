using NTMiner.Controllers;
using NTMiner.Core;
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
                    };
                    DataResponse<List<AppSettingData>> response = Post<DataResponse<List<AppSettingData>>>(SControllerName, nameof(IAppSettingController.AppSettings), null, request);
                    if (response.IsSuccess()) {
                        return response.Data;
                    }
                    return new List<AppSettingData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<AppSettingData>();
                }
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<AppSettingData> request = new DataRequest<AppSettingData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IAppSettingController.SetAppSetting), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region SetAppSettingsAsync
            public void SetAppSettingsAsync(List<AppSettingData> entities, Action<ResponseBase, Exception> callback) {
                DataRequest<List<AppSettingData>> request = new DataRequest<List<AppSettingData>>() {
                    Data = entities
                };
                PostAsync(SControllerName, nameof(IAppSettingController.SetAppSettings), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
