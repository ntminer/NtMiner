using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class CalcConfigBinaryController : ApiControllerBase, ICalcConfigBinaryController<HttpResponseMessage> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coinCodes">逗号分割</param>
        /// <returns></returns>
        [Role.Public]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage CalcConfigs() {
            DataResponse<List<CalcConfigData>> response = CalcConfigController.DoCalcConfigs(string.Empty);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = AppRoot.BinaryContentType;
            return httpResponseMessage;
        }

        [Role.Public]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Query(string coinCodes) {
            DataResponse<List<CalcConfigData>> response = CalcConfigController.DoCalcConfigs(coinCodes);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new ByteArrayContent(VirtualRoot.BinarySerializer.Serialize(response))
            };
            httpResponseMessage.Content.Headers.ContentType = AppRoot.BinaryContentType;
            return httpResponseMessage;
        }
    }
}
