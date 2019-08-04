using NTMiner.View;
using NTMiner.Views.Ucs;
using System.Windows;

namespace NTMiner.Views {
    public class AppViewFactory : AbstractAppViewFactory {
        public AppViewFactory() { }

        public override Window CreateMainWindow() {
            return new MainWindow();
        }

        public override Window CreateSplashWindow() {
            return new SplashWindow();
        }

        public override void Link() {
            VirtualRoot.Window<ShowDialogWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        DialogWindow.ShowDialog(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon);
                    });
                });
            VirtualRoot.Window<ShowQQGroupQrCodeCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        QQGroupQrCode.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowCalcCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Calc.ShowWindow(message.CoinVm);
                    });
                });
            VirtualRoot.Window<ShowEthNoDevFeeCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        EthNoDevFeeEdit.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowCalcConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CalcConfig.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowMinerClientsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientsWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowNTMinerUpdaterConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        NTMinerUpdaterConfig.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowAboutPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        AboutPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowKernelOutputPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                    });
                });
            VirtualRoot.Window<ShowKernelInputPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelInputPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowCoinPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                    });
                });
            VirtualRoot.Window<ShowGroupPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GroupPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowSysDicPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowVirtualMemoryCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        VirtualMemory.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowRestartWindowsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        RestartWindows.ShowDialog();
                    });
                });
            VirtualRoot.Window<ShowNotificationSampleCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        NotificationSample.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowPropertyCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Property.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowChartsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ChartsWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowOverClockDataPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        OverClockDataPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowUserPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowKernelsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelsWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowKernelDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
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
            VirtualRoot.Window<ShowFileWriterPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FileWriterPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowFragmentWriterPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FragmentWriterPage.ShowWindow();
                    });
                });
            VirtualRoot.Window<FragmentWriterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
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
        }
    }
}
