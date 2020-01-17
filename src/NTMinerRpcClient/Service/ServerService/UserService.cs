using NTMiner.Controllers;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.ServerService {
    public class UserServiceFace {
        private static readonly string SControllerName = RpcRoot.GetControllerName<IUserController>();

        public UserServiceFace() {
        }

        #region GetUsers
        // TODO:异步化
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<UserData> GetUsers(Guid? clientId) {
            try {
                DataRequest<Guid?> request = new DataRequest<Guid?> {
                    Data = clientId
                };
                DataResponse<List<UserData>> response = RpcRoot.Post<DataResponse<List<UserData>>>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IUserController.Users), request, request, timeout: 2000);
                if (response != null && response.Data != null) {
                    return response.Data;
                }
                return new List<UserData>();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return new List<UserData>();
            }
        }
        #endregion

        #region AddUserAsync
        public void AddUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
            DataRequest<UserData> request = new DataRequest<UserData>() {
                Data = userData
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IUserController.AddUser), request, request, callback);
        }
        #endregion

        #region UpdateUserAsync
        public void UpdateUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
            DataRequest<UserData> request = new DataRequest<UserData>() {
                Data = userData
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IUserController.UpdateUser), request, request, callback);
        }
        #endregion

        #region RemoveUserAsync
        public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
            DataRequest<String> request = new DataRequest<String>() {
                Data = loginName
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IUserController.RemoveUser), request, request, callback);
        }
        #endregion
    }
}
