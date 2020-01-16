using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class OfficialServer {
        public class ControlCenterServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IControlCenterController>();

            private readonly string _host;
            private readonly int _port;

            public ControlCenterServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetCalcConfigsAsync
            public void GetCalcConfigsAsync(Action<List<CalcConfigData>> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        CalcConfigsRequest request = new CalcConfigsRequest {
                        };
                        RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IControlCenterController.CalcConfigs), request, (DataResponse<List<CalcConfigData>> response, Exception e) => {
                            if (response.IsSuccess()) {
                                callback?.Invoke(response.Data);
                            }
                            else {
                                callback?.Invoke(new List<CalcConfigData>());
                            }
                        }, timeountMilliseconds: 10 * 1000);
                    }
                    catch (Exception e) {
                        callback?.Invoke(new List<CalcConfigData>());
                        Logger.ErrorDebugLine(e);
                    }
                });
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
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IControlCenterController.SaveCalcConfigs), request, request, callback);
            }
            #endregion
        }
    }
}
