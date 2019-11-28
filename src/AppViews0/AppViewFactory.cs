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
            var location = this.GetType();
            VirtualRoot.AddCmdPath<ShowDialogWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowQQGroupQrCodeCommand>(action: message => {
                UIThread.Execute(() => {
                    QQGroupQrCode.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCalcCommand>(action: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowLocalIpsCommand>(action: message => {
                UIThread.Execute(() => {
                    LocalIpConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowEthNoDevFeeCommand>(action: message => {
                UIThread.Execute(() => {
                    EthNoDevFeeEdit.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCalcConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    CalcConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNTMinerUpdaterConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerUpdaterConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowAboutPageCommand>(action: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelOutputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelInputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowTagBrandCommand>(action: message => {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandTag.ShowWindow();
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCoinPageCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowGroupPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowSysDicPageCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => {
                    VirtualMemory.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowRestartWindowsCommand>(action: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNotificationSampleCommand>(action: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowPropertyCommand>(action: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowChartsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    ChartsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowOverClockDataPageCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNTMinerWalletPageCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMessagePathIdsCommand>(action: message => {
                UIThread.Execute(() => {
                    MessagePathIds.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowUserPageCommand>(action: message => {
                UIThread.Execute(() => {
                    UserPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowRemoteDesktopLoginDialogCommand>(action: message => {
                RemoteDesktopLogin.ShowWindow(message.Vm);
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EnvironmentVariableEditCommand>(action: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<InputSegmentEditCommand>(action: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<CoinKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<CoinEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ColumnsShowEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowSpeedChartsCommand>(action: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowFileWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<FileWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowFragmentWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<FragmentWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<GroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ServerMessageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<KernelInputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<KernelOutputKeywordEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<KernelOutputTranslaterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<KernelOutputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowPackagesWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<KernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientSettingCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientSetting.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerNamesSeterCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerNamesSeter.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowGpuProfilesPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientAddCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientAdd.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<MinerGroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<NTMinerWalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<MineWorkEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<OverClockDataEditCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<PackageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<PoolKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<PoolEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<SysDicItemEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<SysDicEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelOutputKeywordsCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywords.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<UserEditCommand>(action: message => {
                UIThread.Execute(() => {
                    UserEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<WalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
        }
    }
}
