using NTMiner.Views;
using NTMiner.Views.Ucs;
using System.Windows;

namespace NTMiner {
    public class AnotherWindowFactory : DefaultWindowFactory {
        protected override Window CreateMainWindow() {
            return new MainWindow();
        }

        public override Window CreateSplashWindow() {
            return new SplashWindow();
        }

        public override void Link() {
            VirtualRoot.Window<ShowKernelDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelsWindow.ShowWindow(message.KernelId, message.DownloadComplete);
                    });
                });
            VirtualRoot.Window<EnvironmentVariableEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                    });
                });
            VirtualRoot.Window<InputSegmentEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                    });
                });
            VirtualRoot.Window<CoinKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<CoinEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ColumnsShowEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowContainerWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ContainerWindow window = ContainerWindow.GetWindow(message.Vm);
                        window?.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowSpeedChartsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SpeedCharts.ShowWindow(message.GpuSpeedVm);
                    });
                });
            VirtualRoot.Window<GroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelInputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelInputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputFilterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputFilterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputTranslaterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowPackagesWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackagesWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<KernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowLogColorCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        LogColor.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowMinerClientSettingCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientSetting.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowMinerNamesSeterCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerNamesSeter.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowGpuProfilesPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                    });
                });
            VirtualRoot.Window<ShowMinerClientAddCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientAdd.ShowWindow();
                    });
                });
            VirtualRoot.Window<MinerGroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<MineWorkEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MineWorkEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<OverClockDataEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PackageEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackageEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowControlCenterHostConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ControlCenterHostConfig.ShowWindow();
                    });
                });
            VirtualRoot.Window<SysDicItemEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<SysDicEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<UserEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<WalletEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        WalletEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<UpgradeCommand>(LogEnum.DevConsole,
                action: message => {
                    Upgrade(message.FileName, message.Callback);
                });
        }
    }
}
