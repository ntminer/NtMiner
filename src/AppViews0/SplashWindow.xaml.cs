using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner.Views {
    // 供其它界面使用的AppStatic静态类型中有很多依赖对象，比如SolidColorBrush、BitmapImage等，所以必须确保该SplashWindow没有访问AppStatic
    public partial class SplashWindow : Window {
        public static void ShowWindowAsync(Action<SplashWindow> callback) {
            Thread t = new Thread(() => {
                SplashWindow win = new SplashWindow();
                win.Closed += (d, k) => {
                    System.Windows.Threading.Dispatcher.ExitAllFrames();
                };
                win.Show();
                callback?.Invoke(win);
                System.Windows.Threading.Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private SplashWindow() {
            InitializeComponent();
            // BitmapImage是依赖对象，而SplashWindow是在单独的线程中使用的，所以SplashWindow不能使用任何其它界面用到的依赖对象
            this.BigLogo.Source = new BitmapImage(new Uri((VirtualRoot.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));
            this.TbFullVersion.Text = $"v{MainAssemblyInfo.CurrentVersion}({MainAssemblyInfo.CurrentVersionTag})";
    }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
