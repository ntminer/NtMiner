using NTMiner.User;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace NTMiner.Controllers {
    public abstract class ApiControllerBase : ApiController {
        public class ClientSignData {
            public ClientSignData(string loginName, string sign, long timestamp) {
                this.LoginName = loginName;
                this.Sign = sign;
                this.Timestamp = timestamp;
            }

            public string LoginName { get; private set; }
            public string Sign { get; private set; }
            public long Timestamp { get; private set; }

            private UserId _userId;
            public UserId UserId {
                get {
                    if (_userId == null) {
                        _userId = UserId.Create(this.LoginName);
                    }
                    return _userId;
                }
            }
        }

        private string _minerIp = null;
        protected string MinerIp {
            get {
                if (_minerIp == null) {
                    _minerIp = GetWebClientIp();
                    if (_minerIp == null) {
                        _minerIp = string.Empty;
                    }
                }
                return _minerIp;
            }
        }

        private string GetWebClientIp() {
            string ip;
            if (Request.Properties.ContainsKey("MS_HttpContext")) {
                ip = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name)) {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name];
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

        protected new UserData User {
            get {
                return (UserData)ControllerContext.RouteData.Values["_user"];
            }
        }
    }
}
