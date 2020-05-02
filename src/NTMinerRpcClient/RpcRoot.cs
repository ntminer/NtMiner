using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class RpcRoot {
        public static string OfficialServerHost { get; private set; }
        public static int OfficialServerPort;
        public static string OfficialServerAddress = SetOfficialServerAddress("server.ntminer.com:3339");
        public static string SetOfficialServerAddress(string address) {
            if (!address.Contains(":")) {
                address = address + ":" + 3339;
            }
            OfficialServerAddress = address;
            string[] parts = address.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int port)) {
                throw new InvalidProgramException();
            }
            OfficialServerHost = parts[0];
            OfficialServerPort = port;
            return address;
        }

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
                    using (HttpClient client = CreateHttpClient()) {
                        if (timeountMilliseconds.HasValue) {
                            if (timeountMilliseconds.Value < 100) {
                                timeountMilliseconds *= 1000;
                            }
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds.Value);
                        }
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

        /// <summary>
        /// 给定一个类型，返回基于命名约定的控制器名。如果给定的类型名不以Consoller为后缀则引发
        /// InvalidProgramException异常，如果给定的类型是接口类型但不以I开头同样会异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetControllerName<T>() {
            Type t = typeof(T);
            string name = t.Name;
            if (t.IsGenericType) {
                name = name.Substring(0, name.IndexOf('`'));
            }
            if (!name.EndsWith("Controller")) {
                throw new InvalidProgramException("控制器类型名需要以Controller为后缀");
            }
            int startIndex = 0;
            int length = name.Length - "Controller".Length;
            if (t.IsInterface) {
                if (name[0] != 'I') {
                    throw new InvalidProgramException("接口类型名需要以I为开头");
                }
                startIndex = 1;
                length -= 1;
            }
            return name.Substring(startIndex, length);
        }

        /// <summary>
        /// 将字典转化为url查询字符串，返回的字符串以'?'问好开头。
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static string ToQueryString(this Dictionary<string, string> query) {
            string queryString = string.Empty;
            if (query != null && query.Count != 0) {
                queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
            }
            return queryString;
        }

        public static HttpClient CreateHttpClient() {
            return new HttpClient {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }
    }
}
