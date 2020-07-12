using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static class HttpRpcRoot {
        /// <summary>
        /// 注意：Request时原始HttpContent，Fire忽略Response
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="query"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
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
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        client.SetTimeout(timeountMilliseconds);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync(RpcRoot.GetUrl(host, port, controller, action, query), content);
                        NTMinerConsole.DevDebug($"{action} {getHttpResponse.Result.ReasonPhrase}");
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
