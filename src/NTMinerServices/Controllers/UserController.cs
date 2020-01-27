using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserController : ApiControllerBase, IUserController {
        #region Users
        [HttpPost]
        public DataResponse<List<UserData>> Users([FromBody]DataRequest<Guid?> request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<UserData>>>("参数错误");
            }
            try {
                if (!request.Data.HasValue) {
                    // request.Data是ClientId，如果未传ClientId表示是群控客户端，群控客户端获取用户表需验证身份
                    if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<UserData>> response)) {
                        return response;
                    }
                }
                var data = HostRoot.Instance.UserSet.AsEnumerable().Cast<UserData>().ToList();
                if (request.Data.HasValue) {
                    // request.Data是ClientId，挖矿端获取用户表无需验证身份但获取到的用户表的密码是加密的和群控客户端获取到的不同的
                    data = data.Select(a => new UserData(a)).ToList();
                    foreach (var user in data) {
                        user.Password = HashUtil.Sha1(HashUtil.Sha1(user.Password) + request.Data.Value);
                    }
                }
                return DataResponse<List<UserData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<UserData>>>(e.Message);
            }
        }
        #endregion

        #region AddUser
        [HttpPost]
        public ResponseBase AddUser([FromBody]DataRequest<UserData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new AddUserCommand(request.Data));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region UpdateUser
        [HttpPost]
        public ResponseBase UpdateUser([FromBody]DataRequest<UserData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new UpdateUserCommand(request.Data));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveUser
        [HttpPost]
        public ResponseBase RemoveUser([FromBody]DataRequest<String> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new RemoveUserCommand(request.Data));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        public ResponseBase ChangePassword([FromBody]ChangePasswordRequest request) {
            if (request == null || string.IsNullOrEmpty(request.LoginName)
                || string.IsNullOrEmpty(request.OldPassword)
                || string.IsNullOrEmpty(request.NewPassword)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                IUser user = HostRoot.Instance.UserSet.GetUser(request.LoginName);
                if (user == null) {
                    return ResponseBase.ClientError($"登录名不存在");
                }
                if (user.Password == request.NewPassword) {
                    return ResponseBase.Ok();
                }
                if (user.Password != request.OldPassword) {
                    return ResponseBase.ClientError($"旧密码不正确");
                }
                VirtualRoot.Execute(new ChangePasswordCommand(request.LoginName, request.OldPassword, request.NewPassword, request.Description));
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
