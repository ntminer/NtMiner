using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerGroupController : ApiControllerBase, IMinerGroupController {
        #region MinerGroups
        [HttpPost]
        public DataResponse<List<MinerGroupData>> MinerGroups([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<MinerGroupData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<MinerGroupData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.MinerGroupSet.GetAll();
                return DataResponse<List<MinerGroupData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<MinerGroupData>>>(e.Message);
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
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.MinerGroupSet.AddOrUpdate(request.Data);
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
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                IMinerGroup minerGroup = HostRoot.Instance.MinerGroupSet.GetMinerGroup(request.Data);
                if (minerGroup == null) {
                    return ResponseBase.Ok();
                }
                if (HostRoot.Instance.ClientSet.IsAnyClientInGroup(request.Data)) {
                    return ResponseBase.ClientError($"组{minerGroup.Name}下有矿机，请先移除矿机再做删除操作");
                }
                HostRoot.Instance.MinerGroupSet.Remove(request.Data);
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
