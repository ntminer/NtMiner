using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class IndexController : ApiController {
        private static readonly string _indexFileFullName = NTMinerRegistry.GetIndexHtmlFileFullName();

        public HttpResponseMessage Get() {
            string html = "no content";
            if (!string.IsNullOrEmpty(_indexFileFullName)) {
                html = File.ReadAllText(_indexFileFullName);
            }
            var result = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
            return result;
        }
    }
}
