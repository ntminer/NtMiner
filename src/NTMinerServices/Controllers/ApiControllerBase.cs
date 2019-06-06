using System.Web.Http;

namespace NTMiner.Controllers {
    public abstract class ApiControllerBase : ApiController {
        protected string ClientIp {
            get {
                return Request.GetWebClientIp();
            }
        }

        protected bool IsInnerIp {
            get {
                return Ip.Util.IsInnerIp(ClientIp);
            }
        }
    }
}
