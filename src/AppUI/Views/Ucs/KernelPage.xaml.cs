using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelPage : UserControl {
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete = null) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Logo",
                CloseVisible = Visibility.Visible,
                HeaderVisible = Visibility.Collapsed,
                FooterVisible = Visibility.Collapsed,
                Width = DevMode.IsDebugMode ? 1200 : AppStatic.MainWindowWidth,
                Height = AppStatic.GetMainWindowHeight(isDevMode: false)
            },
            ucFactory: (window) => {
                var uc = new KernelPage();
                uc.CloseWindow = () => window.Close();
                return uc;
            },
            beforeShow: uc => {
                if (kernelId != Guid.Empty) {
                    KernelPageViewModel vm = (KernelPageViewModel)uc.DataContext;
                    vm.Download(kernelId, (isSuccess, message) => {
                        if (isSuccess) {
                            ((KernelPage)uc).CloseWindow();
                        }
                        downloadComplete(isSuccess, message);
                    });
                }
            }, fixedSize: true);
        }

        public Action CloseWindow { get; set; }

        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void BtnDownloadMenu_Click(object sender, RoutedEventArgs e) {
            if (Vm.KernelDownloadingVisible == Visibility.Visible) {
                Vm.KernelDownloadingVisible = Visibility.Collapsed;
            }
            else {
                Vm.KernelDownloadingVisible = Visibility.Visible;
            }
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset - CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset + CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void CoinsScrollView_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ListBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            Vm.Search.Execute(null);
        }
    }
}
