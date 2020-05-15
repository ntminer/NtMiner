using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class JsonRpcRoot {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="callback"></param>
        public static void GetAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            Action<TResponse, Exception> callback,
            int? timeountMilliseconds = null) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        client.SetTimeout(timeountMilliseconds);
                        Task<HttpResponseMessage> message = client.GetAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}");
                        message.Result.Content.ReadAsAsync<TResponse>().ContinueWith(t => {
                            callback?.Invoke(t.Result, null);
                        });
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }
    }
}
