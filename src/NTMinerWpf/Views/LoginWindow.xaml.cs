using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        public LoginWindowViewModel Vm {
            get {
                return (LoginWindowViewModel)this.DataContext;
            }
        }

        public LoginWindow() {
            InitializeComponent();

            this.TxtPassword.Focus();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void KbButtonLogin_Click(object sender, RoutedEventArgs e) {
            string passwordSha1 = HashUtil.Sha1(TxtPassword.Password);
            Server.ControlCenterService.Login(Vm.LoginName, passwordSha1, response => {
                Execute.OnUIThread(() => {
                    if (response == null) {
                        Vm.Message = "服务器忙";
                        Vm.MessageVisible = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            Execute.OnUIThread(() => {
                                Vm.MessageVisible = Visibility.Collapsed;
                            });
                        });
                        return;
                    }
                    if (response.IsSuccess()) {
                        Server.LoginName = Vm.LoginName;
                        Server.Password = passwordSha1;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else {
                        Vm.Message = response.Description;
                        Vm.MessageVisible = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            Execute.OnUIThread(() => {
                                Vm.MessageVisible = Visibility.Collapsed;
                            });
                        });
                        return;
                    }
                });
            });
        }
    }
}
