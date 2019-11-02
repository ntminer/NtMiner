using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WrapperMinerClientController : ApiControllerBase, IWrapperMinerClientController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]WrapperRequest<SignRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.RestartWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region ShutdownWindows
        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]WrapperRequest<SignRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.ShutdownWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region UpgradeNTMiner
        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]WrapperRequest<UpgradeNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.UpgradeNTMiner(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region StartMine
        [HttpPost]
        public ResponseBase StartMine([FromBody]WrapperRequest<WorkRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                IClientData clientData = HostRoot.Instance.ClientSet.GetByObjectId(request.ObjectId);
                if (clientData == null) {
                    return ResponseBase.ClientError("给定标识的矿机不存在");
                }
                string localJson = string.Empty, serverJson = string.Empty;
                Guid workId = request.InnerRequest.WorkId;
                if (workId != Guid.Empty) {
                    localJson = SpecialPath.ReadMineWorkLocalJsonFile(workId).Replace("{{MinerName}}", clientData.MinerName);
                    serverJson = SpecialPath.ReadMineWorkServerJsonFile(workId);
                }
                WorkRequest innerRequest = new WorkRequest {
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                response = Client.NTMinerDaemonService.StartMine(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region RestartNTMiner
        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]WrapperRequest<WorkRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                IClientData clientData = HostRoot.Instance.ClientSet.GetByObjectId(request.ObjectId);
                if (clientData == null) {
                    return ResponseBase.ClientError("给定标识的矿机不存在");
                }
                string localJson = string.Empty, serverJson = string.Empty;
                Guid workId = request.InnerRequest.WorkId;
                if (workId != Guid.Empty) {
                    localJson = SpecialPath.ReadMineWorkLocalJsonFile(workId).Replace("{{MinerName}}", clientData.MinerName);
                    serverJson = SpecialPath.ReadMineWorkServerJsonFile(workId);
                }
                WorkRequest innerRequest = new WorkRequest {
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                response = Client.NTMinerDaemonService.RestartNTMiner(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region StopMine
        [HttpPost]
        public ResponseBase StopMine([FromBody]WrapperRequest<SignRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.StopMine(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion

        #region SetClientMinerProfileProperty
        [HttpPost]
        public ResponseBase SetClientMinerProfileProperty([FromBody]WrapperRequest<SetClientMinerProfilePropertyRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.MinerClientService.SetMinerProfileProperty(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                string message = e.GetInnerMessage();
                Logger.ErrorDebugLine(message, e);
                return ResponseBase.ServerError(message);
            }
        }
        #endregion
    }
}
