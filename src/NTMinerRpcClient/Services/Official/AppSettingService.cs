using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Services.Official {
    public class AppSettingService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IAppSettingController>();

        public AppSettingService() {
        }

        public void GetTimeAsync(Action<DateTime> callback) {
            RpcRoot.GetAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.GetTime), null, callback: (DateTime datetime, Exception e) => {
                callback?.Invoke(datetime);
            }, timeountMilliseconds: 10 * 1000);
        }

        #region GetJsonFileVersionAsync
        public void GetJsonFileVersionAsync(string key, Action<ServerState> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            RpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.GetJsonFileVersion), request, (string line, Exception e) => {
                callback?.Invoke(ServerState.FromLine(line));
            }, timeountMilliseconds: 10 * 1000);
        }
        #endregion

        #region SetAppSettingAsync
        public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<AppSettingData> request = new DataRequest<AppSettingData>() {
                Data = entity
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.SetAppSetting), data: request, callback);
        }
        #endregion
    }
}
