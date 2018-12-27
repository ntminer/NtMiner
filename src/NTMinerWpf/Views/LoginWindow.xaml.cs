using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : MetroWindow {
        public LoginWindow() {
            InitializeComponent();
            this.TbHost.Text = $"{Server.MinerServerHost}:{Server.MinerServerPort.ToString()}";
            this.TxtPassword.Focus();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void KbButtonLogin_Click(object sender, RoutedEventArgs e) {
            string passwordSha1 = HashUtil.Sha1(TxtPassword.Password);
            Server.ControlCenterService.Login(TxtLoginName.Text, passwordSha1, response => {
                Execute.OnUIThread(() => {
                    if (response == null) {
                        TbMessage.Text = "服务器忙";
                        TbMessage.Visibility = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            Execute.OnUIThread(() => {
                                TbMessage.Visibility = Visibility.Collapsed;
                            });
                        });
                        return;
                    }
                    if (response.IsSuccess()) {
                        Server.LoginName = TxtLoginName.Text;
                        Server.Password = passwordSha1;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else {
                        TbMessage.Text = response.Description;
                        TbMessage.Visibility = Visibility.Visible;
                        TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                            Execute.OnUIThread(() => {
                                TbMessage.Visibility = Visibility.Collapsed;
                            });
                        });
                        return;
                    }
                });
            });
        }
    }
}
