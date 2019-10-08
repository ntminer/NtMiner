using NTMiner.View;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class SplashWindow : Window, ISplashWindow {
        private bool _isNormalClose = false;

        public SplashWindowViewModel Vm {
            get {
                return (SplashWindowViewModel)this.DataContext;
            }
        }

        public SplashWindow() {
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            InitializeComponent();
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            if (!_isNormalClose) {
                Application.Current.Shutdown();
            }
        }

        public void NormalClose() {
            _isNormalClose = true;
            this.Close();
        }
    }
}
