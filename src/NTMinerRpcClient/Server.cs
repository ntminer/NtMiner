using System;

namespace NTMiner {
    public partial class Server {
        public readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public readonly UserServiceFace UserService = UserServiceFace.Instance;
        public readonly MinerGroupServiceFace MinerGroupService = MinerGroupServiceFace.Instance;
        public readonly MineWorkServiceFace MineWorkService = MineWorkServiceFace.Instance;
        public readonly WalletServiceFace WalletService = WalletServiceFace.Instance;
        public readonly PoolServiceFace PoolService = PoolServiceFace.Instance;
        public readonly ColumnsShowServiceFace ColumnsShowService = ColumnsShowServiceFace.Instance;
        public readonly ClientServiceFace ClientService = ClientServiceFace.Instance;
        public readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public readonly WrapperMinerClientServiceFace MinerClientService = WrapperMinerClientServiceFace.Instance;

        internal Server() { }

        private static void PostAsync<T>(string controller, string action, object data, Action<T, Exception> callback) where T : class {
            RpcRoot.PostAsync<T>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, controller, action, null, data, callback);
        }

        private static void PostAsync<T>(string controller, string action, IGetSignData signData, object data, Action<T, Exception> callback) where T : class {
            RpcRoot.PostAsync<T>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, controller, action, signData.ToQuery(), data, callback);
        }

        private static T Post<T>(string controller, string action, object data, int? timeout = null) where T : class {
            return RpcRoot.Post<T>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, controller, action, null, data, timeout);
        }

        private static T Post<T>(string controller, string action, IGetSignData signData, object data, int? timeout = null) where T : class {
            return RpcRoot.Post<T>(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, controller, action, signData.ToQuery(), data, timeout);
        }
    }
}
