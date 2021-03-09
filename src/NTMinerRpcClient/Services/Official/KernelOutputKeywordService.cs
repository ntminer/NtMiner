using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class KernelOutputKeywordService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IKernelOutputKeywordController>();

        internal KernelOutputKeywordService() {
        }

        #region GetKernelOutputKeywords
        public void GetKernelOutputKeywords(Action<KernelOutputKeywordsResponse, Exception> callback) {
            object request = new object();
            RpcRoot.JsonRpc.PostAsync(
                RpcRoot.OfficialServerHost, 
                RpcRoot.OfficialServerPort, 
                _controllerName, 
                nameof(IKernelOutputKeywordController.KernelOutputKeywords), 
                request, 
                callback);
        }
        #endregion

        #region AddOrUpdateKernelOutputKeywordAsync
        public void AddOrUpdateKernelOutputKeywordAsync(KernelOutputKeywordData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<KernelOutputKeywordData> request = new DataRequest<KernelOutputKeywordData>() {
                Data = entity
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost, 
                RpcRoot.OfficialServerPort, 
                _controllerName, 
                nameof(IKernelOutputKeywordController.AddOrUpdateKernelOutputKeyword), 
                data: request, 
                callback);
        }
        #endregion

        #region RemoveKernelOutputKeyword
        public void RemoveKernelOutputKeyword(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.JsonRpc.SignPostAsync(
                RpcRoot.OfficialServerHost, 
                RpcRoot.OfficialServerPort, 
                _controllerName, 
                nameof(IKernelOutputKeywordController.RemoveKernelOutputKeyword), 
                data: request, 
                callback);
        }
        #endregion
    }
}
