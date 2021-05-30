using NTMiner.Gpus;
using NTMiner.ServerNode;
using System;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AdminController : ApiControllerBase, IAdminController {
        [Role.Admin]
        [HttpPost]
        public ResponseBase SetClientTestId(DataRequest<ClientTestIdData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.ClientTestIdDataRedis.SetAsync(request.Data).ContinueWith(t => {
                    AppRoot.AdminMqSender.SendRefreshMinerTestId();
                });
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public QueryActionCountsResponse QueryActionCounts([FromBody] QueryActionCountsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryActionCountsResponse>("参数错误");
            }
            request.PagingTrim();
            var data = ActionCountRoot.QueryActionCounts(request, out int total);

            return QueryActionCountsResponse.Ok(data, total);
        }

        [Role.Admin]
        [HttpPost]
        public QueryGpuNameCountsResponse QueryGpuNameCounts([FromBody] QueryGpuNameCountsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryGpuNameCountsResponse>("参数错误");
            }
            request.PagingTrim();
            var data = AppRoot.GpuNameSet.QueryGpuNameCounts(request, out int total);

            return QueryGpuNameCountsResponse.Ok(data, total);
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<MqCountData[]> MqCounts() {
            var data = AppRoot.MqCountSet.GetAll();
            return DataResponse<MqCountData[]>.Ok(data);
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<MqCountData> MqCount(DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<MqCountData>>("参数错误");
            }
            MqCountData data = AppRoot.MqCountSet.GetByAppId(request.Data);
            return DataResponse<MqCountData>.Ok(data);
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<MqAppIds> MqAppIds() {
            var appIds = AppRoot.MqCountSet.GetAppIds();
            var appId = appIds.FirstOrDefault();
            MqCountData mqCountData = null;
            if (!string.IsNullOrEmpty(appId)) {
                mqCountData = AppRoot.MqCountSet.GetByAppId(appId);
            }
            return DataResponse<MqAppIds>.Ok(new MqAppIds {
                AppIds = appIds,
                AppId = appId,
                MqCountData = mqCountData
            });
        }

        [Role.Admin]
        [HttpPost]
        public TopNRemoteIpsResponse TopNRemoteIps([FromBody] DataRequest<int> request) {
            if (request == null || request.Data <= 0) {
                return ResponseBase.InvalidInput<TopNRemoteIpsResponse>("参数错误");
            }
            var data = ServerRoot.RemoteIpSet.GetTopNRemoteIpEntries(request.Data).Select(a => a.ToDto()).ToList();
            return TopNRemoteIpsResponse.Ok(data, ServerRoot.RemoteIpSet.Count);
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<WebApiServerState> GetServerState([FromBody] object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<WebApiServerState>>("参数错误");
            }
            return new DataResponse<WebApiServerState> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = AppRoot.GetServerState()
            };
        }
    }
}
