using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WrapperMinerClientController : ApiController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]WrapperRequest<RestartWindowsRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
        public ResponseBase OpenNTMiner([FromBody]WrapperRequest<OpenNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.OpenNTMiner(request.InnerRequest);
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
        public ResponseBase RestartNTMiner([FromBody]WrapperRequest<RestartNTMinerRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.RestartNTMiner(request.InnerRequest);
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
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
        public ResponseBase StartMine([FromBody]WrapperRequest<StartMineRequest> request) {
            if (request == null || request.InnerRequest == null || string.IsNullOrEmpty(request.InnerRequest.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                response = Client.NTMinerDaemonService.StartMine(request.InnerRequest);
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
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
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
