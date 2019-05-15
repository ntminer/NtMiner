using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class SplashWindow : Window {
        public SplashWindowViewModel Vm {
            get {
                return (SplashWindowViewModel)this.DataContext;
            }
        }

        public SplashWindow() {
#if DEBUG
            VirtualRoot.Stopwatch.Restart();
#endif
            InitializeComponent();
#if DEBUG
            Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
