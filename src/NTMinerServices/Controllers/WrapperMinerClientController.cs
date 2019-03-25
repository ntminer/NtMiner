using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WrapperMinerClientController : ApiController, IWrapperMinerClientController {
        private string ClientIp {
            get {
                return Request.GetWebClientIp();
            }
        }

        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.RestartWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region ShutdownWindows
        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.ShutdownWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
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
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.UpgradeNTMiner(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
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
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                IClientData clientData = HostRoot.Current.ClientSet.GetByObjectId(request.ObjectId);
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
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                response = Client.NTMinerDaemonService.StartMine(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
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
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                IClientData clientData = HostRoot.Current.ClientSet.GetByObjectId(request.ObjectId);
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
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                response = Client.NTMinerDaemonService.RestartNTMiner(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region StopMine
        [HttpPost]
        public ResponseBase StopMine([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.StopMine(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
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
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                response = Client.MinerClientService.SetMinerProfileProperty(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
