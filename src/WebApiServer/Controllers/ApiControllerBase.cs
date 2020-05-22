using NTMiner.User;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace NTMiner.Controllers {
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
                    _remoteIp = GetWebClientIp();
                    if (_remoteIp == null) {
                        _remoteIp = string.Empty;
                    }
                }
                return _remoteIp;
            }
        }

        private string GetWebClientIp() {
            string ip;
            if (Request.Properties.TryGetValue("MS_HttpContext", out object obj) && obj is HttpContextWrapper context) {
                ip = context.Request.UserHostAddress;
            }
            else if (Request.Properties.TryGetValue(RemoteEndpointMessageProperty.Name, out obj) && obj is RemoteEndpointMessageProperty prop) {
                ip = prop.Address;
            }
            else if (HttpContext.Current != null) {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            else {
                ip = string.Empty;
            }
            if (ip == "::1") {
                ip = "127.0.0.1";
            }
            return ip;
        }
    }
}
