using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class KernelOutputKeywordController : ApiControllerBase, IKernelOutputKeywordController {
        [HttpPost]
        public KernelOutputKeywordsResponse KernelOutputKeywords(KernelOutputKeywordsRequest request) {
            try {
                var data = HostRoot.Instance.KernelOutputKeywordSet;
                return KernelOutputKeywordsResponse.Ok(data.AsEnumerable().Select(a => KernelOutputKeywordData.Create(a)).ToList(), NTMiner.Timestamp.GetTimestamp(HostRoot.Instance.KernelOutputKeywordTimestamp));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<KernelOutputKeywordsResponse>(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RemoveKernelOutputKeyword(DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new RemoveKernelOutputKeywordCommand(request.Data));
                HostRoot.Instance.UpdateKernelOutputKeywordTimestamp(DateTime.Now);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase AddOrUpdateKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(request.Data));
                HostRoot.Instance.UpdateKernelOutputKeywordTimestamp(DateTime.Now);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
