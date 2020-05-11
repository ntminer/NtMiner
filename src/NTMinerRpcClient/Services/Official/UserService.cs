using NTMiner.Controllers;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class UserService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IUserController>();

        public UserService() {
        }

        #region LoginAsync
        public void LoginAsync(string loginName, string password, Action<DataResponse<LoginedUser>, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.Login), RpcUser.GetSignData(loginName, password), new object(), callback);
        }
        #endregion

        public void IsLoginNameExistAsync(string loginName, Action<bool> callback) {
            JsonRpcRoot.GetAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.IsLoginNameExist), new Dictionary<string, string> {
                ["loginName"] = loginName
            }, (DataResponse<bool> response, Exception e) => {
                callback?.Invoke(response.IsSuccess() && response.Data);
            }, timeountMilliseconds: 5000);
        }

        public void QueryUsersAsync(QueryUsersRequest request, Action<QueryUsersResponse, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.QueryUsers), data: request, callback: (QueryUsersResponse response, Exception e) => {
                if (response != null && response.Data != null && response.Data.Count != 0) {
                    foreach (var user in response.Data) {
                        user.Password = Cryptography.QuickUtil.TextDecrypt(Convert.FromBase64String(user.Password), RpcRoot.RpcUser.Password);
                        user.PrivateKey = Cryptography.QuickUtil.TextDecrypt(Convert.FromBase64String(user.PrivateKey), RpcRoot.RpcUser.Password);
                    }
                }
                callback?.Invoke(response, e);
            }, timeountMilliseconds: 2000);
        }

        public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.RemoveUser), new DataRequest<string> {
                Data = loginName
            }, callback);
        }

        public void EnableUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.EnableUser), new DataRequest<string> {
                Data = loginName
            }, callback);
        }

        public void DisableUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.DisableUser), new DataRequest<string> {
                Data = loginName
            }, callback);
        }

        public void AddAdminRoleAsync(string loginName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.AddAdminRole), new DataRequest<string> {
                Data = loginName
            }, callback);
        }

        public void RemoveAdminRoleAsync(string loginName, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.RemoveAdminRole), new DataRequest<string> {
                Data = loginName
            }, callback);
        }

        public void SignUpAsync(SignUpRequest data, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.SignUp), data, callback);
        }

        public void UpdateUserAsync(UserUpdateData data, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.UpdateUser), data, callback);
        }

        public void ChangePasswordAsync(ChangePasswordRequest request, Action<ResponseBase, Exception> callback) {
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IUserController.ChangePassword), request, callback);
        }
    }
}
