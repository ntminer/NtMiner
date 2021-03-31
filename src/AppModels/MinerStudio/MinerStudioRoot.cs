using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.MinerStudio.Impl;
using NTMiner.MinerStudio.Vms;
using NTMiner.Ws;
using System;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public static IWsClient WsClient { get; private set; } = EmptyWsClient.Instance;
        public static MinerClientConsoleViewModel MinerClientConsoleVm { get; private set; } = new MinerClientConsoleViewModel();
        public static MinerClientMessagesViewModel MinerClientMessagesVm { get; private set; } = new MinerClientMessagesViewModel();
        public static MinerClientOperationResultsViewModel MinerClientOperationResultsVm { get; private set; } = new MinerClientOperationResultsViewModel();
        public static IReadOnlyNTMinerFileSet ReadOnlyNTMinerFileSet { get; private set; } = new ReadOnlyNTMinerFileSet();

        public static readonly LocalMinerStudioService LocalMinerStudioService = new LocalMinerStudioService();
        private static readonly ServerMinerStudioService _serverMinerStudioService = new ServerMinerStudioService();

        public static IMinerStudioService MinerStudioService {
            get {
                if (RpcRoot.IsOuterNet) {
                    return _serverMinerStudioService;
                }
                else {
                    return LocalMinerStudioService;
                }
            }
        }

        public static void Init(IWsClient wsClient) {
            WsClient = wsClient;
        }

        public static void Login(Action onLoginSuccess, string serverHost = null, Action btnCloseClick = null) {
            WpfUtil.Login(onLoginSuccess: () => {
                NTMinerContext.MinerStudioContext.UserAppSettingSet.Init(RpcRoot.RpcUser.LoginedUser.UserAppSettings);
                onLoginSuccess?.Invoke();
            }, serverHost, btnCloseClick);
        }

        public static MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return MinerClientsWindowViewModel.Instance;
            }
        }

        public static ColumnsShowViewModels ColumnsShowVms {
            get {
                return ColumnsShowViewModels.Instance;
            }
        }

        public static MinerGroupViewModels MinerGroupVms {
            get {
                return MinerGroupViewModels.Instance;
            }
        }

        public static MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Instance;
            }
        }

        public static OverClockDataViewModels OverClockDataVms {
            get {
                return OverClockDataViewModels.Instance;
            }
        }

        public static NTMinerWalletViewModels NTMinerWalletVms {
            get {
                return NTMinerWalletViewModels.Instance;
            }
        }

        private static bool _isMinerClientMessagesVisible = false;
        public static bool IsMinerClientMessagesVisible {
            get { return _isMinerClientMessagesVisible; }
        }

        public static void SetIsMinerClientMessagesVisible(bool value) {
            _isMinerClientMessagesVisible = value;
            if (value) {
                MinerClientMessagesVm.SendGetLocalMessagesMqMessage(isManual: true);
            }
        }
    }
}
