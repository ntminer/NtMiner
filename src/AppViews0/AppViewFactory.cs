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
            VirtualRoot.AddCmdPath<ShowDialogWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            });
            VirtualRoot.AddCmdPath<ShowQQGroupQrCodeCommand>(action: message => {
                UIThread.Execute(() => {
                    QQGroupQrCode.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowCalcCommand>(action: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            });
            VirtualRoot.AddCmdPath<ShowLocalIpsCommand>(action: message => {
                UIThread.Execute(() => {
                    LocalIpConfig.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowEthNoDevFeeCommand>(action: message => {
                UIThread.Execute(() => {
                    EthNoDevFeeEdit.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowCalcConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    CalcConfig.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowMinerClientsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientsWindow.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowNTMinerUpdaterConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerUpdaterConfig.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowAboutPageCommand>(action: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowKernelOutputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            });
            VirtualRoot.AddCmdPath<ShowKernelInputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowTagBrandCommand>(action: message => {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandTag.ShowWindow();
            });
            VirtualRoot.AddCmdPath<ShowCoinPageCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            });
            VirtualRoot.AddCmdPath<ShowGroupPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowSysDicPageCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => {
                    VirtualMemory.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowRestartWindowsCommand>(action: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog();
                });
            });
            VirtualRoot.AddCmdPath<ShowNotificationSampleCommand>(action: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowPropertyCommand>(action: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowChartsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    ChartsWindow.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowOverClockDataPageCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowNTMinerWalletPageCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowMessagePathIdsCommand>(action: message => {
                UIThread.Execute(() => {
                    MessagePathIds.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowUserPageCommand>(action: message => {
                UIThread.Execute(() => {
                    UserPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowRemoteDesktopLoginDialogCommand>(action: message => {
                RemoteDesktopLogin.ShowWindow(message.Vm);
            });
            VirtualRoot.AddCmdPath<ShowKernelsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<ShowKernelDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            });
            VirtualRoot.AddCmdPath<EnvironmentVariableEditCommand>(action: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            });
            VirtualRoot.AddCmdPath<InputSegmentEditCommand>(action: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            });
            VirtualRoot.AddCmdPath<CoinKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<CoinEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ColumnsShowEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ShowSpeedChartsCommand>(action: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            });
            VirtualRoot.AddCmdPath<ShowFileWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<FileWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ShowFragmentWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<FragmentWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<GroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ServerMessageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<KernelInputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<KernelOutputKeywordEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<KernelOutputTranslaterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<KernelOutputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ShowPackagesWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<KernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ShowMinerClientSettingCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientSetting.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.AddCmdPath<ShowMinerNamesSeterCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerNamesSeter.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.AddCmdPath<ShowGpuProfilesPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            });
            VirtualRoot.AddCmdPath<ShowMinerClientAddCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientAdd.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<MinerGroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<NTMinerWalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<MineWorkEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<OverClockDataEditCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<PackageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<PoolKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<PoolEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<SysDicItemEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<SysDicEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<ShowKernelOutputKeywordsCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywords.ShowWindow();
                });
            });
            VirtualRoot.AddCmdPath<UserEditCommand>(action: message => {
                UIThread.Execute(() => {
                    UserEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.AddCmdPath<WalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
        }
    }
}
