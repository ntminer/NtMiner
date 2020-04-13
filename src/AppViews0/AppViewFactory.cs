using NTMiner.MinerStudio;
using NTMiner.View;
using NTMiner.Views.MinerStudio;
using NTMiner.Views.MinerStudio.Ucs;
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
                UIThread.Execute(() => () => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowQQGroupQrCodeCommand>(action: message => {
                UIThread.Execute(() => () => {
                    QQGroupQrCode.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCalcCommand>(action: message => {
                UIThread.Execute(() => () => {
                    Calc.ShowWindow(message.CoinVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowLocalIpsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    Ucs.LocalIpConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCalcConfigCommand>(action: message => {
                UIThread.Execute(() => () => {
                    CalcConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientsWindowCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerClientsWindow.ShowWindow(message.IsToggle);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNTMinerUpdaterConfigCommand>(action: message => {
                UIThread.Execute(() => () => {
                    NTMinerUpdaterConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientFinderConfigCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerClientFinderConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowAboutPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    AboutPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelOutputPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelInputPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelInputPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowTagBrandCommand>(action: message => {
                if (NTMinerContext.IsBrandSpecified) {
                    return;
                }
                UIThread.Execute(() => () => {
                    BrandTag.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCoinPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowCoinGroupsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    CoinGroupPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowSysDicPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    SysDicPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => () => {
                    Ucs.VirtualMemory.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowRestartWindowsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    RestartWindows.ShowDialog(new RestartWindowsViewModel(message.CountDownSeconds));
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNotificationSampleCommand>(action: message => {
                UIThread.Execute(() => () => {
                    NotificationSample.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowPropertyCommand>(action: message => {
                UIThread.Execute(() => () => {
                    Property.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowChartsWindowCommand>(action: message => {
                UIThread.Execute(() => () => {
                    ChartsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowOverClockDataPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    OverClockDataPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerStudioVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerStudio.Ucs.VirtualMemory.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerStudioLocalIpsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerStudio.Ucs.LocalIpConfig.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowNTMinerWalletPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    NTMinerWalletPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMessagePathIdsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MessagePathIds.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowUserPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    UserPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowWsServerNodePageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    WsServerNodePage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowRemoteDesktopLoginDialogCommand>(action: message => {
                UIThread.Execute(() => () => {
                    RemoteDesktopLogin.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelsWindowCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelDownloaderCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditEnvironmentVariableCommand>(action: message => {
                UIThread.Execute(() => () => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditInputSegmentCommand>(action: message => {
                UIThread.Execute(() => () => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditCoinKernelCommand>(action: message => {
                UIThread.Execute(() => () => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditCoinCommand>(action: message => {
                UIThread.Execute(() => () => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowSpeedChartsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowFileWriterPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    FileWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditFileWriterCommand>(action: message => {
                UIThread.Execute(() => () => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowFragmentWriterPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    FragmentWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditFragmentWriterCommand>(action: message => {
                UIThread.Execute(() => () => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditGroupCommand>(action: message => {
                UIThread.Execute(() => () => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditServerMessageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditKernelInputCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditKernelOutputKeywordCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditKernelOutputTranslaterCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditKernelOutputCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowPackagesWindowCommand>(action: message => {
                UIThread.Execute(() => () => {
                    PackagesWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditKernelCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientSettingCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerClientSetting.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerNamesSeterCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerNamesSeter.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowGpuProfilesPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowMinerClientAddCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerClientAdd.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditMinerGroupCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditNTMinerWalletCommand>(action: message => {
                UIThread.Execute(() => () => {
                    NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditMineWorkCommand>(action: message => {
                UIThread.Execute(() => () => {
                    MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditOverClockDataCommand>(action: message => {
                UIThread.Execute(() => () => {
                    OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditPackageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditPoolKernelCommand>(action: message => {
                UIThread.Execute(() => () => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditPoolCommand>(action: message => {
                UIThread.Execute(() => () => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditSysDicItemCommand>(action: message => {
                UIThread.Execute(() => () => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditSysDicCommand>(action: message => {
                UIThread.Execute(() => () => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowKernelOutputKeywordsCommand>(action: message => {
                UIThread.Execute(() => () => {
                    KernelOutputKeywords.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<ShowSignUpPageCommand>(action: message => {
                UIThread.Execute(() => () => {
                    SignUpPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.AddCmdPath<EditWalletCommand>(action: message => {
                UIThread.Execute(() => () => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
        }
    }
}
