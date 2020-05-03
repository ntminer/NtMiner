using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static class HttpRpcRoot {
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
    }
}
