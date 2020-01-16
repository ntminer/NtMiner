using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class RpcRoot {
        public static Server Server = new Server();
        public static OfficialServer OfficialServer = new OfficialServer(NTKeyword.OfficialServerHost, NTKeyword.ControlCenterPort);
        public static Client Client = new Client();

        public static void PostAsync<T>(string host, int port, string controller, string action, Action<T, Exception> callback, int timeountMilliseconds = 0) {
            PostAsync<T>(host, port, controller, action, (Dictionary<string, string>)null, null, callback, timeountMilliseconds);
        }

        public static void PostAsync<T>(string host, int port, string controller, string action, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) {
            PostAsync<T>(host, port, controller, action, (Dictionary<string, string>)null, data, callback, timeountMilliseconds);
        }

        public static void PostAsync<T>(string host, int port, string controller, string action, IGetSignData signData, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) {
            PostAsync<T>(host, port, controller, action, signData.ToQuery(), data, callback, timeountMilliseconds);
        }


        public static T Post<T>(string host, int port, string controller, string action, object data, int? timeout = null) {
            return Post<T>(host, port, controller, action, (Dictionary<string, string>)null, data, timeout);
        }

        public static T Post<T>(string host, int port, string controller, string action, IGetSignData signData, object data, int? timeout = null) {
            return Post<T>(host, port, controller, action, signData.ToQuery(), data, timeout);
        }

        public static void GetAsync<T>(string host, int port, string controller, string action, Dictionary<string, string> data, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = Create()) {
                        string queryString = string.Empty;
                        if (data != null && data.Count != 0) {
                            queryString = "?" + string.Join("&", data.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> message = client.GetAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{queryString}");
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }

        private static T Post<T>(string host, int port, string controller, string action, Dictionary<string, string> query, object data, int? timeout = null) {
            try {
                string queryString = string.Empty;
                if (query != null && query.Count != 0) {
                    queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                }
                using (HttpClient client = Create()) {
                    if (timeout.HasValue) {
                        client.Timeout = TimeSpan.FromMilliseconds(timeout.Value);
                    }
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{queryString}", data);
                    T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return default;
            }
        }

        private static void PostAsync<T>(string host, int port, string controller, string action, Dictionary<string, string> query, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = Create()) {
                        if (timeountMilliseconds != 0) {
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        string queryString = string.Empty;
                        if (query != null && query.Count != 0) {
                            queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{queryString}", data);
                        T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }
    }
}
