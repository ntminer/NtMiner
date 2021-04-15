using NTMiner.Vms;
using System.Diagnostics;
using System.Windows;

namespace NTMiner.Views {
    public partial class DialogWindow : BlankWindow {
        public static void ShowSoftDialog(DialogWindowViewModel vm) {
            Window window = new DialogWindow(vm);
            window.MousePosition();
            window.ShowSoftDialog();
        }

        public static void ShowHardDialog(DialogWindowViewModel vm) {
            Window window = new DialogWindow(vm);
            window.MousePosition();
            window.ShowHardDialog();
        }

        public DialogWindowViewModel Vm { get; private set; }

        private DialogWindow(DialogWindowViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            this.Title = vm.Title;
            InitializeComponent();
            this.TbUcName.Text = nameof(DialogWindow);
            if (!string.IsNullOrEmpty(vm.Icon)) {
                object obj = AppUtil.GetResource(vm.Icon);
                if (obj != null) {
                    this.Resources["Icon"] = obj;
                }
            }

            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
        }

        private void KbYesButton_Click(object sender, RoutedEventArgs e) {
            Vm.OnYes?.Invoke();
            this.Close();
        }

        private void KbNoButton_Click(object sender, RoutedEventArgs e) {
            if (Vm.OnNo != null) {
                if (Vm.IsConfirmNo && !Vm.NoText.StartsWith("请再点一次")) {
                    string noText = Vm.NoText;
                    int n = 4;
                    Vm.NoText = $"请再点一次({n.ToString()})";
                    this.BuildViaTimesLimitPath<Per1SecondEvent>("倒计时'请再点一次'", LogEnum.None, viaTimesLimit: n, this.GetType(), PathPriority.Normal, message => {
                        n--;
                        Vm.NoText = $"请再点一次({n.ToString()})";
                        if (n == 0) {
                            Vm.NoText = noText;
                        }
                    });
                }
                else if (Vm.OnNo.Invoke()) {
                    this.Close();
                }
            }
            else {
                this.Close();
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(Vm.HelpUrl)) {
                Process.Start(Vm.HelpUrl);
            }
        }
    }
}
