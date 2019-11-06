using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class MineWorkServiceFace {
            public static readonly MineWorkServiceFace Instance = new MineWorkServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IMineWorkController>();

            private MineWorkServiceFace() { }

            #region GetMineWorks
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<MineWorkData>> response = Post<DataResponse<List<MineWorkData>>>(SControllerName, nameof(IMineWorkController.MineWorks), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, timeout: 2000);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<MineWorkData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<MineWorkData>();
                }
            }
            #endregion

            #region AddOrUpdateMineWorkAsync
            public void AddOrUpdateMineWorkAsync(MineWorkData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = AddOrUpdateMineWork(entity);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region AddOrUpdateMineWork
            public ResponseBase AddOrUpdateMineWork(MineWorkData entity) {
                entity.ModifiedOn = DateTime.Now;
                DataRequest<MineWorkData> request = new DataRequest<MineWorkData> {
                    Data = entity
                };
                ResponseBase response = Post<ResponseBase>(SControllerName, nameof(IMineWorkController.AddOrUpdateMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request);
                return response;
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid> {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IMineWorkController.RemoveMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region ExportMineWorkAsync
            public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
                ExportMineWorkRequest request = new ExportMineWorkRequest {
                    MineWorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                PostAsync(SControllerName, nameof(IMineWorkController.ExportMineWork), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region GetLocalJson
            public string GetLocalJson(Guid workId) {
                try {
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        Data = workId
                    };
                    var response = Post<DataResponse<string>>(SControllerName, nameof(IMineWorkController.GetLocalJson), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request);
                    if (response != null) {
                        return response.Data;
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return string.Empty;
            }
            #endregion
        }
    }
}
