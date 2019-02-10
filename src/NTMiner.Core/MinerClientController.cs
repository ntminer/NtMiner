using NTMiner.MinerClient;
using NTMiner.User;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MinerClientController : ApiController {
        [HttpPost]
        public ResponseBase AddUser([FromBody]AddUserRequest request) {
            if (request == null || string.IsNullOrEmpty(request.LoginName) || string.IsNullOrEmpty(request.Password)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                IUser user;
                if (NTMinerRoot.Current.UserSet.TryGetKey(request.LoginName, out user)) {
                    if (user.Password == request.Password) {
                        return ResponseBase.Ok(request.MessageId);
                    }
                    else {
                        return ResponseBase.ClientError(request.MessageId, $"密码错误");
                    }
                }
                VirtualRoot.Execute(new AddUserCommand(request));
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand());
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]StartMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StartMine(request.WorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]StopMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StopMineAsync();
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetMinerProfileProperty([FromBody]SetMinerProfilePropertyRequest request) {
            if (request == null || string.IsNullOrEmpty(request.PropertyName)) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.SetMinerProfileProperty(request.PropertyName, request.Value);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
    }
}
