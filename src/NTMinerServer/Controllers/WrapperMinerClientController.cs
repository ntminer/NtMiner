using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.IO;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WrapperMinerClientController : ApiController, IWrapperMinerClientController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]WrapperRequest<RestartWindowsRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.RestartWindows(request.InnerRequest);
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
        public ResponseBase ShutdownWindows([FromBody]WrapperRequest<ShutdownWindowsRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.ShutdownWindows(request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region OpenNTMiner
        [HttpPost]
        public ResponseBase OpenNTMiner([FromBody]WrapperRequest<MinerServer.OpenNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
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
                Daemon.OpenNTMinerRequest innerRequest = new Daemon.OpenNTMinerRequest() {
                    ClientIp = request.InnerRequest.ClientIp,
                    MessageId = request.InnerRequest.MessageId,
                    LoginName = request.InnerRequest.LoginName,
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = request.InnerRequest.WorkId,
                    LocalJson = File.ReadAllText(SpecialPath.GetMineWorkLocalJsonFileFullName(request.InnerRequest.WorkId)),
                    ServerJson = File.ReadAllText(SpecialPath.GetMineWorkServerJsonFileFullName(request.InnerRequest.WorkId))
                };
                innerRequest.SignIt(HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.ClientId));
                response = Client.NTMinerDaemonService.OpenNTMiner(innerRequest);
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
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.UpgradeNTMiner(request.InnerRequest);
                return response;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region CloseNTMiner
        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]WrapperRequest<CloseNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.CloseNTMiner(request.InnerRequest);
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
        public ResponseBase StartMine([FromBody]WrapperRequest<MinerClient.StartMineRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
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
                Daemon.StartMineRequest innerRequest = new Daemon.StartMineRequest {
                    ClientIp = request.InnerRequest.ClientIp,
                    MessageId = request.InnerRequest.MessageId,
                    LoginName = request.InnerRequest.LoginName,
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = request.InnerRequest.WorkId,
                    LocalJson = File.ReadAllText(SpecialPath.GetMineWorkLocalJsonFileFullName(request.InnerRequest.WorkId)),
                    ServerJson = File.ReadAllText(SpecialPath.GetMineWorkServerJsonFileFullName(request.InnerRequest.WorkId))
                };
                innerRequest.SignIt(HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.ClientId));
                response = Client.NTMinerDaemonService.StartMine(innerRequest);
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
        public ResponseBase RestartNTMiner([FromBody]WrapperRequest<MinerServer.RestartNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
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
                Daemon.RestartNTMinerRequest innerRequest = new Daemon.RestartNTMinerRequest {
                    ClientIp = request.InnerRequest.ClientIp,
                    LoginName = request.InnerRequest.LoginName,
                    MessageId = request.InnerRequest.MessageId,
                    Timestamp = request.InnerRequest.Timestamp,
                    WorkId = request.InnerRequest.WorkId,
                    LocalJson = File.ReadAllText(SpecialPath.GetMineWorkLocalJsonFileFullName(request.InnerRequest.WorkId)),
                    ServerJson = File.ReadAllText(SpecialPath.GetMineWorkServerJsonFileFullName(request.InnerRequest.WorkId))
                };
                innerRequest.SignIt(HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.ClientId));
                response = Client.NTMinerDaemonService.RestartNTMiner(innerRequest);
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
        public ResponseBase StopMine([FromBody]WrapperRequest<StopMineRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.MinerClientService.StopMine(request.InnerRequest);
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
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                response = Client.MinerClientService.SetMinerProfileProperty(request.InnerRequest);
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
