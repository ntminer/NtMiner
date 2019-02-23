using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerClientController : ApiController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]RestartWindowsRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.RestartWindowsAsync(request.ClientIp, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region ShutdownWindows
        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]ShutdownWindowsRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.ShutdownWindowsAsync(request.ClientIp, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region OpenNTMiner
        [HttpPost]
        public ResponseBase OpenNTMiner([FromBody]OpenNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.OpenNTMinerAsync(request.ClientIp, request.WorkId, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RestartNTMiner
        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]RestartNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.RestartNTMinerAsync(request.ClientIp, request.WorkId, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region UpgradeNTMiner
        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.UpgradeNTMinerAsync(request.ClientIp, request.NTMinerFileName, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region CloseNTMiner
        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]CloseNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.ClientIp)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                NTMinerDaemonService.Instance.CloseNTMinerAsync(request.ClientIp, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StartMine
        [HttpPost]
        public ResponseBase StartMine([FromBody]StartMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                MinerClientService.Instance.StartMineAsync(request.ClientIp, request.WorkId, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region StopMine
        [HttpPost]
        public ResponseBase StopMine([FromBody]StopMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                MinerClientService.Instance.StopMineAsync(request.ClientIp, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region SetClientMinerProfileProperty
        [HttpPost]
        public ResponseBase SetClientMinerProfileProperty([FromBody]SetClientMinerProfilePropertyRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                MinerClientService.Instance.SetMinerProfilePropertyAsync(request.ClientIp, request.PropertyName, request.Value, null);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
