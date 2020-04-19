using NTMiner.Core;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserMinerGroupController : ApiControllerBase, IUserMinerGroupController {
        #region MinerGroups
        [HttpPost]
        public DataResponse<List<UserMinerGroupData>> MinerGroups([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<UserMinerGroupData>>>("参数错误");
            }
            try {
                if (!IsValidUser(request, out DataResponse<List<UserMinerGroupData>> response, out UserData user)) {
                    return response;
                }
                var data = WebApiRoot.MinerGroupSet.GetsByLoginName(user.LoginName);
                return DataResponse<List<UserMinerGroupData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<UserMinerGroupData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateMinerGroup
        [HttpPost]
        public ResponseBase AddOrUpdateMinerGroup([FromBody]DataRequest<MinerGroupData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                    return response;
                }
                WebApiRoot.MinerGroupSet.AddOrUpdate(request.Data.ToUserMinerGroup(user.LoginName));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveMinerGroup
        [HttpPost]
        public ResponseBase RemoveMinerGroup([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                    return response;
                }
                IUserMinerGroup minerGroup = WebApiRoot.MinerGroupSet.GetById(request.Data);
                if (minerGroup == null) {
                    return ResponseBase.Ok();
                }
                if (minerGroup.LoginName != user.LoginName) {
                    return ResponseBase.Forbidden("无权操作");
                }
                if (WebApiRoot.ClientDataSet.IsAnyClientInGroup(request.Data)) {
                    return ResponseBase.ClientError($"组{minerGroup.Name}下有矿机，请先移除矿机再做删除操作");
                }
                WebApiRoot.MinerGroupSet.RemoveById(request.Data);
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
