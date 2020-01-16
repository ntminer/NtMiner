using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IAppSettingController>();

            private readonly string _host;
            private readonly int _port;
            public AppSettingServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetAppSettings
            public List<AppSettingData> GetAppSettings() {
                try {
                    AppSettingsRequest request = new AppSettingsRequest {
                    };
                    DataResponse<List<AppSettingData>> response = RpcRoot.Post<DataResponse<List<AppSettingData>>>(_host, _port, SControllerName, nameof(IAppSettingController.AppSettings), request);
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
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IAppSettingController.SetAppSetting), request, request, callback);
            }
            #endregion

            #region SetAppSettingsAsync
            public void SetAppSettingsAsync(List<AppSettingData> entities, Action<ResponseBase, Exception> callback) {
                DataRequest<List<AppSettingData>> request = new DataRequest<List<AppSettingData>>() {
                    Data = entities
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IAppSettingController.SetAppSettings), request, request, callback);
            }
            #endregion
        }
    }
}
