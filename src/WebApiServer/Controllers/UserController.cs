using NTMiner.User;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class UserController : ApiControllerBase, IUserController {
        #region Admin的操作
        /// <summary>
        /// 验证Admin
        /// </summary>
        [HttpPost]
        public QueryUsersResponse QueryUsers([FromBody]QueryUsersRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryUsersResponse>("参数错误");
            }
            if (!IsValidAdmin(request, out QueryUsersResponse response)) {
                return response;
            }
            try {
                var data = WebApiRoot.UserSet.QueryUsers(request, out int total);
                return new QueryUsersResponse {
                    StateCode = 200,
                    ReasonPhrase = "Ok",
                    Description = "成功",
                    Data = data,
                    Total = total
                };
            }
            catch (Exception e) {
                return ResponseBase.ServerError<QueryUsersResponse>(e.Message);
            }
        }

        /// <summary>
        /// 验证Admin
        /// </summary>
        [HttpPost]
        public ResponseBase RemoveUser([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<string> response, out UserData admin)) {
                return response;
            }
            if (request.Data == Role.RoleEnum.admin.GetName()) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作admin");
            }
            if (request.Data == admin.LoginName) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能删除自己");
            }
            try {
                WebApiRoot.UserSet.Remove(request.Data);
                return ResponseBase.Ok("删除成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }

        /// <summary>
        /// 验证Admin
        /// </summary>
        [HttpPost]
        public ResponseBase EnableUser([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<string> response, out UserData admin)) {
                return response;
            }
            if (request.Data == Role.RoleEnum.admin.GetName()) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作admin");
            }
            if (request.Data == admin.LoginName) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能启用自己");
            }
            try {
                WebApiRoot.UserSet.Enable(request.Data);
                return ResponseBase.Ok("启用成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }

        /// <summary>
        /// 验证Admin
        /// 应在界面上告知用户被禁用的账户下的所有矿机的外网群控将掉线
        /// </summary>
        [HttpPost]
        public ResponseBase DisableUser([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<string> response, out UserData admin)) {
                return response;
            }
            if (request.Data == Role.RoleEnum.admin.GetName()) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作admin");
            }
            if (request.Data == admin.LoginName) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能禁用自己");
            }
            try {
                WebApiRoot.UserSet.Disable(request.Data);
                return ResponseBase.Ok("禁用成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }


        [HttpPost]
        public ResponseBase AddAdminRole([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<string> response, out UserData admin)) {
                return response;
            }
            if (request.Data == Role.RoleEnum.admin.GetName()) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作admin");
            }
            if (request.Data == admin.LoginName) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作自己");
            }
            try {
                WebApiRoot.UserSet.AddAdminRole(request.Data);
                return ResponseBase.Ok("设置成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }


        [HttpPost]
        public ResponseBase RemoveAdminRole([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<string> response, out UserData admin)) {
                return response;
            }
            if (request.Data == Role.RoleEnum.admin.GetName()) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作admin");
            }
            if (request.Data == admin.LoginName) {
                return ResponseBase.InvalidInput<DataResponse<string>>("不能操作自己");
            }
            try {
                WebApiRoot.UserSet.RemoveAdminRole(request.Data);
                return ResponseBase.Ok("移除超管角色成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region Login
        [HttpPost]
        public DataResponse<LoginedUser> Login([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<LoginedUser>>("参数错误");
            }
            try {
                if (!IsValidUser(request, out DataResponse<LoginedUser> response, out UserData user)) {
                    return response;
                }
                return DataResponse<LoginedUser>.Ok(user.ToLoginedUserData());
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<LoginedUser>>(e.Message);
            }
        }
        #endregion

        #region IsLoginNameExist
        [HttpGet]
        public DataResponse<bool> IsLoginNameExist([FromUri]string loginName) {
            bool isExist;
            if (string.IsNullOrEmpty(loginName)) {
                isExist = true;
            }
            else {
                isExist = WebApiRoot.UserSet.Contains(loginName);
            }
            return new DataResponse<bool> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = isExist
            };
        }
        #endregion

        #region SignUp
        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost]
        public ResponseBase SignUp([FromBody]SignUpRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            if (!VirtualRoot.IsValidLoginName(request.LoginName, out string message)) {
                return ResponseBase.InvalidInput(message);
            }
            if (request.ActionCaptchaId == Guid.Empty 
                || string.IsNullOrEmpty(request.ActionCaptcha)
                || !WebApiRoot.CaptchaSet.IsValid(request.ActionCaptchaId, base.MinerIp, request.ActionCaptcha)) {
                return ResponseBase.InvalidInput("验证码错误");
            }
            if (!WebApiRoot.UserSet.IsReadied) {
                return ResponseBase.InvalidInput("服务器用户集启动中，请稍后");
            }
            if (WebApiRoot.UserSet.Contains(request.LoginName)) {
                return ResponseBase.InvalidInput("登录名已被占用，请更换");
            }

            // TODO:验证邮箱验证码，注册时如果填了邮箱则验证邮箱的验证码，更新用户信息时如果变动了邮箱则验证邮箱验证码。
            // 验证验证码的存在性以及邮箱地址和验证码的对应关系的正确性而不是只验证验证码的存在性。

            // TODO:验证手机验证码，注册时如果填写了手机则验证手机验证码，更新用户信息时如果变动了手机则验证手机验证码。
            // 验证验证码的存在性以及手机和验证码的对应关系的正确性而不是只验证验证码的存在性。
            var key = Cryptography.RSAHelper.GetRASKey();
            UserData userData = request.ToUserData(key.PublicKey, key.PrivateKey);
            WebApiRoot.UserSet.Add(userData);

            return ResponseBase.Ok("注册成功");
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// 验证User，不具有修改密码的功能，修改密码走ChangePassword过程
        /// </summary>
        [HttpPost]
        public ResponseBase UpdateUser([FromBody]DataRequest<UserUpdateInput> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                return response;
            }
            if (request.Data.ActionCaptchaId == Guid.Empty
                || string.IsNullOrEmpty(request.Data.ActionCaptcha)
                || !WebApiRoot.CaptchaSet.IsValid(request.Data.ActionCaptchaId, base.MinerIp, request.Data.ActionCaptcha)) {
                return ResponseBase.InvalidInput("验证码错误");
            }

            // TODO:验证邮箱验证码，注册时如果填了邮箱则验证邮箱的验证码，更新用户信息时如果变动了邮箱则验证邮箱验证码。
            // 验证验证码的存在性以及邮箱地址和验证码的对应关系的正确性而不是只验证验证码的存在性。
            // TODO:如果未填写邮箱使用数据库记录的邮箱填充request.Data.Email确保下层不会误将邮箱更新为空

            // TODO:验证手机验证码，注册时如果填写了手机则验证手机验证码，更新用户信息时如果变动了手机则验证手机验证码。
            // 验证验证码的存在性以及手机和验证码的对应关系的正确性而不是只验证验证码的存在性。
            // TODO:如果未填写手机使用数据库记录的手机填充request.Data.Email确保下层不会误将手机更新为空
            try {
                WebApiRoot.UserSet.Update(request.Data);
                return ResponseBase.Ok("更新成功");
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// 验证User
        /// </summary>
        [HttpPost]
        public ResponseBase ChangePassword([FromBody]ChangePasswordRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            if (string.IsNullOrEmpty(request.NewPassword)) {
                return ResponseBase.InvalidInput("密码不能为空");
            }
            if (!IsValidUser(request, out ResponseBase response, out UserData user)) {
                return response;
            }
            WebApiRoot.UserSet.ChangePassword(user.LoginName, request.NewPassword);
            return ResponseBase.Ok("密码修改成功");
        }
        #endregion
    }
}
