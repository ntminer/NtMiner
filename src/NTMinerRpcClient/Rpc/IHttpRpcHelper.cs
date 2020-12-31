using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NTMiner.Rpc {
    public interface IHttpRpcHelper {
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
        void FirePostAsync(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            HttpContent content,
            Action callback = null,
            int timeountMilliseconds = 0);
    }
}
