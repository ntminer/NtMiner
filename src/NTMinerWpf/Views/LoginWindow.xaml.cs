using MahApps.Metro.Controls;
using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        public static string ViewId = nameof(LoginWindow);

        private readonly LoginWindowViewModel _vm;
        public LoginWindow() {
            _vm = new LoginWindowViewModel();
            this.DataContext = _vm;
            EventHandler ChangeNotiCenterWindowLocation = Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += ChangeNotiCenterWindowLocation;
            this.LocationChanged += ChangeNotiCenterWindowLocation;
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

        private void CbLanguage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            LangViewModel selectedItem = (LangViewModel)e.AddedItems[0];
            if (selectedItem != VirtualRoot.Lang) {
                VirtualRoot.Lang = selectedItem;
                ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            }
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e) {
            string passwordSha1 = HashUtil.Sha1(_vm.Password);
            Server.ControlCenterService.LoginAsync(_vm.LoginName, passwordSha1, (response, exception) => {
                UIThread.Execute(() => {
                    if (response == null) {
                        _vm.ShowMessage("服务器忙");
                        return;
                    }
                    if (response.IsSuccess()) {
                        SingleUser.LoginName = _vm.LoginName;
                        SingleUser.SetPasswordSha1(passwordSha1);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (_vm.LoginName == "admin" && response.StateCode == 404) {
                        _vm.IsPasswordAgainVisible = Visibility.Visible;
                        _vm.ShowMessage(response.Description);
                        this.PbPasswordAgain.Focus();
                    }
                    else {
                        _vm.IsPasswordAgainVisible = Visibility.Collapsed;
                        _vm.ShowMessage(response.Description);
                    }
                });
            });
        }
    }
}
