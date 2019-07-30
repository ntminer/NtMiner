using System;
using System.Net;

namespace NTMiner {
    public class NTMinerWebClient : WebClient {
        /// <summary>
        /// 单位秒，默认60秒
        /// </summary>
        public int TimeoutSeconds { get; set; }

        public NTMinerWebClient() {
            this.TimeoutSeconds = 180;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutSeconds">秒</param>
        public NTMinerWebClient(int timeoutSeconds) {
            this.TimeoutSeconds = timeoutSeconds;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var result = base.GetWebRequest(address);
            result.Timeout = this.TimeoutSeconds * 1000;
            return result;
        }
    }
}
