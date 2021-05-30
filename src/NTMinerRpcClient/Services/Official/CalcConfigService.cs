using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class CalcConfigService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<ICalcConfigController>();

        internal CalcConfigService() {
        }

        #region GetCalcConfigsAsync
        public void GetCalcConfigsAsync(Action<List<CalcConfigData>> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(ICalcConfigController.CalcConfigs),
                null,
                callback: (DataResponse<List<CalcConfigData>> response, Exception e) => {
                    if (response.IsSuccess()) {
                        callback?.Invoke(response.Data);
                    }
                    else {
                        callback?.Invoke(new List<CalcConfigData>());
                    }
                }, timeountMilliseconds: 10 * 1000);
        }

        public void QueryCalcConfigsAsync(List<string> coinCodes, Action<List<CalcConfigData>> callback) {
            if (coinCodes == null) {
                coinCodes = new List<string>();
            }
            RpcRoot.JsonRpc.GetAsync(
                _controllerName,
                nameof(ICalcConfigController.Query),
                new Dictionary<string, string> {
                    ["coinCodes"] = string.Join(",", coinCodes)
                },
                callback: (DataResponse<List<CalcConfigData>> response, Exception e) => {
                    if (response.IsSuccess()) {
                        callback?.Invoke(response.Data);
                    }
                    else {
                        callback?.Invoke(new List<CalcConfigData>());
                    }
                }, timeountMilliseconds: 10 * 1000);
        }
        #endregion

        #region SaveCalcConfigsAsync
        public void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase, Exception> callback) {
            if (configs == null || configs.Count == 0) {
                return;
            }
            SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                Data = configs
            };
            RpcRoot.JsonRpc.SignPostAsync(
                _controllerName,
                nameof(ICalcConfigController.SaveCalcConfigs),
                data: request,
                callback);
        }
        #endregion
    }
}
