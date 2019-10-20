using NTMiner.Vms;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class DialogWindow : BlankWindow {
        public static void ShowDialog(DialogWindowViewModel vm) {
            Window window = new DialogWindow(vm);
            window.MousePosition();
            window.ShowDialogEx();
        }

        public DialogWindowViewModel Vm {
            get {
                return (DialogWindowViewModel)this.DataContext;
            }
        }

        private DialogWindow(DialogWindowViewModel vm) {
            this.DataContext = vm;
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
                if (Vm.OnNo.Invoke()) {
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(Vm.HelpUrl)) {
                Process.Start(Vm.HelpUrl);
            }
        }
    }
}
