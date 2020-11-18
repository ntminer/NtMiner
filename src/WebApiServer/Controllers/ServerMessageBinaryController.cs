using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ServerMessageBinaryController : ApiControllerBase, IServerMessageBinaryController<HttpResponseMessage> {
        [Role.Public]
        [HttpPost]
        public HttpResponseMessage ServerMessages(ServerMessagesRequest request) {
            DataResponse<List<ServerMessageData>> response = ServerMessageController.DoServerMessages(request);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = WebApiRoot.BinaryContentType;
            return httpResponseMessage;
        }
    }
}
