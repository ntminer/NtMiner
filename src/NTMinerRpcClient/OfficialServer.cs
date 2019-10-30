using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NTMiner {
    public static partial class OfficialServer {
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
        public const string NTMinerBucket = "https://ntminer.oss-cn-beijing.aliyuncs.com/";
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly OverClockDataServiceFace OverClockDataService = OverClockDataServiceFace.Instance;
        public static readonly NTMinerWalletServiceFace NTMinerWalletService = NTMinerWalletServiceFace.Instance;
        public static readonly CalcConfigServiceFace CalcConfigService = CalcConfigServiceFace.Instance;

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
        private static void PostAsync<T>(string controller, string action, Dictionary<string, string> query, object param, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        if (timeountMilliseconds != 0) {
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
                        string queryString = string.Empty;
                        if (query != null && query.Count != 0) {
                            queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                        }
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{MainAssemblyInfo.OfficialServerHost}:{NTKeyword.ControlCenterPort}/api/{controller}/{action}{queryString}", param);
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(null, e);
                }
            });
        }

        private static void GetAsync<T>(string controller, string action, Dictionary<string, string> param, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        string queryString = string.Empty;
                        if (param != null && param.Count != 0) {
                            queryString = "?" + string.Join("&", param.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> message = client.GetAsync($"http://{MainAssemblyInfo.OfficialServerHost}:{NTKeyword.ControlCenterPort}/api/{controller}/{action}{queryString}");
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default(T), e);
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
        public static void GetJsonFileVersionAsync(string key, Action<string, string> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            PostAsync("AppSetting", nameof(IAppSettingController.GetJsonFileVersion), null, request, (string text, Exception e) => {
                string jsonFileVersion = string.Empty;
                string minerClientVersion = string.Empty;
                if (!string.IsNullOrEmpty(text)) {
                    string[] parts = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0) {
                        jsonFileVersion = parts[0];
                    }
                    if (parts.Length > 1) {
                        minerClientVersion = parts[1];
                    }
                }
                callback?.Invoke(jsonFileVersion, minerClientVersion);
            }, timeountMilliseconds: 10 * 1000);
        }
        #endregion
    }
}