using NTMiner.MinerStudio;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System.Windows;
using MinerClientUcs = NTMiner.Views.Ucs;
using MinerStudioUcs = NTMiner.MinerStudio.Views.Ucs;
using MinerStudioViews = NTMiner.MinerStudio.Views;

namespace NTMiner {
    public class AppViewFactory : AbstractAppViewFactory {
        public AppViewFactory() { }

        public override Window CreateMainWindow() {
            ConsoleWindow.Instance.Show();
            return new MainWindow {
                Owner = ConsoleWindow.Instance
            };
        }

        public override void BuildPaths() {
            var location = this.GetType();
            VirtualRoot.BuildCmdPath<ShowDialogWindowCommand>(path: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowCalcCommand>(path: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowLocalIpsCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerClientUcs.LocalIpConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowAboutPageCommand>(path: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowKernelOutputPageCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowKernelInputPageCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowTagBrandCommand>(path: message => {
                if (NTMinerContext.IsBrandSpecified) {
                    return;
                }
                UIThread.Execute(() => {
                    BrandTag.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowCoinPageCommand>(path: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowCoinGroupsCommand>(path: message => {
                UIThread.Execute(() => {
                    CoinGroupPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowSysDicPageCommand>(path: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowVirtualMemoryCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerClientUcs.VirtualMemory.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowRestartWindowsCommand>(path: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog(new RestartWindowsViewModel(message.CountDownSeconds));
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowNotificationSampleCommand>(path: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowPropertyCommand>(path: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMessagePathIdsCommand>(path: message => {
                UIThread.Execute(() => {
                    MessagePathIds.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowKernelsWindowCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowKernelDownloaderCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditEnvironmentVariableCommand>(path: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditInputSegmentCommand>(path: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditCoinKernelCommand>(path: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditCoinCommand>(path: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowSpeedChartsCommand>(path: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowFileWriterPageCommand>(path: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditFileWriterCommand>(path: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowFragmentWriterPageCommand>(path: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditFragmentWriterCommand>(path: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditGroupCommand>(path: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditServerMessageCommand>(path: message => {
                UIThread.Execute(() => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditKernelInputCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditKernelOutputKeywordCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditKernelOutputTranslaterCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditKernelOutputCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowPackagesWindowCommand>(path: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditKernelCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditPackageCommand>(path: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditPoolKernelCommand>(path: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditPoolCommand>(path: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditSysDicItemCommand>(path: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditSysDicCommand>(path: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowKernelOutputKeywordsCommand>(path: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywords.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowSignUpPageCommand>(path: message => {
                UIThread.Execute(() => {
                    SignUpPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditWalletCommand>(path: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);

            #region MinerStudio
            VirtualRoot.BuildCmdPath<ShowQQGroupQrCodeCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.QQGroupQrCode.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowCalcConfigCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.CalcConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerClientsWindowCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioViews.MinerClientsWindow.ShowWindow(message.IsToggle);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowNTMinerUpdaterConfigCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerUpdaterConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerClientFinderConfigCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientFinderConfig.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowOverClockDataPageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.OverClockDataPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerStudioVirtualMemoryCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.VirtualMemory.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerStudioLocalIpsCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.LocalIpConfig.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowNTMinerWalletPageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerWalletPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowUserPageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.UserPage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowGpuNamePageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.GpuNameCounts.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowChangePassword>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.ChangePassword.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowWsServerNodePageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.WsServerNodePage.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowRemoteDesktopLoginDialogCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.RemoteDesktopLogin.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerClientSettingCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientSetting.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerNamesSeterCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerNamesSeter.ShowWindow(message.Vm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowGpuProfilesPageCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<ShowMinerClientAddCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientAdd.ShowWindow();
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditMinerGroupCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditNTMinerWalletCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditMineWorkCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditOverClockDataCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            }, location: location);
            VirtualRoot.BuildCmdPath<EditColumnsShowCommand>(path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.ColumnsShowEdit.ShowWindow(message.Source);
                });
            }, location: location);
            #endregion
        }
    }
}
