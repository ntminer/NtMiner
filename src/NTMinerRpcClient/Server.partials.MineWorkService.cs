using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class MineWorkServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IMineWorkController>();

            public MineWorkServiceFace() {
            }

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
                    DataResponse<List<MineWorkData>> response = RpcRoot.Post<DataResponse<List<MineWorkData>>>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IMineWorkController.MineWorks), request, request, timeout: 2000);
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
                ResponseBase response = RpcRoot.Post<ResponseBase>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IMineWorkController.AddOrUpdateMineWork), request, request);
                return response;
            }
            #endregion

            #region RemoveMineWorkAsync
            public void RemoveMineWorkAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid> {
                    Data = id
                };
                RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IMineWorkController.RemoveMineWork), request, request, callback);
            }
            #endregion

            #region ExportMineWorkAsync
            public void ExportMineWorkAsync(Guid workId, string localJson, string serverJson, Action<ResponseBase, Exception> callback) {
                ExportMineWorkRequest request = new ExportMineWorkRequest {
                    MineWorkId = workId,
                    LocalJson = localJson,
                    ServerJson = serverJson
                };
                RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IMineWorkController.ExportMineWork), request, request, callback);
            }
            #endregion

            #region GetLocalJson
            public string GetLocalJson(Guid workId) {
                try {
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        Data = workId
                    };
                    var response = RpcRoot.Post<DataResponse<string>>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, SControllerName, nameof(IMineWorkController.GetLocalJson), request, request);
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
