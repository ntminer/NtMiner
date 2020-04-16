using NTMiner.MinerStudio.Impl;
using NTMiner.MinerStudio.Vms;
using NTMiner.Ws;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public static IWsClient WsClient { get; private set; } = EmptyWsClient.Instance;
        public static IMinerStudioService MinerStudioService { get; private set; } = EmptyMinerStudioService.Instance;
        public static MinerClientConsoleViewModel MinerClientConsoleVm { get; private set; } = new MinerClientConsoleViewModel();
        public static MinerClientMessagesViewModel MinerClientMessagesVm { get; private set; } = new MinerClientMessagesViewModel();
        public static MinerClientOperationResultsViewModel MinerClientOperationResultsVm { get; private set; } = new MinerClientOperationResultsViewModel();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsClient">之所以从外部传入是因为IWsClient的实现类在外部的类库中</param>
        public static void Init(IWsClient wsClient) {
            if (RpcRoot.IsOuterNet) {
                MinerStudioService = new ServerMinerStudioService();
                WsClient = wsClient;
            }
            else {
                MinerStudioService = new LocalMinerStudioService();
            }
        }

        public static MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return MinerClientsWindowViewModel.Instance;
            }
        }

        public static CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                return CoinSnapshotDataViewModels.Instance;
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
                MinerClientMessagesVm.SendGetLocalMessagesMqMessage();
            }
        }
    }
}
