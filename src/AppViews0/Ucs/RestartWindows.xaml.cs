using NTMiner.Hub;
using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class RestartWindows : UserControl {
        public static void ShowDialog() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "重启电脑",
                Width = 400,
                Height = 200,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_Restart"
            }, ucFactory: (window) => {
                RestartWindows uc = new RestartWindows();
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: uc.Id, location: typeof(RestartWindows));
                return uc;
            }, fixedSize: true);
        }

        public RestartWindowsViewModel Vm {
            get {
                return (RestartWindowsViewModel)this.DataContext;
            }
        }

        public readonly Guid Id = Guid.NewGuid();

        private bool _isCanceled = false;
        public RestartWindows() {
            InitializeComponent();
            this.RunOneceOnLoaded(window => {
                IMessagePathId messagePathId = null;
                messagePathId = window.AddViaLimitPath<Per1SecondEvent>("重启倒计时", LogEnum.None, action: message => {
                    if (_isCanceled) {
                        return;
                    }
                    Vm.Seconds = Vm.Seconds - 1;
                    if (messagePathId.ViaLimit == 0) {
                        UIThread.Execute(() => {
                            Windows.Power.Restart();
                        });
                    }
                }, Vm.Seconds, location: this.GetType());
            });
        }

        private void KbCancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            _isCanceled = true;
            VirtualRoot.Execute(new CloseWindowCommand(this.Id));
        }
    }

    public class RestartWindowsViewModel : ViewModelBase {
        private int _seconds = 4;

        public int Seconds {
            get => _seconds;
            set {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
            }
        }
    }
}
