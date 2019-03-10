using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.IO;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WrapperMinerClientController : ApiController, IWrapperMinerClientController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.RestartWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region ShutdownWindows
        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.ShutdownWindows(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpgradeNTMiner
        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]WrapperRequest<UpgradeNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.UpgradeNTMiner(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StartMine
        [HttpPost]
        public ResponseBase StartMine([FromBody]WrapperRequest<WorkRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                IUser user = HostRoot.Current.UserSet.GetUser(request.InnerRequest.LoginName);
                if (user == null) {
                    return ResponseBase.Forbidden(request.MessageId, $"登录名为{request.InnerRequest.LoginName}的用户不存在");
                }
                string localJson = string.Empty, serverJson = string.Empty;
                Guid workId = request.InnerRequest.WorkId;
                if (workId != Guid.Empty) {
                    localJson = SpecialPath.ReadMineWorkLocalJsonFile(workId);
                    serverJson = SpecialPath.ReadMineWorkServerJsonFile(workId);
                }
                WorkRequest innerRequest = new WorkRequest {
                    MessageId = request.InnerRequest.MessageId,
                    LoginName = request.InnerRequest.LoginName,
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                innerRequest.SignIt(HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.ClientId));
                response = Client.NTMinerDaemonService.StartMine(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RestartNTMiner
        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]WrapperRequest<WorkRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                IUser user = HostRoot.Current.UserSet.GetUser(request.InnerRequest.LoginName);
                if (user == null) {
                    return ResponseBase.Forbidden(request.MessageId, $"登录名为{request.InnerRequest.LoginName}的用户不存在");
                }
                string localJson = string.Empty, serverJson = string.Empty;
                Guid workId = request.InnerRequest.WorkId;
                if (workId != Guid.Empty) {
                    localJson = SpecialPath.ReadMineWorkLocalJsonFile(workId);
                    serverJson = SpecialPath.ReadMineWorkServerJsonFile(workId);
                }
                WorkRequest innerRequest = new WorkRequest {
                    LoginName = request.InnerRequest.LoginName,
                    MessageId = request.InnerRequest.MessageId,
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                innerRequest.SignIt(HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.ClientId));
                response = Client.NTMinerDaemonService.RestartNTMiner(request.ClientIp, innerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StopMine
        [HttpPost]
        public ResponseBase StopMine([FromBody]WrapperRequest<SignatureRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.MinerClientService.StopMine(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetClientMinerProfileProperty
        [HttpPost]
        public ResponseBase SetClientMinerProfileProperty([FromBody]WrapperRequest<SetClientMinerProfilePropertyRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.MinerClientService.SetMinerProfileProperty(request.ClientIp, request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
