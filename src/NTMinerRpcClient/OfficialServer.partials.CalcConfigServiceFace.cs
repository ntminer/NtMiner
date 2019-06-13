using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class OfficialServer {
        public class OverClockDataServiceFace {
            public static readonly OverClockDataServiceFace Instance = new OverClockDataServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IOverClockDataController>();

            private OverClockDataServiceFace() { }

            #region GetOverClockDatasAsync
            public void GetOverClockDatasAsync(Action<DataResponse<List<OverClockData>>, Exception> callback) {
                try {
                    OverClockDatasRequest request = new OverClockDatasRequest {
                    };
                    PostAsync(SControllerName, nameof(IOverClockDataController.OverClockDatas), request, callback);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(null, e);
                }
            }
            #endregion

            #region AddOrUpdateOverClockDataAsync
            public void AddOrUpdateOverClockDataAsync(OverClockData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<OverClockData> request = new DataRequest<OverClockData>() {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IOverClockDataController.AddOrUpdateOverClockData), request, callback);
            }
            #endregion

            #region RemoveOverClockDataAsync
            public void RemoveOverClockDataAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IOverClockDataController.RemoveOverClockData), request, callback);
            }
            #endregion
        }
    }
}
