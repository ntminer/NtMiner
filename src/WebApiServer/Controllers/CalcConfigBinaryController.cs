using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class CalcConfigBinaryController : ApiControllerBase, ICalcConfigBinaryController<HttpResponseMessage> {
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage CalcConfigs() {
            DataResponse<List<CalcConfigData>> response = CalcConfigController.DoCalcConfigs();
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return httpResponseMessage;
        }
    }
}
