using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class AutoStartCountdown : UserControl {
        public static void ShowDialog() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "自动挖矿倒计时",
                Width = 400,
                Height = 200,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_Logo"
            }, ucFactory: (window) => {
                AutoStartCountdown uc = new AutoStartCountdown();
                uc.CloseWindow = () => {
                    window.Close();
                };
                return uc;
            }, fixedSize: true);
        }

        public AutoStartCountdownViewModel Vm {
            get {
                return (AutoStartCountdownViewModel)this.DataContext;
            }
        }

        public Action CloseWindow;
        public AutoStartCountdown() {
            InitializeComponent();
            System.Timers.Timer t = new System.Timers.Timer(1000);
            t.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                if (NTMinerRoot.IsAutoStartCanceled) {
                    t.Enabled = false;
                    t.Stop();
                    return;
                }
                Execute.OnUIThread(() => {
                    Vm.Seconds = Vm.Seconds - 1;
                    if (Vm.Seconds <= 0) {
                        CloseWindow();
                    }
                });
            };
            t.Start();
        }

        private void KbCancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            NTMinerRoot.IsAutoStartCanceled = true;
            CloseWindow?.Invoke();
        }
    }

    public class AutoStartCountdownViewModel : ViewModelBase {
        private int _seconds = 9;

        public int Seconds {
            get => _seconds;
            set {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
            }
        }
    }
}
