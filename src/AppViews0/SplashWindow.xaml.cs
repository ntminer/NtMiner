using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner.Views {
    public partial class SplashWindow : Window {
        public SplashWindow() {
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
