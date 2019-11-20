using NTMiner.Notifications;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _loginName;
        private string _password;
        private Visibility _isPasswordAgainVisible = Visibility.Collapsed;
        private string _passwordAgain;
        private String _serverHost;

        public ICommand ActiveAdmin { get; private set; }

        public LoginWindowViewModel() {
            this._loginName = "admin";
            this._serverHost = NTMinerRegistry.GetControlCenterHost();
            this.IsInnerIp = Net.Util.IsInnerIp(_serverHost);
            this.ActiveAdmin = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Password)) {
                    this.ShowMessage("密码不能为空");
                    return;
                }
                else if (this.Password != this.PasswordAgain) {
                    this.ShowMessage("两次输入的密码不一致");
                    return;
                }
                string passwordSha1 = HashUtil.Sha1(Password);
                Server.ControlCenterService.ActiveControlCenterAdminAsync(passwordSha1, (response, e) => {
                    if (response.IsSuccess()) {
                        IsPasswordAgainVisible = Visibility.Collapsed;
                        this.ShowMessage("激活成功", isSuccess: true);
                    }
                    else {
                        this.ShowMessage(response.ReadMessage(e));
                    }
                });
            });
        }

        public void ShowMessage(string message, bool isSuccess = false) {
            UIThread.Execute(() => {
                if (isSuccess) {
                    VirtualRoot.Out.ShowSuccess(message);
                }
                else {
                    NotiCenterWindowViewModel.Instance.Manager.CreateMessage()
                        .Error(message)
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(2))
                        .Queue();
                }
            });
        }

        public string ServerHost {
            get => _serverHost;
            set {
                _serverHost = value;
                OnPropertyChanged(nameof(ServerHost));
                this.IsInnerIp = Net.Util.IsInnerIp(value);
                OnPropertyChanged(nameof(IsInnerIp));
            }
        }

        public bool IsInnerIp {
            get;set;
        }

        public int Port {
            get { return NTKeyword.ControlCenterPort; }
        }

        public Visibility IsPasswordAgainVisible {
            get => _isPasswordAgainVisible;
            set {
                _isPasswordAgainVisible = value;
                OnPropertyChanged(nameof(IsPasswordAgainVisible));
            }
        }

        public string LoginName {
            get => _loginName;
            set {
                if (_loginName != value) {
                    _loginName = value;
                    OnPropertyChanged(nameof(LoginName));
                }
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string PasswordAgain {
            get => _passwordAgain;
            set {
                _passwordAgain = value;
                OnPropertyChanged(nameof(PasswordAgain));
            }
        }
    }
}
