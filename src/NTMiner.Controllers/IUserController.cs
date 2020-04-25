using NTMiner.User;

namespace NTMiner.Controllers {
    public interface IUserController {
        #region Admin的操作
        /// <summary>
        /// 需签名
        /// </summary>
        QueryUsersResponse QueryUsers(QueryUsersRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveUser(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase EnableUser(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase DisableUser(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddAdminRole(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveAdminRole(DataRequest<string> request);
        #endregion

        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<LoginedUser> Login(object request);
        DataResponse<bool> IsLoginNameExist(string loginName);
        ResponseBase SignUp(SignUpRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase UpdateUser(DataRequest<UserUpdateInput> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase ChangePassword(ChangePasswordRequest request);
    }
}
