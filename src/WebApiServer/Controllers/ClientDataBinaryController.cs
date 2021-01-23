using NTMiner.Core.MinerServer;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ClientDataBinaryController : ApiControllerBase, IClientDataBinaryController<HttpResponseMessage> {
        // 留存一长段时间，这个方法最初旧的群控客户端调用了
        [Role.User]
        [HttpPost]
        public HttpResponseMessage QueryClients([FromBody]QueryClientsRequest request) {
            QueryClientsResponse response = ClientDataController.DoQueryClients(request, User);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = AppRoot.BinaryContentType;
            return httpResponseMessage;
        }
    }
}
