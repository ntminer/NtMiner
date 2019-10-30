using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class KernelOutputKeywordServiceFace {
            public static readonly KernelOutputKeywordServiceFace Instance = new KernelOutputKeywordServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IKernelOutputKeywordController>();

            private KernelOutputKeywordServiceFace() { }

            #region GetVersion
            public string GetVersion() {
                try {
                    string version = Post<string>(SControllerName, nameof(IKernelOutputKeywordController.GetVersion), null, null);
                    return version;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return string.Empty;
                }
            }
            #endregion

            #region GetKernelOutputKeywords
            public List<KernelOutputKeywordData> GetKernelOutputKeywords() {
                try {
                    AppSettingsRequest request = new AppSettingsRequest {
                    };
                    DataResponse<List<KernelOutputKeywordData>> response = Post<DataResponse<List<KernelOutputKeywordData>>>(SControllerName, nameof(IKernelOutputKeywordController.KernelOutputKeywords), null, request);
                    if (response.IsSuccess()) {
                        return response.Data;
                    }
                    return new List<KernelOutputKeywordData>();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return new List<KernelOutputKeywordData>();
                }
            }
            #endregion

            #region SetKernelOutputKeywordAsync
            public void SetKernelOutputKeywordAsync(KernelOutputKeywordData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<KernelOutputKeywordData> request = new DataRequest<KernelOutputKeywordData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IKernelOutputKeywordController.SetKernelOutputKeyword), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
