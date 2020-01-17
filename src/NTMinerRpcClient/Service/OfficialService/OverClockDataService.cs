using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.OfficialService {
    public class OverClockDataService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IOverClockDataController>();
        private readonly string _host;
        private readonly int _port;

        public OverClockDataService(string host, int port) {
            _host = host;
            _port = port;
        }

        #region GetOverClockDatasAsync
        public void GetOverClockDatasAsync(Action<DataResponse<List<OverClockData>>, Exception> callback) {
            try {
                OverClockDatasRequest request = new OverClockDatasRequest {
                };
                RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IOverClockDataController.OverClockDatas), request, callback);
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
            RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IOverClockDataController.AddOrUpdateOverClockData), request, request, callback);
        }
        #endregion

        #region RemoveOverClockDataAsync
        public void RemoveOverClockDataAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.PostAsync(_host, _port, _controllerName, nameof(IOverClockDataController.RemoveOverClockData), request, request, callback);
        }
        #endregion
    }
}
