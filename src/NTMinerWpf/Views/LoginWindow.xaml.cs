using MahApps.Metro.Controls;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        private LoginWindowViewModel Vm {
            get {
                return (LoginWindowViewModel)this.DataContext;
            }
        }
        public LoginWindow() {
            EventHandler changeNotiCenterWindowLocation = Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += changeNotiCenterWindowLocation;
            InitializeComponent();
            this.PbPassword.Focus();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            if ((!this.DialogResult.HasValue || !this.DialogResult.Value) && Application.Current.ShutdownMode != ShutdownMode.OnMainWindowClose) {
                Application.Current.Shutdown();
            }
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e) {
            string passwordSha1 = HashUtil.Sha1(Vm.Password);
            Server.ControlCenterService.LoginAsync(Vm.LoginName, passwordSha1, (response, exception) => {
                UIThread.Execute(() => {
                    if (response == null) {
                        Vm.ShowMessage("服务器忙");
                        return;
                    }
                    if (response.IsSuccess()) {
                        SingleUser.LoginName = Vm.LoginName;
                        SingleUser.SetPasswordSha1(passwordSha1);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (Vm.LoginName == "admin" && response.StateCode == 404) {
                        Vm.IsPasswordAgainVisible = Visibility.Visible;
                        Vm.ShowMessage(response.Description);
                        this.PbPasswordAgain.Focus();
                    }
                    else {
                        Vm.IsPasswordAgainVisible = Visibility.Collapsed;
                        Vm.ShowMessage(response.Description);
                    }
                });
            });
        }

        private void OpenServerHostsPopup() {
            var popup = PopupServerHosts;
            popup.IsOpen = true;
            var selected = Vm.ServerHost;
            popup.Child = new ServerHostSelect(
                new ServerHostSelectViewModel(selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.ServerHost != selectedResult) {
                            Vm.ServerHost = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }

        private void ButtonServerHost_Click(object sender, RoutedEventArgs e) {
            OpenServerHostsPopup();
            e.Handled = true;
        }
    }
}
