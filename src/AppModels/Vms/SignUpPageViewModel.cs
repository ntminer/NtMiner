using NTMiner.Controllers;
using NTMiner.User;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SignUpPageViewModel : ViewModelBase, ISignUpInput {
        private string _loginName;
        private Guid _actionCaptchaId = Guid.NewGuid();
        private string _password;
        private string _passwordAgain;
        private string _actionCaptcha;
        private string _loginNameExistMessage;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public ICommand SignUp { get; private set; }
        public ICommand RefreshCaptcha { get; private set; }

        public SignUpPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.RefreshCaptcha = new DelegateCommand(() => {
                ActionCaptchaId = Guid.NewGuid();
            });
            this.SignUp = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.LoginName)) {
                    VirtualRoot.Out.ShowError("登录名不能为空", autoHideSeconds: 4);
                    return;
                }
                else if (string.IsNullOrEmpty(this.Password)) {
                    VirtualRoot.Out.ShowError("密码不能为空", autoHideSeconds: 4);
                    return;
                }
                else if (this.Password != this.PasswordAgain) {
                    VirtualRoot.Out.ShowError("两次输入的密码不一致", autoHideSeconds: 4);
                    return;
                }
                else if (string.IsNullOrEmpty(this.ActionCaptcha)) {
                    VirtualRoot.Out.ShowError("未输入图片验证码", autoHideSeconds: 4);
                    return;
                }
                var data = SignUpRequest.Create(this);
                data.Password = HashUtil.Sha1(data.Password);
                data.PasswordAgain = HashUtil.Sha1(data.PasswordAgain);
                RpcRoot.OfficialServer.UserService.SignUpAsync(data, (response, e) => {
                    if (response.IsSuccess()) {
                        MinerProfileViewModel minerProfile = MinerProfileViewModel.Instance;
                        minerProfile.OuterUserId = this.LoginName;
                        VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                        VirtualRoot.Out.ShowSuccess("注册成功。");
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e));
                    }
                });
            });
        }

        public string LoginName {
            get => _loginName;
            set {
                _loginName = value;
                OnPropertyChanged(nameof(LoginName));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("登录名不能为空");
                }
                if (!VirtualRoot.IsValidLoginName(value, out string message)) {
                    throw new ValidationException(message);
                }
                RpcRoot.OfficialServer.UserService.IsLoginNameExistAsync(value, isExist => {
                    if (isExist) {
                        this.LoginNameExistMessage = "该登录名已被占用，请更换";
                    }
                    else {
                        this.LoginNameExistMessage = string.Empty;
                    }
                });
            }
        }

        public string LoginNameExistMessage {
            get => _loginNameExistMessage;
            set {
                _loginNameExistMessage = value;
                OnPropertyChanged(nameof(LoginNameExistMessage));
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("密码不能为空");
                }
            }
        }

        public string PasswordAgain {
            get => _passwordAgain;
            set {
                _passwordAgain = value;
                OnPropertyChanged(nameof(PasswordAgain));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("请重复以确认密码");
                }
                if (value != Password) {
                    throw new ValidationException("两次输入的密码不一致");
                }
            }
        }

        public Guid ActionCaptchaId {
            get => _actionCaptchaId;
            set {
                _actionCaptchaId = value;
                OnPropertyChanged(nameof(CaptchaUrl));
            }
        }
        /// <summary>
        /// <see cref="ISignUpInput.ActionCaptcha"/>
        /// </summary>
        public string ActionCaptcha {
            get => _actionCaptcha;
            set {
                _actionCaptcha = value;
                OnPropertyChanged(nameof(ActionCaptcha));
            }
        }

        public string CaptchaUrl {
            get {
                return $"http://{RpcRoot.OfficialServerAddress}/api/{RpcRoot.GetControllerName<ICaptchaController<string>>()}/{nameof(ICaptchaController<string>.Get)}?id={ActionCaptchaId.ToString()}";
            }
        }
    }
}
