using NTMiner.Bus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public static readonly AppContext Instance = new AppContext();

        private static readonly List<IDelegateHandler> _contextHandlers = new List<IDelegateHandler>();

        private AppContext() {
            Link();
        }

        private static void Link() {
            VirtualRoot.Window<EnvironmentVariableEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                    });
                });
            VirtualRoot.Window<InputSegmentEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                    });
                });
            VirtualRoot.Window<CoinKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<CoinEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.CoinEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ColumnsShowEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowContainerWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.ContainerWindow window = Views.ContainerWindow.GetWindow(message.Vm);
                        window?.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowSpeedChartsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.SpeedCharts.ShowWindow(message.GpuSpeedVm);
                    });
                });
            VirtualRoot.Window<GroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.GroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelInputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.KernelInputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputFilterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.KernelOutputFilterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputTranslaterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowPackagesWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.PackagesWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<KernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.KernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowLogColorCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.LogColor.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowMinerClientSettingCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.MinerClientSetting.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowMinerNamesSeterCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.MinerNamesSeter.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowGpuProfilesPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                    });
                });
            VirtualRoot.Window<ShowMinerClientAddCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.MinerClientAdd.ShowWindow();
                    });
                });
            VirtualRoot.Window<MinerGroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<MineWorkEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.MineWorkEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<OverClockDataEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PackageEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.PackageEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.PoolEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowControlCenterHostConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.ControlCenterHostConfig.ShowWindow();
                    });
                });
            VirtualRoot.Window<SysDicItemEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<SysDicEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.SysDicEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<UserEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.UserEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<WalletEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.WalletEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
        }

        #region static methods
        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        private static DelegateHandler<TCmd> Window<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        private static DelegateHandler<TEvent> On<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        public static void Enable() {
            foreach (var handler in _contextHandlers) {
                handler.IsEnabled = true;
            }
        }

        public static void Disable() {
            foreach (var handler in _contextHandlers) {
                handler.IsEnabled = false;
            }
        }
        #endregion

        #region context
        public MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return MinerClientsWindowViewModel.Instance;
            }
        }

        public MinerProfileViewModel MinerProfileVm {
            get {
                return MinerProfileViewModel.Instance;
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Instance;
            }
        }

        public GpuSpeedViewModels GpuSpeedVms {
            get {
                return GpuSpeedViewModels.Instance;
            }
        }

        public StartStopMineButtonViewModel StartStopMineButtonVm {
            get {
                return StartStopMineButtonViewModel.Instance;
            }
        }

        public PoolKernelViewModels PoolKernelVms {
            get {
                return PoolKernelViewModels.Instance;
            }
        }

        public PackageViewModels PackageVms {
            get {
                return PackageViewModels.Instance;
            }
        }

        public CoinGroupViewModels CoinGroupVms {
            get {
                return CoinGroupViewModels.Instance;
            }
        }

        public CoinKernelViewModels CoinKernelVms {
            get {
                return CoinKernelViewModels.Instance;
            }
        }

        public CoinProfileViewModels CoinProfileVms {
            get {
                return CoinProfileViewModels.Instance;
            }
        }

        public CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                return CoinSnapshotDataViewModels.Instance;
            }
        }

        public ColumnsShowViewModels ColumnsShowVms {
            get {
                return ColumnsShowViewModels.Instance;
            }
        }

        public DriveSetViewModel DriveSetVm {
            get {
                return DriveSetViewModel.Instance;
            }
        }

        public VirtualMemorySetViewModel VirtualMemorySetVm {
            get {
                return VirtualMemorySetViewModel.Instance;
            }
        }

        public GpuProfileViewModels GpuProfileVms {
            get {
                return GpuProfileViewModels.Instance;
            }
        }

        public GpuViewModels GpuVms {
            get {
                return GpuViewModels.Instance;
            }
        }

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Instance;
            }
        }

        public KernelInputViewModels KernelInputVms {
            get {
                return KernelInputViewModels.Instance;
            }
        }

        public KernelOutputFilterViewModels KernelOutputFilterVms {
            get {
                return KernelOutputFilterViewModels.Instance;
            }
        }

        public KernelOutputTranslaterViewModels KernelOutputTranslaterVms {
            get {
                return KernelOutputTranslaterViewModels.Instance;
            }
        }

        public KernelOutputViewModels KernelOutputVms {
            get {
                return KernelOutputViewModels.Instance;
            }
        }

        public KernelViewModels KernelVms {
            get {
                return KernelViewModels.Instance;
            }
        }

        public MinerGroupViewModels MinerGroupVms {
            get {
                return MinerGroupViewModels.Instance;
            }
        }

        public MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Instance;
            }
        }

        public OverClockDataViewModels OverClockDataVms {
            get {
                return OverClockDataViewModels.Instance;
            }
        }

        public PoolProfileViewModels PoolProfileVms {
            get {
                return PoolProfileViewModels.Instance;
            }
        }

        public PoolViewModels PoolVms {
            get {
                return PoolViewModels.Instance;
            }
        }

        public ShareViewModels ShareVms {
            get {
                return ShareViewModels.Instance;
            }
        }

        public WalletViewModels WalletVms {
            get {
                return WalletViewModels.Instance;
            }
        }

        public UserViewModels UserVms {
            get {
                return UserViewModels.Instance;
            }
        }

        public SysDicViewModels SysDicVms {
            get {
                return SysDicViewModels.Instance;
            }
        }

        public SysDicItemViewModels SysDicItemVms {
            get {
                return SysDicItemViewModels.Instance;
            }
        }

        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return GpuStatusBarViewModel.Instance;
            }
        }
        #endregion
    }
}
