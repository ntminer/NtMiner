using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class NTMinerFileService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<INTMinerFileController>();

        internal NTMinerFileService() {
        }

        #region GetNTMinerFilesAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerFilesAsync(DateTime timestamp, Action<NTMinerFilesResponse, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(INTMinerFileController.GetNTMinerFiles),
                new NTMinerFilesRequest {
                    Timestamp = timestamp
                },
                callback);
        }
        #endregion

        #region AddOrUpdateNTMinerFileAsync
        // ReSharper disable once InconsistentNaming
        public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<NTMinerFileData> request = new DataRequest<NTMinerFileData>() {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(INTMinerFileController.AddOrUpdateNTMinerFile),
                data: request,
                callback);
        }
        #endregion

        #region RemoveNTMinerFileAsync
        // ReSharper disable once InconsistentNaming
        public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost,
                RpcRoot.OfficialServerPort,
                _controllerName,
                nameof(INTMinerFileController.RemoveNTMinerFile),
                data: request,
                callback);
        }
        #endregion
    }
}
