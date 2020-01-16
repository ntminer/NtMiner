using NTMiner.Controllers;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class UserServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IUserController>();

            private readonly string _host;
            private readonly int _port;
            public UserServiceFace(string host, int port) {
                _host = host;
                _port = port;
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
                    DataResponse<List<UserData>> response = RpcRoot.Post<DataResponse<List<UserData>>>(_host, _port, SControllerName, nameof(IUserController.Users), request, request, timeout: 2000);
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
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IUserController.AddUser), request, request, callback);
            }
            #endregion

            #region UpdateUserAsync
            public void UpdateUserAsync(UserData userData, Action<ResponseBase, Exception> callback) {
                DataRequest<UserData> request = new DataRequest<UserData>() {
                    Data = userData
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IUserController.UpdateUser), request, request, callback);
            }
            #endregion

            #region RemoveUserAsync
            public void RemoveUserAsync(string loginName, Action<ResponseBase, Exception> callback) {
                DataRequest<String> request = new DataRequest<String>() {
                    Data = loginName
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IUserController.RemoveUser), request, request, callback);
            }
            #endregion
        }
    }
}
