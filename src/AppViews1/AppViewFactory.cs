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
            VirtualRoot.Window<ShowFileDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
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
            VirtualRoot.Window<ShowInnerPropertyCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        InnerProperty.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowUserPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserPage.ShowWindow();
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
            VirtualRoot.Window<ShowContainerWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ContainerWindow window = ContainerWindow.GetWindow(message.Vm);
                        window?.ShowWindow();
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
            VirtualRoot.Window<ShowGpuProfilesPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
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
