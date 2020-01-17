namespace NTMiner.Service.ServerService {
    public class ServerServices {
        public readonly ControlCenterServiceFace ControlCenterService;
        public readonly UserServiceFace UserService;
        public readonly MinerGroupServiceFace MinerGroupService;
        public readonly MineWorkServiceFace MineWorkService;
        public readonly WalletServiceFace WalletService;
        public readonly PoolServiceFace PoolService;
        public readonly ColumnsShowServiceFace ColumnsShowService;
        public readonly ClientServiceFace ClientService;
        public readonly AppSettingServiceFace AppSettingService;
        public readonly ReportServiceFace ReportService;
        public readonly WrapperMinerClientServiceFace MinerClientService;

        internal ServerServices() {
            ControlCenterService = new ControlCenterServiceFace();
            UserService = new UserServiceFace();
            MinerGroupService = new MinerGroupServiceFace();
            MineWorkService = new MineWorkServiceFace();
            WalletService = new WalletServiceFace();
            PoolService = new PoolServiceFace();
            ColumnsShowService = new ColumnsShowServiceFace();
            ClientService = new ClientServiceFace();
            AppSettingService = new AppSettingServiceFace();
            ReportService = new ReportServiceFace();
            MinerClientService = new WrapperMinerClientServiceFace();
        }
    }
}
