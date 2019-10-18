using NTMiner.View;
using NTMiner.Views.Ucs;
using System.Windows;

namespace NTMiner.Views {
    public class AppViewFactory : AbstractAppViewFactory {
        public AppViewFactory() { }

        public override Window CreateMainWindow() {
            return new MainWindow();
        }

        public override void Link() {
            VirtualRoot.CmdPath<ShowDialogWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        DialogWindow.ShowDialog(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon);
                    });
                });
            VirtualRoot.CmdPath<ShowQQGroupQrCodeCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        QQGroupQrCode.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowCalcCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Calc.ShowWindow(message.CoinVm);
                    });
                });
            VirtualRoot.CmdPath<ShowLocalIpsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        LocalIpConfig.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowEthNoDevFeeCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        EthNoDevFeeEdit.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowCalcConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CalcConfig.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowMinerClientsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientsWindow.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowNTMinerUpdaterConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        NTMinerUpdaterConfig.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowAboutPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        AboutPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowKernelOutputPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                    });
                });
            VirtualRoot.CmdPath<ShowKernelInputPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelInputPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowTagBrandCommand>(LogEnum.DevConsole,
                action: message => {
                    if (NTMinerRoot.IsBrandSpecified) {
                        return;
                    }
                    BrandTag.ShowWindow();
                });
            VirtualRoot.CmdPath<ShowCoinPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                    });
                });
            VirtualRoot.CmdPath<ShowGroupPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GroupPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowSysDicPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowVirtualMemoryCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        VirtualMemory.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowRestartWindowsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        RestartWindows.ShowDialog();
                    });
                });
            VirtualRoot.CmdPath<ShowNotificationSampleCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        NotificationSample.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowPropertyCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Property.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowChartsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ChartsWindow.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowOverClockDataPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        OverClockDataPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowUserPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowKernelsWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelsWindow.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowKernelDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                    });
                });
            VirtualRoot.CmdPath<EnvironmentVariableEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                    });
                });
            VirtualRoot.CmdPath<InputSegmentEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                    });
                });
            VirtualRoot.CmdPath<CoinKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<CoinEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ColumnsShowEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ShowContainerWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ContainerWindow window = ContainerWindow.GetWindow(message.Vm);
                        window?.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowSpeedChartsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SpeedCharts.ShowWindow(message.GpuSpeedVm);
                    });
                });
            VirtualRoot.CmdPath<ShowFileWriterPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FileWriterPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<FileWriterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FileWriterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ShowFragmentWriterPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FragmentWriterPage.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<FragmentWriterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<GroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<KernelInputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelInputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<KernelOutputFilterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputFilterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<KernelOutputTranslaterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<KernelOutputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ShowPackagesWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackagesWindow.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<KernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ShowLogColorCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        LogColor.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<ShowMinerClientSettingCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientSetting.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.CmdPath<ShowMinerNamesSeterCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerNamesSeter.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.CmdPath<ShowGpuProfilesPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                    });
                });
            VirtualRoot.CmdPath<ShowMinerClientAddCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientAdd.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<MinerGroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<MineWorkEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MineWorkEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<OverClockDataEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<PackageEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackageEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<PoolKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<PoolEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<ShowControlCenterHostConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ControlCenterHostConfig.ShowWindow();
                    });
                });
            VirtualRoot.CmdPath<SysDicItemEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<SysDicEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<UserEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.CmdPath<WalletEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        WalletEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
        }
    }
}
