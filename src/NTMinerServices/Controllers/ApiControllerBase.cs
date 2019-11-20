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
                return Net.Util.IsInnerIp(ClientIp);
            }
        }

        protected new IUser User {
            get {
                if (string.IsNullOrEmpty(LoginName)) {
                    return null;
                }
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
                        string[] parts = query.Split('&');
                        foreach (var item in parts) {
                            string[] pair = item.Split('=');
                            if (pair.Length == 2) {
                                _queryString.Add(pair[0], pair[1]);
                            }
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
                return QueryString["sign"];
            }
        }

        protected ulong Timestamp {
            get {
                string t = QueryString["timestamp"];
                if (string.IsNullOrEmpty(t)) {
                    return 0;
                }
                return ulong.TryParse(t, out ulong v) ? v : 0;
            }
        }
    }
}
