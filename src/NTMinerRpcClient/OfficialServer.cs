using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Web;

namespace NTMiner {
    public partial class OfficialServer {
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
        public const string NTMinerBucket = "https://ntminer.oss-cn-beijing.aliyuncs.com/";

        public readonly FileUrlServiceFace FileUrlService;
        public readonly OverClockDataServiceFace OverClockDataService;
        public readonly NTMinerWalletServiceFace NTMinerWalletService;
        public readonly KernelOutputKeywordServiceFace KernelOutputKeywordService;
        public readonly ControlCenterServiceFace ControlCenterService;
        public readonly ServerMessageServiceFace ServerMessageService;

        private readonly string _host;
        private readonly int _port;
        internal OfficialServer(string host, int port) {
            _host = host;
            _port = port;
            FileUrlService = new FileUrlServiceFace(host, port);
            OverClockDataService = new OverClockDataServiceFace(host, port);
            NTMinerWalletService = new NTMinerWalletServiceFace(host, port);
            KernelOutputKeywordService = new KernelOutputKeywordServiceFace(host, port);
            ControlCenterService = new ControlCenterServiceFace(host, port);
            ServerMessageService = new ServerMessageServiceFace(host, port);
        }

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

        public void GetTimeAsync(Action<DateTime> callback) {
            RpcRoot.GetAsync(_host, _port, "AppSetting", nameof(IAppSettingController.GetTime), null, callback: (DateTime datetime, Exception e) => {
                callback?.Invoke(datetime);
            });
        }

        #region GetJsonFileVersionAsync
        public void GetJsonFileVersionAsync(string key, Action<ServerState> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            RpcRoot.PostAsync(_host, _port, "AppSetting", nameof(IAppSettingController.GetJsonFileVersion), request, (string text, Exception e) => {
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