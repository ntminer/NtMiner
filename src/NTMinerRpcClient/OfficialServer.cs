using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NTMiner {
    public partial class OfficialServer {
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
        public const string NTMinerBucket = "https://ntminer.oss-cn-beijing.aliyuncs.com/";
        public readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public readonly OverClockDataServiceFace OverClockDataService = OverClockDataServiceFace.Instance;
        public readonly NTMinerWalletServiceFace NTMinerWalletService = NTMinerWalletServiceFace.Instance;
        public readonly KernelOutputKeywordServiceFace KernelOutputKeywordService = KernelOutputKeywordServiceFace.Instance;
        public readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public readonly ServerMessageServiceFace ServerMessageService = ServerMessageServiceFace.Instance;

        internal OfficialServer() { }

        public static string SignatureSafeUrl(Uri uri) {
            // https://ntminer.oss-cn-beijing.aliyuncs.com/packages/HSPMinerAE2.1.2.zip?Expires=1554472712&OSSAccessKeyId=LTAIHNApO2ImeMxI&Signature=FVTf+nX4grLKcPRxpJd9nf3Py7I=
            // Signature的值长度是28
            string url = uri.ToString();
            const string keyword = "Signature=";
            int index = url.IndexOf(keyword);
            if (index != -1) {
                string signature = url.Substring(index + keyword.Length, 28);
                return url.Substring(0, index) + keyword + HttpUtility.UrlEncode(signature) + url.Substring(index + keyword.Length + 28);
            }
            return url;
        }

        #region private methods
        private static void PostAsync<T>(string controller, string action, Dictionary<string, string> query, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        if (timeountMilliseconds != 0) {
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        string queryString = string.Empty;
                        if (query != null && query.Count != 0) {
                            queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{NTKeyword.OfficialServerHost}:{NTKeyword.ControlCenterPort.ToString()}/api/{controller}/{action}{queryString}", data);
                        T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(null, e);
                }
            });
        }

        private static void PostAsync<T>(string controller, string action, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            PostAsync<T>(controller, action, (Dictionary<string, string>)null, null, callback, timeountMilliseconds);
        }

        private static void PostAsync<T>(string controller, string action, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            PostAsync<T>(controller, action, (Dictionary<string, string>)null, data, callback, timeountMilliseconds);
        }

        private static void PostAsync<T>(string controller, string action, IGetSignData signData, object data, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            PostAsync<T>(controller, action, signData.ToQuery(), data, callback, timeountMilliseconds);
        }

        private static void GetAsync<T>(string controller, string action, Dictionary<string, string> data, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        string queryString = string.Empty;
                        if (data != null && data.Count != 0) {
                            queryString = "?" + string.Join("&", data.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> getHttpResponse = client.GetAsync($"http://{NTKeyword.OfficialServerHost}:{NTKeyword.ControlCenterPort.ToString()}/api/{controller}/{action}{queryString}");
                        T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }
        #endregion

        public static void GetTimeAsync(Action<DateTime> callback) {
            GetAsync("AppSetting", nameof(IAppSettingController.GetTime), null, callback: (DateTime datetime, Exception e) => {
                callback?.Invoke(datetime);
            });
        }

        #region GetJsonFileVersionAsync
        public static void GetJsonFileVersionAsync(string key, Action<ServerState> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            PostAsync("AppSetting", nameof(IAppSettingController.GetJsonFileVersion), request, (string text, Exception e) => {
                string jsonFileVersion = string.Empty;
                string minerClientVersion = string.Empty;
                ulong time = Timestamp.GetTimestamp();
                ulong messageTimestamp = 0;
                ulong kernelOutputKeywordTimestamp = 0;
                if (!string.IsNullOrEmpty(text)) {
                    text = text.Trim();
                    string[] parts = text.Split(new char[] { '|' });
                    if (parts.Length > 0) {
                        jsonFileVersion = parts[0];
                    }
                    if (parts.Length > 1) {
                        minerClientVersion = parts[1];
                    }
                    if (parts.Length > 2) {
                        ulong.TryParse(parts[2], out time);
                    }
                    if (parts.Length > 3) {
                        ulong.TryParse(parts[3], out messageTimestamp);
                    }
                    if (parts.Length > 4) {
                        ulong.TryParse(parts[4], out kernelOutputKeywordTimestamp);
                    }
                }
                callback?.Invoke(new ServerState {
                    JsonFileVersion = jsonFileVersion,
                    MinerClientVersion = minerClientVersion,
                    Time = time,
                    MessageTimestamp = messageTimestamp,
                    OutputKeywordTimestamp = kernelOutputKeywordTimestamp
                });
            }, timeountMilliseconds: 10 * 1000);
        }
        #endregion
    }
}