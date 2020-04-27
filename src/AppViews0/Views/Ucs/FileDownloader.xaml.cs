using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    // 这个是单文件下载器，内核下载器是另一个下载器叫KernelDownloading
    public partial class FileDownloader : UserControl {
        public static void ShowWindow(
            string downloadFileUrl, string fileTitle,
            // window, isSuccess, message, saveFileFullName, etagValue
            Action<ContainerWindow, bool, string, string> downloadComplete) {
                UIThread.Execute(() => {
                    ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                        Title = "下载器 - " + fileTitle,
                        IconName = "Icon_Download",
                        Height = 200,
                        Width = 400,
                        IsMaskTheParent = false,
                        IsChildWindow = true,
                        CloseVisible = System.Windows.Visibility.Visible,
                    }, ucFactory: (window) => {
                        FileDownloaderViewModel vm = new FileDownloaderViewModel(downloadFileUrl, (isSuccess, message, saveFileFullName) => {
                            UIThread.Execute(() => {
                                downloadComplete(window, isSuccess, message, saveFileFullName);
                            });
                        });
                        return new FileDownloader(vm);
                    }, fixedSize: true);
                });
        }

        public FileDownloaderViewModel Vm { get; private set; }

        public FileDownloader(FileDownloaderViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
