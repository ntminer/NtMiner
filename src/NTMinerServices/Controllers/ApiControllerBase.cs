using System;
using System.Collections.Specialized;
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

        protected new IUser User {
            get {
                return HostRoot.Instance.UserSet.GetUser(LoginName);
            }
        }

        private NameValueCollection _queryString;
        private NameValueCollection QueryString {
            get {
                if (_queryString == null) {
                    _queryString = new NameValueCollection();
                    string query = Request.RequestUri.Query;
                    if (!string.IsNullOrEmpty(query)) {
                        query = query.Substring(1);
                        string[] parts = query.Split('=');
                        for (int i = 0; i < parts.Length - 1; i = i + 2) {
                            _queryString.Add(parts[i], parts[i + 1]);
                        }
                    }
                }
                return _queryString;
            }
        }

        protected string LoginName {
            get {
                return QueryString["loginName"];
            }
        }

        protected string Sign {
            get {
                return QueryString["Sign"];
            }
        }

        protected DateTime Timestamp {
            get {
                string t = QueryString["timestamp"];
                if (string.IsNullOrEmpty(t)) {
                    return NTMiner.Timestamp.UnixBaseTime;
                }
                return ulong.TryParse(t, out ulong v) ? NTMiner.Timestamp.FromTimestamp(v) : NTMiner.Timestamp.UnixBaseTime;
            }
        }
    }
}
