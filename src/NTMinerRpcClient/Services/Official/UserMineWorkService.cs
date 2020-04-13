using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class UserMineWorkService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IUserMineWorkController>();

        public UserMineWorkService() {
        }

        #region GetMineWorksAsync
        public void GetMineWorksAsync(Action<DataResponse<List<UserMineWorkData>>, Exception> callback) {
            SignRequest request = new SignRequest {
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.MineWorks), data: request, callback, timeountMilliseconds: 2000);
        }
        #endregion

        #region AddOrUpdateMineWorkAsync
        public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase, Exception> callback) {
            entity.ModifiedOn = DateTime.Now;
            DataRequest<MineWorkData> request = new DataRequest<MineWorkData> {
                Data = entity
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.AddOrUpdateMineWork), data: request, callback);
        }
        #endregion

        #region RemoveMineWorkAsync
        public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid> {
                Data = id
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.RemoveMineWork), data: request, callback);
        }
        #endregion

        #region ExportMineWorkAsync
        public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
            ExportMineWorkRequest request = new ExportMineWorkRequest {
                MineWorkId = workId,
                LocalJson = localJson,
                ServerJson = serverJson
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.ExportMineWork), data: request, callback);
        }
        #endregion

        #region GetLocalJsonAsync
        public void GetLocalJsonAsync(Guid workId, Action<DataResponse<string>, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = workId
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.GetLocalJson), data: request, callback);
        }
        #endregion

        public void GetWorkJsonAsync(Guid workId, Guid clientId, Action<GetWorkJsonResponse, Exception> callback) {
            GetWorkJsonRequest request = new GetWorkJsonRequest() {
                WorkId = workId,
                ClientId = clientId
            };
            RpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserMineWorkController.GetWorkJson), data: request, callback);
        }
    }
}
