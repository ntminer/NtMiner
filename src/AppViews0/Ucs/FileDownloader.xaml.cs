using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FileDownloader : UserControl {
        public static void ShowWindow(
            string downloadFileUrl, string fileTitle,
            // window, isSuccess, message, saveFileFullName, etagValue
            Action<ContainerWindow, bool, string, string> downloadComplete) {
                UIThread.Execute(() => () => {
                    ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                        Title = "下载 - " + fileTitle,
                        IconName = "Icon_Download",
                        Height = 200,
                        Width = 400,
                        CloseVisible = System.Windows.Visibility.Visible,
                    }, ucFactory: (window) => {
                        FileDownloaderViewModel vm = new FileDownloaderViewModel(downloadFileUrl, (isSuccess, message, saveFileFullName) => {
                            UIThread.Execute(() => () => {
                                downloadComplete(window, isSuccess, message, saveFileFullName);
                            });
                        });
                        return new FileDownloader(vm);
                    }, fixedSize: true);
                });
        }

        public FileDownloaderViewModel Vm {
            get {
                return (FileDownloaderViewModel)this.DataContext;
            }
        }

        public FileDownloader(FileDownloaderViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
