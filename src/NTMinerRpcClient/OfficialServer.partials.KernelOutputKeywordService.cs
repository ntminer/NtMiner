using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;

namespace NTMiner {
    public partial class OfficialServer {
        public partial class KernelOutputKeywordServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IKernelOutputKeywordController>();

            private readonly string _host;
            private readonly int _port;

            public KernelOutputKeywordServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetKernelOutputKeywords
            public void GetKernelOutputKeywords(Action<KernelOutputKeywordsResponse, Exception> callback) {
                try {
                    KernelOutputKeywordsRequest request = new KernelOutputKeywordsRequest {
                    };
                    RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IKernelOutputKeywordController.KernelOutputKeywords), request, callback);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(null, e);
                }
            }
            #endregion

            #region AddOrUpdateKernelOutputKeywordAsync
            public void AddOrUpdateKernelOutputKeywordAsync(KernelOutputKeywordData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<KernelOutputKeywordData> request = new DataRequest<KernelOutputKeywordData>() {
                    Data = entity
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IKernelOutputKeywordController.AddOrUpdateKernelOutputKeyword), request, request, callback);
            }
            #endregion

            #region RemoveKernelOutputKeyword
            public void RemoveKernelOutputKeyword(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IKernelOutputKeywordController.RemoveKernelOutputKeyword), request, request, callback);
            }
            #endregion
        }
    }
}
