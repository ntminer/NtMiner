using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class OfficialServer {
        public partial class KernelOutputKeywordServiceFace {
            public static readonly KernelOutputKeywordServiceFace Instance = new KernelOutputKeywordServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IKernelOutputKeywordController>();

            private KernelOutputKeywordServiceFace() { }

            #region GetKernelOutputKeywords
            public void GetKernelOutputKeywords(Action<DataResponse<List<KernelOutputKeywordData>>, Exception> callback) {
                try {
                    KernelOutputKeywordsRequest request = new KernelOutputKeywordsRequest {
                    };
                    PostAsync(SControllerName, nameof(IKernelOutputKeywordController.KernelOutputKeywords), null, request, callback);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(null, e);
                }
            }
            #endregion

            #region SetKernelOutputKeywordAsync
            public void SetKernelOutputKeywordAsync(KernelOutputKeywordData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<KernelOutputKeywordData> request = new DataRequest<KernelOutputKeywordData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IKernelOutputKeywordController.AddOrUpdateKernelOutputKeyword), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveKernelOutputKeyword
            public void RemoveKernelOutputKeyword(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IKernelOutputKeywordController.RemoveKernelOutputKeyword), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
