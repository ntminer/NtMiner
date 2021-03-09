using NTMiner.Rpc;
using NTMiner.Rpc.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NTMiner {
    public static partial class RpcRoot {
        public static readonly IJsonRpcHelper JsonRpc = new JsonRpcHelper();

        static RpcRoot() {
        }

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

        public static string GetUrl(string host, int port, string controller, string action, Dictionary<string, string> query) {
            string queryString = string.Empty;
            if (query != null && query.Count != 0) {
                queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
            }
            return $"http://{host}:{port.ToString()}/api/{controller}/{action}{queryString}";
        }

        public static HttpClient CreateHttpClient() {
            return new HttpClient {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public static void SetTimeout(this HttpClient client, int? timeountMilliseconds) {
            if (!timeountMilliseconds.HasValue || timeountMilliseconds.Value < 0) {
                return;
            }
            if (timeountMilliseconds != 0) {
                if (timeountMilliseconds < 100) {
                    timeountMilliseconds *= 1000;
                }
                client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds.Value);
            }
        }
    }
}
