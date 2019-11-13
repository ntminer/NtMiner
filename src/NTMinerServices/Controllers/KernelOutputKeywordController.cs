using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class KernelOutputKeywordController : ApiControllerBase, IKernelOutputKeywordController {
        [HttpPost]
        public string GetVersion() {
            string version = string.Empty;
            try {
                if (!HostRoot.Instance.AppSettingSet.TryGetAppSetting(NTKeyword.KernelOutputKeywordVersionAppSettingKey, out IAppSetting data) || data.Value == null) {
                    version = string.Empty;
                }
                else {
                    version = data.Value.ToString();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return version;
        }

        [HttpPost]
        public DataResponse<List<KernelOutputKeywordData>> KernelOutputKeywords(KernelOutputKeywordsRequest request) {
            try {
                var data = HostRoot.Instance.KernelOutputKeywordSet;
                return DataResponse<List<KernelOutputKeywordData>>.Ok(data.Select(a => KernelOutputKeywordData.Create(a)).ToList());
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<KernelOutputKeywordData>>>(e.Message);
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
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase SetKernelOutputKeyword(DataRequest<KernelOutputKeywordData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(request.Data));
                VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData {
                    Key = NTKeyword.KernelOutputKeywordVersionAppSettingKey,
                    Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
