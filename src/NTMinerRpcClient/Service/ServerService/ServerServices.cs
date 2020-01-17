namespace NTMiner.Service.ServerService {
    public class ServerServices {
        public readonly ControlCenterService ControlCenterService;
        public readonly UserService UserService;
        public readonly MinerGroupService MinerGroupService;
        public readonly MineWorkService MineWorkService;
        public readonly WalletService WalletService;
        public readonly PoolService PoolService;
        public readonly ColumnsShowServiceFace ColumnsShowService;
        public readonly ClientService ClientService;
        public readonly AppSettingServiceFace AppSettingService;
        public readonly ReportService ReportService;
        public readonly WrapperMinerClientService MinerClientService;

        internal ServerServices() {
            ControlCenterService = new ControlCenterService();
            UserService = new UserService();
            MinerGroupService = new MinerGroupService();
            MineWorkService = new MineWorkService();
            WalletService = new WalletService();
            PoolService = new PoolService();
            ColumnsShowService = new ColumnsShowServiceFace();
            ClientService = new ClientService();
            AppSettingService = new AppSettingServiceFace();
            ReportService = new ReportService();
            MinerClientService = new WrapperMinerClientService();
        }
    }
}
