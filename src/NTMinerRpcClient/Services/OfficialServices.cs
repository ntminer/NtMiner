using System;
using System.Web;

namespace NTMiner.Services {
    using Official;

    public class OfficialServices {
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";

        public readonly WsServerNodeService WsServerNodeService = new WsServerNodeService();
        public readonly FileUrlService FileUrlService = new FileUrlService();
        public readonly OverClockDataService OverClockDataService = new OverClockDataService();
        public readonly NTMinerWalletService NTMinerWalletService = new NTMinerWalletService();
        public readonly KernelOutputKeywordService KernelOutputKeywordService = new KernelOutputKeywordService();
        public readonly CalcConfigService CalcConfigService = new CalcConfigService();
        public readonly CoinSnapshotService CoinSnapshotService = new CoinSnapshotService();
        public readonly ServerMessageService ServerMessageService = new ServerMessageService();
        public readonly ReportService ReportService = new ReportService();
        public readonly UserService UserService = new UserService();
        public readonly AppSettingService AppSettingService = new AppSettingService();
        public readonly UserAppSettingService UserAppSettingService = new UserAppSettingService();
        public readonly ClientDataService ClientDataService = new ClientDataService();
        public readonly UserMinerGroupService UserMinerGroupService = new UserMinerGroupService();
        public readonly UserMineWorkService UserMineWorkService = new UserMineWorkService();

        internal OfficialServices() {
        }

        public string SignatureSafeUrl(Uri uri) {
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
    }
}
