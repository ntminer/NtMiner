using NTMiner.User;

namespace NTMiner.Controllers {
    public interface IUserController {
        #region Admin的操作
        /// <summary>
        /// 验证Admin
        /// </summary>
        QueryUsersResponse QueryUsers(QueryUsersRequest request);
        /// <summary>
        /// 验证Admin
        /// </summary>
        ResponseBase RemoveUser(DataRequest<string> request);
        /// <summary>
        /// 验证Admin
        /// </summary>
        ResponseBase EnableUser(DataRequest<string> request);
        /// <summary>
        /// 验证Admin
        /// </summary>
        ResponseBase DisableUser(DataRequest<string> request);
        /// <summary>
        /// 验证Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponseBase AddAdminRole(DataRequest<string> request);

        ResponseBase RemoveAdminRole(DataRequest<string> request);
        #endregion

        DataResponse<LoginedUser> Login(object request);
        /// <summary>
        /// 判断给定用户名的用户是否已经存在
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        DataResponse<bool> IsLoginNameExist(string loginName);
        /// <summary>
        /// 用户注册
        /// </summary>
        ResponseBase SignUp(SignUpRequest request);
        /// <summary>
        /// 验证User，不具有修改密码的功能，修改密码走ChangePassword过程
        /// </summary>
        ResponseBase UpdateUser(DataRequest<UserUpdateInput> request);
        /// <summary>
        /// 验证User
        /// </summary>
        ResponseBase ChangePassword(ChangePasswordRequest request);
    }
}
