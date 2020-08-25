using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class AppSettingService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IAppSettingController>();

        internal AppSettingService() {
        }

        #region GetJsonFileVersionAsync
        public void GetJsonFileVersionAsync(string key, Action<ServerStateResponse> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.GetJsonFileVersion), request, (string line, Exception e) => {
                callback?.Invoke(ServerStateResponse.FromLine(line));
            }, timeountMilliseconds: 10 * 1000);
        }
        #endregion

        #region SetAppSettingAsync
        public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<AppSettingData> request = new DataRequest<AppSettingData>() {
                Data = entity
            };
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.SetAppSetting), data: request, callback);
        }
        #endregion
    }
}
