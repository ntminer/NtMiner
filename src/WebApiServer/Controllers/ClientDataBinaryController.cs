using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ClientDataBinaryController : ApiControllerBase, IClientDataBinaryController<HttpResponseMessage> {
        [Role.User]
        [HttpPost]
        public HttpResponseMessage QueryClients([FromBody]QueryClientsRequest request) {
            QueryClientsResponse response = ClientDataController.DoQueryClients(request, User);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return httpResponseMessage;
        }

        // 这个是供WsServer节点走内网以管理员的名义向WebApiServer获取挖矿端列表的接口
        [Role.Admin]
        [HttpPost]
        public HttpResponseMessage QueryClientsForWs([FromBody]QueryClientsForWsRequest request) {
            QueryClientsResponse response;
            if (request == null) {
                response = ResponseBase.InvalidInput<QueryClientsResponse>("参数错误");
            }
            else {
                request.PagingTrim();
                try {
                    var user = WebApiRoot.UserSet.GetUser(UserId.CreateLoginNameUserId(request.LoginName));
                    if (user == null) {
                        response = ResponseBase.InvalidInput<QueryClientsResponse>("用户不存在");
                    }
                    else {
                        var data = WebApiRoot.ClientDataSet.QueryClients(
                            user,
                            request,
                            out int total,
                            out List<CoinSnapshotData> latestSnapshots,
                            out int totalOnlineCount,
                            out int totalMiningCount) ?? new List<ClientData>();
                        response = QueryClientsResponse.Ok(data, total, latestSnapshots, totalMiningCount, totalOnlineCount);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError<QueryClientsResponse>(e.Message);
                }
            }
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return httpResponseMessage;
        }
    }
}
