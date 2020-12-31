using System;
using System.Collections.Generic;

namespace NTMiner.Rpc {
    public interface IBinaryRequestJsonResponseRpcHelper {
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
        void PostAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            object data,
            Action<TResponse, Exception> callback,
            int timeountMilliseconds = 0);
    }
}
