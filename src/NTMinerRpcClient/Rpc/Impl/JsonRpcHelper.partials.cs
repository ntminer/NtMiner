using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Rpc.Impl {
    public partial class JsonRpcHelper : IJsonRpcHelper {
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
        public void PostAsync<TResponse>(
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
        public void PostAsync<TResponse>(
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
        public void SignPostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            object data,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            PostAsync(host, port, controller, action, query: RpcRoot.RpcUser.GetSignData(data), data, callback, timeountMilliseconds);
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
        public void PostAsync<TResponse>(
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
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        if (timeountMilliseconds > 0) {
                            client.SetTimeout(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync(RpcRoot.GetUrl(host, port, controller, action, query), data);
                        if (getHttpResponse.Result.IsSuccessStatusCode) {
                            getHttpResponse.Result.Content.ReadAsAsync<TResponse>().ContinueWith(t => {
                                callback?.Invoke(t.Result, null);
                            });
                        }
                        else {
                            callback?.Invoke(default, new NTMinerHttpException($"{action} http response {getHttpResponse.Result.StatusCode.ToString()} {getHttpResponse.Result.ReasonPhrase}") {
                                StatusCode = getHttpResponse.Result.StatusCode,
                                ReasonPhrase = getHttpResponse.Result.ReasonPhrase
                            });
                        }
                    }
                }
                catch (Exception e) {
                    NTMinerConsole.DevError(e.Message + e.StackTrace);
                    callback?.Invoke(default, e);
                }
            });
        }

        public void FirePostAsync(
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
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        if (timeountMilliseconds > 0) {
                            client.SetTimeout(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync(RpcRoot.GetUrl(host, port, controller, action, query), data);
                        NTMinerConsole.DevDebug($"{action} {getHttpResponse.Result.ReasonPhrase}");
                        if (!getHttpResponse.Result.IsSuccessStatusCode) {
                            NTMinerConsole.DevDebug($"{action} http response {getHttpResponse.Result.StatusCode.ToString()} {getHttpResponse.Result.ReasonPhrase}");
                        }
                        callback?.Invoke();
                    }
                }
                catch {
                    callback?.Invoke();
                }
            });
        }
    }
}
