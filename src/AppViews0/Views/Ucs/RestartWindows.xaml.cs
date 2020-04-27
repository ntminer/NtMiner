using NTMiner.Hub;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class RestartWindows : UserControl {
        public static void ShowDialog(RestartWindowsViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "重启电脑",
                Width = 400,
                Height = 200,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_Restart"
            }, ucFactory: (window) => {
                RestartWindows uc = new RestartWindows(vm);
                window.AddCloseWindowOnecePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public RestartWindowsViewModel Vm { get; private set; }

        private bool _isCanceled = false;
        public RestartWindows(RestartWindowsViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
            this.OnLoaded(window => {
                IMessagePathId messagePathId = null;
                messagePathId = window.AddViaTimesLimitPath<Per1SecondEvent>("重启倒计时", LogEnum.None, action: message => {
                    if (_isCanceled) {
                        return;
                    }
                    Vm.Seconds = Vm.Seconds - 1;
                    if (messagePathId.ViaTimesLimit == 0) {
                        Windows.Power.Restart();
                    }
                }, Vm.Seconds, location: this.GetType());
            });
        }

        private void KbCancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            _isCanceled = true;
            VirtualRoot.Execute(new CloseWindowCommand(this.Vm.Id));
            VirtualRoot.ThisLocalInfo(nameof(RestartWindows), "取消重启电脑");
        }
    }
}
