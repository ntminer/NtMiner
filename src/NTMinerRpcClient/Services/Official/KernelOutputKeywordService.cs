using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class KernelOutputKeywordService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<IKernelOutputKeywordController>();

        public KernelOutputKeywordService() {
        }

        #region GetKernelOutputKeywords
        public void GetKernelOutputKeywords(Action<KernelOutputKeywordsResponse, Exception> callback) {
            object request = new object();
            JsonRpcRoot.PostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IKernelOutputKeywordController.KernelOutputKeywords), request, callback);
        }
        #endregion

        #region AddOrUpdateKernelOutputKeywordAsync
        public void AddOrUpdateKernelOutputKeywordAsync(KernelOutputKeywordData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<KernelOutputKeywordData> request = new DataRequest<KernelOutputKeywordData>() {
                Data = entity
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IKernelOutputKeywordController.AddOrUpdateKernelOutputKeyword), data: request, callback);
        }
        #endregion

        #region RemoveKernelOutputKeyword
        public void RemoveKernelOutputKeyword(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(IKernelOutputKeywordController.RemoveKernelOutputKeyword), data: request, callback);
        }
        #endregion
    }
}
