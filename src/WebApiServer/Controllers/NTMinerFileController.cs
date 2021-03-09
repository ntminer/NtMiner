using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class NTMinerFileController : ApiControllerBase, INTMinerFileController {
        [Role.Public]
        [HttpPost]
        public List<NTMinerFileData> NTMinerFiles() {
            return AppRoot.NTMinerFileSet.AsEnumerable().ToList();
        }

        [Role.Public]
        [HttpPost]
        public NTMinerFilesResponse GetNTMinerFiles([FromBody] NTMinerFilesRequest request) {
            if (request == null) {
                return new NTMinerFilesResponse {
                    Description = "参数错误",
                    StateCode = 491,
                    ReasonPhrase = "InvalidInput",
                    Timestamp = DateTime.MinValue
                };
            }
            if (request.Timestamp <= Timestamp.UnixBaseTime || request.Timestamp.AddSeconds(1) < AppRoot.NTMinerFileSet.LastChangedOn) {
                return NTMinerFilesResponse.Ok(AppRoot.NTMinerFileSet.AsEnumerable().ToList(), AppRoot.NTMinerFileSet.LastChangedOn);
            }
            return NTMinerFilesResponse.Ok(new List<NTMinerFileData>(), AppRoot.NTMinerFileSet.LastChangedOn);
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase AddOrUpdateNTMinerFile([FromBody] DataRequest<NTMinerFileData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.NTMinerFileSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveNTMinerFile([FromBody] DataRequest<Guid> request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.NTMinerFileSet.RemoveById(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
