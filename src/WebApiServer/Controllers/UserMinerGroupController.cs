using NTMiner.Core;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserMinerGroupController : ApiControllerBase, IUserMinerGroupController {
        #region MinerGroups
        [Role.User]
        [HttpPost]
        public DataResponse<List<UserMinerGroupData>> MinerGroups([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<UserMinerGroupData>>>("参数错误");
            }
            try {
                var data = AppRoot.MinerGroupSet.GetsByLoginName(User.LoginName);
                return DataResponse<List<UserMinerGroupData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<UserMinerGroupData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        [Role.User]
        [HttpPost]
        public ResponseBase AddOrUpdateMinerGroup([FromBody]DataRequest<MinerGroupData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.MinerGroupSet.AddOrUpdate(request.Data.ToUserMinerGroup(User.LoginName));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        [Role.User]
        [HttpPost]
        public ResponseBase RemoveMinerGroup([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                IUserMinerGroup minerGroup = AppRoot.MinerGroupSet.GetById(request.Data);
                if (minerGroup == null) {
                    return ResponseBase.Ok();
                }
                if (minerGroup.LoginName != User.LoginName) {
                    return ResponseBase.Forbidden("无权操作");
                }
                if (AppRoot.ClientDataSet.IsAnyClientInGroup(request.Data)) {
                    return ResponseBase.ClientError($"组{minerGroup.Name}下有矿机，请先移除矿机再做删除操作");
                }
                AppRoot.MinerGroupSet.RemoveById(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
