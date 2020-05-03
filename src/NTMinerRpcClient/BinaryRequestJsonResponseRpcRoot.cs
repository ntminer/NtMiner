using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static class BinaryRequestJsonResponseRpcRoot {
        /// <summary>
        /// 注意：Request时ByteArrayContent，Response时ReadAsAsync<TResponse>。
        /// </summary>
        /// <typeparam name="TResponse">post的data的类型</typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public static void PostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            byte[] bytes,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        if (timeountMilliseconds != 0) {
                            if (timeountMilliseconds < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        if (bytes == null) {
                            bytes = new byte[0];
                        }
                        HttpContent content = new ByteArrayContent(bytes);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{host}:{port.ToString()}/api/{controller}/{action}{query.ToQueryString()}", content);
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
    }
}
