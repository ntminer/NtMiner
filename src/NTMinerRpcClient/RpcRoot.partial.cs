using NTMiner.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class RpcRoot {
        public static RpcUser RpcUser { get; private set; } = RpcUser.Empty;
        public static bool IsOuterNet { get; private set; } = false;
        public static bool IsInnerNet {
            get {
                return !IsOuterNet;
            }
        }
        public static bool IsLogined {
            get {
                if (RpcUser == null || RpcUser == RpcUser.Empty) {
                    return false;
                }
                return !string.IsNullOrEmpty(RpcUser.LoginName) && !string.IsNullOrEmpty(RpcUser.Password);
            }
        }

        public static void SetRpcUser(RpcUser rpcUser) {
            if (RpcUser != null) {
                RpcUser.Logout();
            }
            RpcUser = rpcUser;
        }

        public static void SetIsOuterNet(bool value) {
            bool isChanged = IsOuterNet != value;
            IsOuterNet = value;
            if (isChanged) {
                VirtualRoot.RaiseEvent(new MinerStudioServiceSwitchedEvent(value ? MinerStudioServiceType.Out : MinerStudioServiceType.Local));
            }
        }

        public static OfficialServices OfficialServer = new OfficialServices();
        public static ClientServices Client = new ClientServices();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public static void PostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            PostAsync(host, port, controller, action, query: null, data: null, callback, timeountMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="data">post的数据</param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public static void PostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            object data,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            PostAsync(host, port, controller, action, query: null, data, callback, timeountMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="signData">用于组装url查询字符串</param>
        /// <param name="data">post的数据</param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public static void SignPostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            object data,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            PostAsync(host, port, controller, action, query: RpcUser.GetSignData(data), data, callback, timeountMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="data">post的数据</param>
        /// <param name="timeountMilliseconds"></param>
        /// <returns></returns>
        public static TResponse Post<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            object data,
            int? timeountMilliseconds = null) {
            return Post<TResponse>(host, port, controller, action, query: null, data, timeountMilliseconds);
        }

        public static TResponse SignPost<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            object data,
            int? timeountMilliseconds = null) {
            return Post<TResponse>(host, port, controller, action, query: RpcUser.GetSignData(data), data, timeountMilliseconds);
        }

        /// <summary>
        /// 异步Post
        /// </summary>
        /// <typeparam name="TResponse">post的data的类型</typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="data">post的数据</param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public static void PostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            object data,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = CreateHttpClient()) {
                        if (timeountMilliseconds != 0) {
                            if (timeountMilliseconds < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}", data);
                        getHttpResponse.Result.Content.ReadAsAsync<TResponse>().ContinueWith(t => {
                            callback?.Invoke(t.Result, null);
                        });
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }

        public static void FirePostAsync(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            object data,
            Action callback = null,
            int timeountMilliseconds = 0) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = CreateHttpClient()) {
                        if (timeountMilliseconds != 0) {
                            if (timeountMilliseconds < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}", data);
                        Write.DevDebug($"{action} {getHttpResponse.Result.ReasonPhrase}");
                        callback?.Invoke();
                    }
                }
                catch {
                    callback?.Invoke();
                }
            });
        }

        public static void FirePostAsync(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            HttpContent content,
            Action callback = null,
            int timeountMilliseconds = 0) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = CreateHttpClient()) {
                        if (timeountMilliseconds != 0) {
                            if (timeountMilliseconds < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}", content);
                        Write.DevDebug($"{action} {getHttpResponse.Result.ReasonPhrase}");
                        callback?.Invoke();
                    }
                }
                catch {
                    callback?.Invoke();
                }
            });
        }

        /// <summary>
        /// 同步Post
        /// </summary>
        /// <typeparam name="TResponse">post的data的类型</typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="data">post的数据</param>
        /// <param name="timeountMilliseconds"></param>
        /// <returns></returns>
        private static TResponse Post<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            object data,
            int? timeountMilliseconds = null) {
            try {
                using (HttpClient client = CreateHttpClient()) {
                    if (timeountMilliseconds.HasValue) {
                        if (timeountMilliseconds.Value < 100) {
                            timeountMilliseconds *= 1000;
                        }
                        client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds.Value);
                    }
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}", data);
                    TResponse response = getHttpResponse.Result.Content.ReadAsAsync<TResponse>().Result;
                    return response;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return default;
            }
        }

        private static bool _isServerMessagesVisible = false;
        /// <summary>
        /// 表示服务器消息在界面上当前是否是可见的。true表示是可见的，反之不是。
        /// </summary>
        /// <remarks>本地会根据服务器消息在界面山是否可见优化网络传输，不可见的时候不从服务器加载消息。</remarks>
        public static bool IsServerMessagesVisible {
            get { return _isServerMessagesVisible; }
        }

        // 独立一个方法是为了方便编程工具走查代码，这算是个模式吧，不只出现这一次。编程的用户有三个：1，人；2，编程工具；3，运行时；
        public static void SetIsServerMessagesVisible(bool value) {
            _isServerMessagesVisible = value;
        }
    }
}
