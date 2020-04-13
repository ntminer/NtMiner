using NTMiner.User;
using System.Collections.Specialized;
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

        private ClientSignData _clientSign;
        protected ClientSignData ClientSign {
            get {
                if (_clientSign == null) {
                    var queryString = new NameValueCollection();
                    string query = Request.RequestUri.Query;
                    if (!string.IsNullOrEmpty(query)) {
                        query = query.Substring(1);
                        string[] parts = query.Split('&');
                        foreach (var item in parts) {
                            string[] pair = item.Split('=');
                            if (pair.Length == 2) {
                                queryString.Add(pair[0], pair[1]);
                            }
                        }
                    }
                    long timestamp = 0;
                    string t = queryString["timestamp"];
                    if (!string.IsNullOrEmpty(t)) {
                        long.TryParse(t, out timestamp);
                    }
                    _clientSign = new ClientSignData(queryString["loginName"], queryString["sign"], timestamp);
                }
                return _clientSign;
            }
        }

        protected bool IsValidUser<TResponse>(ISignableData data, out TResponse response) where TResponse : ResponseBase, new() {
            return IsValidUser(data, out response, out _);
        }

        protected bool IsValidUser<TResponse>(ISignableData data, out TResponse response, out UserData user) where TResponse : ResponseBase, new() {
            user = null;
            if (!WebApiRoot.UserSet.IsReadied) {
                string message = "服务器用户集启动中，请稍后";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            ClientSignData query = ClientSign;
            if (!Timestamp.IsInTime(query.Timestamp)) {
                response = ResponseBase.Expired<TResponse>();
                return false;
            }
            // 对于User来说LoginName可以是LoginName、Email、Mobile
            if (!string.IsNullOrEmpty(query.LoginName)) {
                user = WebApiRoot.UserSet.GetUser(UserId.Create(query.LoginName));
            }
            if (user == null) {
                string message = "用户不存在";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            if (user.IsAdmin()) {
                response = null;
                return true;
            }
            string mySign = RpcUser.CalcSign(user.LoginName, user.Password, query.Timestamp, data);
            if (query.Sign != mySign) {
                string message = "用户登录名或密码错误";
                response = ResponseBase.Forbidden<TResponse>(message);
                return false;
            }
            response = null;
            return true;
        }

        protected bool IsValidAdmin<TResponse>(ISignableData data, out TResponse response) where TResponse : ResponseBase, new() {
            return IsValidAdmin(data, out response, out _);
        }

        protected bool IsValidAdmin<TResponse>(ISignableData data, out TResponse response, out UserData user) where TResponse : ResponseBase, new() {
            user = null;
            if (!WebApiRoot.UserSet.IsReadied) {
                string message = "服务器用户集启动中，请稍后";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            ClientSignData query = ClientSign;
            if (!Timestamp.IsInTime(query.Timestamp)) {
                response = ResponseBase.Expired<TResponse>();
                return false;
            }
            if (!string.IsNullOrEmpty(query.LoginName)) {
                user = WebApiRoot.UserSet.GetUser(query.UserId);
            }
            if (user == null && !string.IsNullOrEmpty(query.LoginName)) {
                user = WebApiRoot.UserSet.GetUser(query.UserId);
            }
            if (user == null) {
                string message = "用户不存在";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            else if (!user.IsAdmin()) {
                string message = "对不起，您不是超管";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            string mySign = RpcUser.CalcSign(user.LoginName, user.Password, query.Timestamp, data);
            if (query.Sign != mySign) {
                string message = "登录名或密码错误";
                response = ResponseBase.Forbidden<TResponse>(message);
                Write.DevDebug(() => $"{message} sign:{query.Sign} mySign:{mySign}");
                return false;
            }
            response = null;
            return true;
        }
    }
}
