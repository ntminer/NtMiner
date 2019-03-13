using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _hostAndPort;
        private string _loginName;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;
        private string _password;
        private Visibility _isPasswordAgainVisible = Visibility.Collapsed;
        private string _passwordAgain;
        private SolidColorBrush _messageForeground;

        public ICommand ActiveAdmin { get; private set; }

        public LoginWindowViewModel() {
            this._hostAndPort = $"{Server.MinerServerHost}:{WebApiConst.MinerServerPort}";
            this._loginName = "admin";
            this.ActiveAdmin = new DelegateCommand(() => {
                string message = string.Empty;
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
                        this.ShowMessage("激活成功");
                    }
                    else {
                        this.ShowMessage(response != null ? response.Description : "激活失败");
                    }
                });
            });
        }

        private void ShowMessage(string message, bool isSuccess = false) {
            this.Message = message;
            MessageVisible = Visibility.Visible;
            if (isSuccess) {
                this.MessageForeground = new SolidColorBrush(Colors.Green);
            }
            else {
                this.MessageForeground = new SolidColorBrush(Colors.Red);
            }
            TimeSpan.FromSeconds(4).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MessageVisible = Visibility.Collapsed;
                });
            });
        }

        public LangViewModels LangVms {
            get { return LangViewModels.Current; }
        }

        public Visibility IsPasswordAgainVisible {
            get => _isPasswordAgainVisible;
            set {
                _isPasswordAgainVisible = value;
                OnPropertyChanged(nameof(IsPasswordAgainVisible));
            }
        }

        public string HostAndPort {
            get => _hostAndPort;
            set {
                if (_hostAndPort != value) {
                    _hostAndPort = value;
                    OnPropertyChanged(nameof(HostAndPort));
                }
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

        public string Message {
            get => _message;
            set {
                if (_message != value) {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public SolidColorBrush MessageForeground {
            get => _messageForeground;
            set {
                _messageForeground = value;
                OnPropertyChanged(nameof(MessageForeground));
            }
        }

        public Visibility MessageVisible {
            get => _messageVisible;
            set {
                if (_messageVisible != value) {
                    _messageVisible = value;
                    OnPropertyChanged(nameof(MessageVisible));
                }
            }
        }
    }
}
