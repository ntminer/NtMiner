using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class JsonRequestBinaryResponseRpcRoot {
        /// <summary>
        /// 注意：Response时ReadAsByteArrayAsync后进行二进制反序列化。
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
                        if (timeountMilliseconds.HasValue) {
                            if (timeountMilliseconds.Value < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds.Value);
                        }
                        Task<HttpResponseMessage> message = client.GetAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}");
                        message.Result.Content.ReadAsByteArrayAsync().ContinueWith(t => {
                            callback?.Invoke(VirtualRoot.BinarySerializer.Deserialize<TResponse>(t.Result), null);
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
