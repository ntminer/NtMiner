using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class ColumnsShowServiceFace {
            public static readonly ColumnsShowServiceFace Instance = new ColumnsShowServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IColumnsShowController>();

            private ColumnsShowServiceFace() { }

            #region GetColumnsShows
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<ColumnsShowData> GetColumnsShows() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<ColumnsShowData>> response = Post<DataResponse<List<ColumnsShowData>>>(SControllerName, nameof(IColumnsShowController.ColumnsShows), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<ColumnsShowData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<ColumnsShowData>();
                }
            }
            #endregion

            #region AddOrUpdateColumnsShowAsync
            public void AddOrUpdateColumnsShowAsync(ColumnsShowData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<ColumnsShowData> request = new DataRequest<ColumnsShowData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IColumnsShowController.AddOrUpdateColumnsShow), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveColumnsShowAsync
            public void RemoveColumnsShowAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IColumnsShowController.RemoveColumnsShow), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
