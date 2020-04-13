using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
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
            RpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IAppSettingController.GetJsonFileVersion), request, (string text, Exception e) => {
                string jsonFileVersion = string.Empty;
                string minerClientVersion = string.Empty;
                long time = Timestamp.GetTimestamp();
                long messageTimestamp = 0;
                long kernelOutputKeywordTimestamp = 0;
                if (!string.IsNullOrEmpty(text)) {
                    text = text.Trim();
                    string[] parts = text.Split(new char[] { '|' });
                    if (parts.Length > 0) {
                        jsonFileVersion = parts[0];
                    }
                    if (parts.Length > 1) {
                        minerClientVersion = parts[1];
                    }
                    if (parts.Length > 2) {
                        long.TryParse(parts[2], out time);
                    }
                    if (parts.Length > 3) {
                        long.TryParse(parts[3], out messageTimestamp);
                    }
                    if (parts.Length > 4) {
                        long.TryParse(parts[4], out kernelOutputKeywordTimestamp);
                    }
                }
                callback?.Invoke(new ServerState {
                    JsonFileVersion = jsonFileVersion,
                    MinerClientVersion = minerClientVersion,
                    Time = time,
                    MessageTimestamp = messageTimestamp,
                    OutputKeywordTimestamp = kernelOutputKeywordTimestamp
                });
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
