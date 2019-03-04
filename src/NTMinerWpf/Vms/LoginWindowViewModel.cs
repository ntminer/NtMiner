using NTMiner.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _hostAndPort;
        private string _loginName;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;
        private string _password;

        public ICommand Login { get; private set; }

        public LoginWindowViewModel() {
            this._hostAndPort = $"{Server.MinerServerHost}:{Server.MinerServerPort.ToString()}";
            this._loginName = "admin";
            this.Login = new DelegateCommand(() => {
                string passwordSha1 = HashUtil.Sha1(Password);
                Server.ControlCenterService.LoginAsync(LoginName, passwordSha1, (response, exception) => {
                    UIThread.Execute(() => {
                        if (response == null) {
                            Message = "服务器忙";
                            MessageVisible = Visibility.Visible;
                            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                                UIThread.Execute(() => {
                                    MessageVisible = Visibility.Collapsed;
                                });
                            });
                            return;
                        }
                        if (response.IsSuccess()) {
                            SingleUser.LoginName = LoginName;
                            SingleUser.PasswordSha1 = passwordSha1;
                            Window window = TopWindow.GetTopWindow();
                            if (window != null && window.GetType() == typeof(LoginWindow)) {
                                window.DialogResult = true;
                                window.Close();
                            }
                        }
                        else {
                            Message = response.Description;
                            MessageVisible = Visibility.Visible;
                            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                                UIThread.Execute(() => {
                                    MessageVisible = Visibility.Collapsed;
                                });
                            });
                            return;
                        }
                    });
                });
            });
        }

        public LangViewModels LangVms {
            get { return LangViewModels.Current; }
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

        public string Message {
            get => _message;
            set {
                if (_message != value) {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
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
