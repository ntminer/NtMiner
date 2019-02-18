using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerClientController : ApiController {
        #region RestartWindows
        [HttpPost]
        public ResponseBase RestartWindows([FromBody]RestartWindowsRequest request) {
            // TODO:
            throw new NotImplementedException();
        }
        #endregion

        #region ShutdownWindows
        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]ShutdownWindowsRequest request) {
            // TODO:
            throw new NotImplementedException();
        }
        #endregion

        #region OpenNTMiner
        [HttpPost]
        public ResponseBase OpenNTMiner([FromBody]OpenNTMinerRequest request) {
            // TODO:
            throw new NotImplementedException();
        }
        #endregion

        #region RestartNTMiner
        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]RestartNTMinerRequest request) {
            // TODO:
            throw new NotImplementedException();
        }
        #endregion

        #region UpgradeNTMiner
        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            // TODO:
            throw new NotImplementedException();
        }
        #endregion

        #region CloseNTMiner
        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]CloseNTMinerRequest request) {
            // TODO:
            throw new NotImplementedException();
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
                // TODO:
                throw new NotImplementedException();
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
                // TODO:更新挖矿状态和算力
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
