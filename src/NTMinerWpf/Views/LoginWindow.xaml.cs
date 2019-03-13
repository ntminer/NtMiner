using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        public static string ViewId = nameof(LoginWindow);

        public LoginWindowViewModel Vm {
            get {
                return (LoginWindowViewModel)this.DataContext;
            }
        }

        public LoginWindow() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
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
            string passwordSha1 = HashUtil.Sha1(Vm.Password);
            Server.ControlCenterService.LoginAsync(Vm.LoginName, passwordSha1, (response, exception) => {
                if (response == null) {
                    Vm.Message = "服务器忙";
                    Vm.MessageVisible = Visibility.Visible;
                    TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                        UIThread.Execute(() => {
                            Vm.MessageVisible = Visibility.Collapsed;
                        });
                    });
                    return;
                }
                UIThread.Execute(() => {
                    if (response.IsSuccess()) {
                        SingleUser.LoginName = Vm.LoginName;
                        SingleUser.SetPasswordSha1(passwordSha1);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (Vm.LoginName == "admin" && response.StateCode == 404) {
                        Vm.IsPasswordAgainVisible = Visibility.Visible;
                        Vm.Message = response.Description;
                        Vm.MessageVisible = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            UIThread.Execute(() => {
                                Vm.MessageVisible = Visibility.Collapsed;
                            });
                        });
                    }
                    else {
                        Vm.IsPasswordAgainVisible = Visibility.Collapsed;
                        Vm.Message = response.Description;
                        Vm.MessageVisible = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            UIThread.Execute(() => {
                                Vm.MessageVisible = Visibility.Collapsed;
                            });
                        });
                    }
                });
            });
        }
    }
}
