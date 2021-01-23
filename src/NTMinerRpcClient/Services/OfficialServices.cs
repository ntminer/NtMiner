namespace NTMiner.Services {
    using Official;

    public class OfficialServices {
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";

        public readonly WsServerNodeService WsServerNodeService = new WsServerNodeService();
        public readonly WebApiServerNodeService WebApiServerNodeService = new WebApiServerNodeService();
        public readonly FileUrlService FileUrlService = new FileUrlService();
        public readonly OverClockDataService OverClockDataService = new OverClockDataService();
        public readonly NTMinerWalletService NTMinerWalletService = new NTMinerWalletService();
        public readonly KernelOutputKeywordService KernelOutputKeywordService = new KernelOutputKeywordService();
        public readonly CalcConfigService CalcConfigService = new CalcConfigService();
        public readonly ServerMessageService ServerMessageService = new ServerMessageService();
        public readonly ReportService ReportService = new ReportService();
        public readonly ReportBinaryService ReportBinaryService = new ReportBinaryService();
        public readonly UserService UserService = new UserService();
        public readonly AppSettingService AppSettingService = new AppSettingService();
        public readonly UserAppSettingService UserAppSettingService = new UserAppSettingService();
        public readonly ClientDataService ClientDataService = new ClientDataService();
        public readonly UserMinerGroupService UserMinerGroupService = new UserMinerGroupService();
        public readonly UserMineWorkService UserMineWorkService = new UserMineWorkService();
        public readonly GpuNameService GpuNameService = new GpuNameService();

        internal OfficialServices() {
        }
    }
}
