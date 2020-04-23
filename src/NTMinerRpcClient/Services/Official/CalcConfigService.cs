using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class CalcConfigService {
        private readonly string _controllerName = RpcRoot.GetControllerName<ICalcConfigController>();

        public CalcConfigService() {
        }

        #region GetCalcConfigsAsync
        public void GetCalcConfigsAsync(Action<List<CalcConfigData>> callback) {
            RpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(ICalcConfigController.CalcConfigs), null, (DataResponse<List<CalcConfigData>> response, Exception e) => {
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
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(ICalcConfigController.SaveCalcConfigs), data: request, callback);
        }
        #endregion
    }
}
