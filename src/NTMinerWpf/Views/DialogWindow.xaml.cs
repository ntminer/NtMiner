using NTMiner.Vms;
using System;
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

        public DialogWindowViewModel Vm {
            get {
                return (DialogWindowViewModel)this.DataContext;
            }
        }

        private DialogWindow(DialogWindowViewModel vm) {
            this.DataContext = vm;
            this.Title = vm.Title;
            InitializeComponent();
            if (!string.IsNullOrEmpty(vm.Icon) && Application.Current.Resources.Contains(vm.Icon)) {
                this.Resources["Icon"] = Application.Current.Resources[vm.Icon];
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
                    this.AddViaTimesLimitPath<Per1SecondEvent>("倒计时'请再点一次'", LogEnum.None, message => {
                        UIThread.Execute(() => {
                            n--;
                            Vm.NoText = $"请再点一次({n.ToString()})";
                            if (n == 0) {
                                Vm.NoText = noText;
                            }
                        });
                    }, viaTimesLimit: n, this.GetType());
                }
                else if (Vm.OnNo.Invoke()) {
                    this.Close();
                }
            }
            else {
                this.Close();
            }
        }

        private void KbOkButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(Vm.HelpUrl)) {
                Process.Start(Vm.HelpUrl);
            }
        }
    }
}
