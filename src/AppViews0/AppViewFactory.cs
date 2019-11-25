using NTMiner.View;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System.Windows;

namespace NTMiner.Views {
    public class AppViewFactory : AbstractAppViewFactory {
        public AppViewFactory() { }

        public override Window CreateMainWindow() {
            return new MainWindow();
        }

        public override void Link() {
            VirtualRoot.BuildCmdPath<ShowDialogWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            });
            VirtualRoot.BuildCmdPath<ShowQQGroupQrCodeCommand>(action: message => {
                UIThread.Execute(() => {
                    QQGroupQrCode.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowCalcCommand>(action: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowLocalIpsCommand>(action: message => {
                UIThread.Execute(() => {
                    LocalIpConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowEthNoDevFeeCommand>(action: message => {
                UIThread.Execute(() => {
                    EthNoDevFeeEdit.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowCalcConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    CalcConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientsWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowNTMinerUpdaterConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerUpdaterConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowAboutPageCommand>(action: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelOutputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelInputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowTagBrandCommand>(action: message => {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandTag.ShowWindow();
            });
            VirtualRoot.BuildCmdPath<ShowCoinPageCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            });
            VirtualRoot.BuildCmdPath<ShowGroupPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowSysDicPageCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => {
                    VirtualMemory.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowRestartWindowsCommand>(action: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog();
                });
            });
            VirtualRoot.BuildCmdPath<ShowNotificationSampleCommand>(action: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowPropertyCommand>(action: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowChartsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    ChartsWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowOverClockDataPageCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowNTMinerWalletPageCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMessagePathIdsCommand>(action: message => {
                UIThread.Execute(() => {
                    MessagePathIds.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowUserPageCommand>(action: message => {
                UIThread.Execute(() => {
                    UserPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowRemoteDesktopLoginDialogCommand>(action: message => {
                RemoteDesktopLogin.ShowWindow(message.Vm);
            });
            VirtualRoot.BuildCmdPath<ShowKernelsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            });
            VirtualRoot.BuildCmdPath<EnvironmentVariableEditCommand>(action: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            });
            VirtualRoot.BuildCmdPath<InputSegmentEditCommand>(action: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            });
            VirtualRoot.BuildCmdPath<CoinKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<CoinEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ColumnsShowEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowSpeedChartsCommand>(action: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowFileWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<FileWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowFragmentWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<FragmentWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<GroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ServerMessageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<KernelInputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<KernelOutputKeywordEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<KernelOutputTranslaterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<KernelOutputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowPackagesWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<KernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientSettingCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientSetting.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerNamesSeterCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerNamesSeter.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowGpuProfilesPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientAddCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientAdd.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<MinerGroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<NTMinerWalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<MineWorkEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<OverClockDataEditCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<PackageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<PoolKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<PoolEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<SysDicItemEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<SysDicEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelOutputKeywordsCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywords.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<UserEditCommand>(action: message => {
                UIThread.Execute(() => {
                    UserEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<WalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
        }
    }
}
