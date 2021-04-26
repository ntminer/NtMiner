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
            VirtualRoot.BuildCmdPath<ShowDialogWindowCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            });
            VirtualRoot.BuildCmdPath<ShowCalcCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowLocalIpsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    LocalIpConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowAboutPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelOutputPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelInputPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowTagBrandCommand>(location: location, LogEnum.DevConsole, path: message => {
                if (NTMinerContext.IsBrandSpecified) {
                    return;
                }
                UIThread.Execute(() => {
                    BrandTag.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowCoinPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            });
            VirtualRoot.BuildCmdPath<ShowCoinGroupsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    CoinGroupPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowSysDicPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowVirtualMemoryCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerClientUcs.VirtualMemory.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowRestartWindowsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog(new RestartWindowsViewModel(message.CountDownSeconds));
                });
            });
            VirtualRoot.BuildCmdPath<ShowNotificationSampleCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowPropertyCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMessagePathIdsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MessagePathIds.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelsWindowCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelDownloaderCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            });
            VirtualRoot.BuildCmdPath<EditEnvironmentVariableCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            });
            VirtualRoot.BuildCmdPath<EditInputSegmentCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            });
            VirtualRoot.BuildCmdPath<EditCoinKernelCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditCoinCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowSpeedChartsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowFileWriterPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<EditFileWriterCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowFragmentWriterPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<EditFragmentWriterCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditGroupCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditServerMessageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditKernelInputCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditKernelOutputKeywordCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditKernelOutputTranslaterCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditKernelOutputCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowPackagesWindowCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<EditKernelCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditPackageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditPoolKernelCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditPoolCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditSysDicItemCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditSysDicCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<ShowKernelOutputKeywordsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    KernelOutputKeywords.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<EditWalletCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            #region MinerStudio
            VirtualRoot.BuildCmdPath<ShowQQGroupQrCodeCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.QQGroupQrCode.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowCalcConfigCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.CalcConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientsWindowCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioViews.MinerClientsWindow.ShowWindow(message.IsToggle);
                });
            });
            VirtualRoot.BuildCmdPath<ShowNTMinerUpdaterConfigCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerUpdaterConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientFinderConfigCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientFinderConfig.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowOverClockDataPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.OverClockDataPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerStudioVirtualMemoryCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.VirtualMemory.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerStudioLocalIpsCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.LocalIpConfig.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowNTMinerWalletPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerWalletPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowUserPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.UserPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowGpuNamePageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.GpuNameCounts.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowActionCountPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.ActionCounts.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowMqCountsPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MqCountsPage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowChangePassword>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.ChangePassword.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowWsServerNodePageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.WsServerNodePage.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<ShowRemoteDesktopLoginDialogCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.RemoteDesktopLogin.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientSettingCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientSetting.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerNamesSeterCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerNamesSeter.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowGpuProfilesPageCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            });
            VirtualRoot.BuildCmdPath<ShowMinerClientAddCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerClientAdd.ShowWindow();
                });
            });
            VirtualRoot.BuildCmdPath<EditMinerGroupCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditNTMinerWalletCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditMineWorkCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditOverClockDataCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.BuildCmdPath<EditColumnsShowCommand>(location: location, LogEnum.DevConsole, path: message => {
                UIThread.Execute(() => {
                    MinerStudioUcs.ColumnsShowEdit.ShowWindow(message.Source);
                });
            });
            #endregion
        }
    }
}
