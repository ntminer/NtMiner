using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.ServerService {
    public class MinerGroupService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IMinerGroupController>();

        public MinerGroupService() {
        }

        #region GetMinerGroups
        // TODO:异步化
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns></returns>
        public List<MinerGroupData> GetMinerGroups() {
            try {
                SignRequest request = new SignRequest {
                };
                DataResponse<List<MinerGroupData>> response = RpcRoot.Post<DataResponse<List<MinerGroupData>>>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IMinerGroupController.MinerGroups), request, request, timeout: 2000);
                if (response != null && response.Data != null) {
                    return response.Data;
                }
                return new List<MinerGroupData>();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return new List<MinerGroupData>();
            }
        }
        #endregion

        #region AddOrUpdateMinerGroupAsync
        public void AddOrUpdateMinerGroupAsync(MinerGroupData entity, Action<ResponseBase, Exception> callback) {
            entity.ModifiedOn = DateTime.Now;
            DataRequest<MinerGroupData> request = new DataRequest<MinerGroupData> {
                Data = entity
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IMinerGroupController.AddOrUpdateMinerGroup), request, request, callback);
        }
        #endregion

        #region RemoveMinerGroupAsync
        public void RemoveMinerGroupAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IMinerGroupController.RemoveMinerGroup), request, request, callback);
        }
        #endregion
    }
}
