using System;
using System.Net.Http;

namespace NTMiner {
    public static class HttpClientFactory {
        public static HttpClient Create() {
            return new HttpClient {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }
    }
}
