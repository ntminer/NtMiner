namespace NTMiner {
    public partial class Server {
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

        internal Server(string host, int port) {
            ControlCenterService = new ControlCenterServiceFace(host, port);
            UserService = new UserServiceFace(host, port);
            MinerGroupService = new MinerGroupServiceFace(host, port);
            MineWorkService = new MineWorkServiceFace(host, port);
            WalletService = new WalletServiceFace(host, port);
            PoolService = new PoolServiceFace(host, port);
            ColumnsShowService = new ColumnsShowServiceFace(host, port);
            ClientService = new ClientServiceFace(host, port);
            AppSettingService = new AppSettingServiceFace(host, port);
            ReportService = new ReportServiceFace(host, port);
            MinerClientService = new WrapperMinerClientServiceFace(host, port);
        }
    }
}
