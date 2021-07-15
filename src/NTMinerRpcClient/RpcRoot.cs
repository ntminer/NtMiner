using NTMiner.Rpc;
using NTMiner.Rpc.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace NTMiner {
    public static partial class RpcRoot {
        public static readonly IJsonRpcHelper JsonRpc = new JsonRpcHelper();

        static RpcRoot() {
            SetOfficialServerAddress(NTKeyword.OfficialServerAddress);
        }

        public static string OfficialServerHost { get; private set; }
        public static int OfficialServerPort { get; private set; }
        public static string OfficialServerAddress { get; private set; }
        public static void SetOfficialServerAddress(string address) {
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

        public static byte[] ZipDecompress(byte[] zippedData) {
            using (Stream ms = new MemoryStream(zippedData),
                          compressedzipStream = new GZipStream(ms, CompressionMode.Decompress),
                          outBuffer = new MemoryStream()) {
                byte[] block = new byte[NTKeyword.IntK];
                while (true) {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0) {
                        break;
                    }
                    else {
                        outBuffer.Write(block, 0, bytesRead);
                    }
                }
                compressedzipStream.Close();
                return ((MemoryStream)outBuffer).ToArray();
            }
        }
    }
}
