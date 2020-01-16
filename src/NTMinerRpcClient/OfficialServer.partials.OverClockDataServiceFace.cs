using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class OfficialServer {
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
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IOverClockDataController.AddOrUpdateOverClockData), request, request, callback);
            }
            #endregion

            #region RemoveOverClockDataAsync
            public void RemoveOverClockDataAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IOverClockDataController.RemoveOverClockData), request, request, callback);
            }
            #endregion
        }
    }
}
