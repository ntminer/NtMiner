using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Rpc.Impl {
    public class BinaryRequestJsonResponseRpcHelper : IBinaryRequestJsonResponseRpcHelper {
        public BinaryRequestJsonResponseRpcHelper() { }

        /// <summary>
        /// 注意：Request时ByteArrayContent，Response时ReadAsAsync<TResponse>。
        /// </summary>
        /// <typeparam name="TResponse">post的data的类型</typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="data">字节数组</param>
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
                        byte[] bytes = VirtualRoot.BinarySerializer.Serialize(data);
                        if (bytes == null) {
                            bytes = new byte[0];
                        }
                        HttpContent content = new ByteArrayContent(bytes);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync(RpcRoot.GetUrl(host, port, controller, action, query), content);
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
                    callback?.Invoke(default, e);
                }
            });
        }
    }
}
