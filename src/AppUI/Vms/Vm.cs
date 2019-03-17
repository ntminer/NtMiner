namespace NTMiner.Vms {
    public class Vm : ViewModelBase {
        public static readonly Vm Instance = new Vm();
        private RootViewModel _root;

        private Vm() {
            _root = new RootViewModel();
            NTMinerRoot.Current.OnReRendMinerProfile += () => {
                OnPropertyChanged(nameof(Root));
            };
        }

        public RootViewModel Root {
            get => _root;
            set {
                _root = value;
                OnPropertyChanged(nameof(Root));
            }
        }

        public class RootViewModel : ViewModelBase {
            public RootViewModel() { }

            public MinerProfileViewModel MinerProfile {
                get {
                    return MinerProfileViewModel.Current;
                }
            }

            public StateBarViewModel StateBarVm {
                get {
                    return StateBarViewModel.Current;
                }
            }

            public AppSettingViewModels AppSettingVms {
                get {
                    return AppSettingViewModels.Current;
                }
            }

            public CoinGroupViewModels CoinGroupVms {
                get {
                    return CoinGroupViewModels.Current;
                }
            }

            public CoinKernelViewModels CoinKernelVms {
                get {
                    return CoinKernelViewModels.Current;
                }
            }

            public CoinProfileViewModels CoinProfileVms {
                get {
                    return CoinProfileViewModels.Current;
                }
            }

            public CoinSnapshotDataViewModels CoinSnapshotDataVms {
                get {
                    return CoinSnapshotDataViewModels.Current;
                }
            }

            public CoinViewModels CoinVms {
                get {
                    return CoinViewModels.Current;
                }
            }

            public ColumnsShowViewModels ColumnsShowVms {
                get {
                    return ColumnsShowViewModels.Current;
                }
            }

            public GpuProfileViewModels GpuProfileVms {
                get {
                    return GpuProfileViewModels.Current;
                }
            }

            public GpuSpeedViewModels GpuSpeedVms {
                get {
                    return GpuSpeedViewModels.Current;
                }
            }

            public GpuViewModels GpuVms {
                get {
                    return GpuViewModels.Current;
                }
            }

            public GroupViewModels GroupVms {
                get {
                    return GroupViewModels.Current;
                }
            }

            public HandlerIdViewModels HandlerIdVms {
                get {
                    return HandlerIdViewModels.Current;
                }
            }

            public KernelInputViewModels KernelInputVms {
                get {
                    return KernelInputViewModels.Current;
                }
            }

            public KernelOutputFilterViewModels KernelOutputFilterVms {
                get {
                    return KernelOutputFilterViewModels.Current;
                }
            }

            public KernelOutputTranslaterViewModels KernelOutputTranslaterVms {
                get {
                    return KernelOutputTranslaterViewModels.Current;
                }
            }

            public KernelOutputViewModels KernelOutputVms {
                get {
                    return KernelOutputViewModels.Current;
                }
            }

            public KernelViewModels KernelVms {
                get {
                    return KernelViewModels.Current;
                }
            }

            public MinerGroupViewModels MinerGroupVms {
                get {
                    return MinerGroupViewModels.Current;
                }
            }

            public MineWorkViewModels MineWorkVms {
                get {
                    return MineWorkViewModels.Current;
                }
            }

            public OverClockDataViewModels OverClockDataVms {
                get {
                    return OverClockDataViewModels.Current;
                }
            }

            public PoolKernelViewModels PoolKernelVms {
                get {
                    return PoolKernelViewModels.Current;
                }
            }

            public PoolProfileViewModels PoolProfileVms {
                get {
                    return PoolProfileViewModels.Current;
                }
            }

            public PoolViewModels PoolVms {
                get {
                    return PoolViewModels.Current;
                }
            }

            public ShareViewModels ShareVms {
                get {
                    return ShareViewModels.Current;
                }
            }

            public SysDicItemViewModels SysDicItemVms {
                get {
                    return SysDicItemViewModels.Current;
                }
            }

            public SysDicViewModels SysDicVms {
                get {
                    return SysDicViewModels.Current;
                }
            }

            public UserViewModels UserVms {
                get {
                    return UserViewModels.Current;
                }
            }

            public WalletViewModels WalletVms {
                get {
                    return WalletViewModels.Current;
                }
            }
        }
    }
}
