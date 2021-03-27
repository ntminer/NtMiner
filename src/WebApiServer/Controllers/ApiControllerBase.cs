using NTMiner.User;
using System.Web.Http;

namespace NTMiner.Controllers {
    [CountActionFilter]
    public abstract class ApiControllerBase : ApiController {
        protected new UserData User {
            get {
                // 在UserAttribute ActionFilter里赋值的
                return (UserData)ControllerContext.RouteData.Values["_user"];
            }
        }

        private string _remoteIp = null;
        protected string RemoteIp {
            get {
                if (_remoteIp == null) {
                    _remoteIp = Request.GetRemoteIp();
                    if (_remoteIp == null) {
                        _remoteIp = string.Empty;
                    }
                }
                return _remoteIp;
            }
        }
    }
}
