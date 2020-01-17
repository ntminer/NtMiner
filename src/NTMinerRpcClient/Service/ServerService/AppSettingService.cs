using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.ServerService {
    public class AppSettingServiceFace {
        private static readonly string SControllerName = RpcRoot.GetControllerName<IAppSettingController>();

        public AppSettingServiceFace() {
        }

        #region GetAppSettings
        public List<AppSettingData> GetAppSettings() {
            try {
                AppSettingsRequest request = new AppSettingsRequest {
                };
                DataResponse<List<AppSettingData>> response = RpcRoot.Post<DataResponse<List<AppSettingData>>>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IAppSettingController.AppSettings), request);
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
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IAppSettingController.SetAppSetting), request, request, callback);
        }
        #endregion

        #region SetAppSettingsAsync
        public void SetAppSettingsAsync(List<AppSettingData> entities, Action<ResponseBase, Exception> callback) {
            DataRequest<List<AppSettingData>> request = new DataRequest<List<AppSettingData>>() {
                Data = entities
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IAppSettingController.SetAppSettings), request, request, callback);
        }
        #endregion
    }
}
