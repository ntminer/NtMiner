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
            VirtualRoot.CreateCmdPath<ShowDialogWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    DialogWindow.ShowDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            });
            VirtualRoot.CreateCmdPath<ShowQQGroupQrCodeCommand>(action: message => {
                UIThread.Execute(() => {
                    QQGroupQrCode.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowCalcCommand>(action: message => {
                UIThread.Execute(() => {
                    Calc.ShowWindow(message.CoinVm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowLocalIpsCommand>(action: message => {
                UIThread.Execute(() => {
                    LocalIpConfig.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowEthNoDevFeeCommand>(action: message => {
                UIThread.Execute(() => {
                    EthNoDevFeeEdit.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowCalcConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    CalcConfig.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowMinerClientsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientsWindow.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowNTMinerUpdaterConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerUpdaterConfig.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowAboutPageCommand>(action: message => {
                UIThread.Execute(() => {
                    AboutPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowKernelOutputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowKernelInputPageCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowTagBrandCommand>(action: message => {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandTag.ShowWindow();
            });
            VirtualRoot.CreateCmdPath<ShowCoinPageCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            });
            VirtualRoot.CreateCmdPath<ShowGroupPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowSysDicPageCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowVirtualMemoryCommand>(action: message => {
                UIThread.Execute(() => {
                    VirtualMemory.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowRestartWindowsCommand>(action: message => {
                UIThread.Execute(() => {
                    RestartWindows.ShowDialog();
                });
            });
            VirtualRoot.CreateCmdPath<ShowNotificationSampleCommand>(action: message => {
                UIThread.Execute(() => {
                    NotificationSample.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowPropertyCommand>(action: message => {
                UIThread.Execute(() => {
                    Property.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowChartsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    ChartsWindow.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowOverClockDataPageCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowNTMinerWalletPageCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowUserPageCommand>(action: message => {
                UIThread.Execute(() => {
                    UserPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowKernelsWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelsWindow.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowKernelDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            });
            VirtualRoot.CreateCmdPath<EnvironmentVariableEditCommand>(action: message => {
                UIThread.Execute(() => {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            });
            VirtualRoot.CreateCmdPath<InputSegmentEditCommand>(action: message => {
                UIThread.Execute(() => {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            });
            VirtualRoot.CreateCmdPath<CoinKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<CoinEditCommand>(action: message => {
                UIThread.Execute(() => {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ColumnsShowEditCommand>(action: message => {
                UIThread.Execute(() => {
                    ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ShowContainerWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    ContainerWindow window = ContainerWindow.GetWindow(message.Vm);
                    window?.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowSpeedChartsCommand>(action: message => {
                UIThread.Execute(() => {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowFileWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<FileWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ShowFragmentWriterPageCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterPage.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<FragmentWriterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<GroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<KernelInputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<KernelOutputFilterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputFilterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<KernelOutputTranslaterEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<KernelOutputEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ShowPackagesWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    PackagesWindow.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<KernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ShowLogColorCommand>(action: message => {
                UIThread.Execute(() => {
                    LogColor.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<ShowMinerClientSettingCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientSetting.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowMinerNamesSeterCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerNamesSeter.ShowWindow(message.Vm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowGpuProfilesPageCommand>(action: message => {
                UIThread.Execute(() => {
                    GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                });
            });
            VirtualRoot.CreateCmdPath<ShowMinerClientAddCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerClientAdd.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<MinerGroupEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<NTMinerWalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    NTMinerWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<MineWorkEditCommand>(action: message => {
                UIThread.Execute(() => {
                    MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<OverClockDataEditCommand>(action: message => {
                UIThread.Execute(() => {
                    OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<PackageEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<PoolKernelEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<PoolEditCommand>(action: message => {
                UIThread.Execute(() => {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<ShowControlCenterHostConfigCommand>(action: message => {
                UIThread.Execute(() => {
                    ControlCenterHostConfig.ShowWindow();
                });
            });
            VirtualRoot.CreateCmdPath<SysDicItemEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<SysDicEditCommand>(action: message => {
                UIThread.Execute(() => {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<UserEditCommand>(action: message => {
                UIThread.Execute(() => {
                    UserEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            VirtualRoot.CreateCmdPath<WalletEditCommand>(action: message => {
                UIThread.Execute(() => {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });
        }
    }
}
