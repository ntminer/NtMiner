using NTMiner.Controllers;
using NTMiner.User;
using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class ChangePasswordViewModel : ViewModelBase {
        private Guid _actionCaptchaId = Guid.NewGuid();
        private string _actionCaptcha;
        private string _newPassword;
        private string _newPasswordAgain;
        private string _oldPassword;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public ICommand RefreshCaptcha { get; private set; }
        public ICommand Ok { get; private set; }

        public ChangePasswordViewModel() {
            this.RefreshCaptcha = new DelegateCommand(() => {
                ActionCaptchaId = Guid.NewGuid();
            });
            this.Ok = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(OldPassword)) {
                    VirtualRoot.Out.ShowWarn("未填写旧密码");
                    return;
                }
                if (RpcRoot.RpcUser.Password != HashUtil.Sha1(OldPassword)) {
                    VirtualRoot.Out.ShowWarn("旧密码错误");
                    return;
                }
                if (string.IsNullOrEmpty(NewPassword)) {
                    VirtualRoot.Out.ShowWarn("未填写新密码");
                    return;
                }
                if (string.IsNullOrEmpty(NewPasswordAgain)) {
                    VirtualRoot.Out.ShowWarn("未填写重复密码");
                    return;
                }
                if (NewPassword != NewPasswordAgain) {
                    VirtualRoot.Out.ShowWarn("两次输入的密码不一致");
                    return;
                }
                if (string.IsNullOrEmpty(ActionCaptcha)) {
                    VirtualRoot.Out.ShowWarn("未填写验证码");
                    return;
                }
                RpcRoot.OfficialServer.UserService.ChangePasswordAsync(new ChangePasswordRequest {
                    NewPassword = HashUtil.Sha1(NewPassword),
                    ActionCaptcha = ActionCaptcha,
                    ActionCaptchaId = ActionCaptchaId
                }, (response, e) => {
                    if (!response.IsSuccess()) {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), header: "修改密码失败", autoHideSeconds: 4);
                    }
                    else {
                        VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                    }
                });
            });
        }

        public string OldPassword {
            get => _oldPassword;
            set {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }

        public string NewPassword {
            get => _newPassword;
            set {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public string NewPasswordAgain {
            get => _newPasswordAgain;
            set {
                _newPasswordAgain = value;
                OnPropertyChanged(nameof(NewPasswordAgain));
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
                return $"http://{RpcRoot.OfficialServerAddress}/api/{ControllerUtil.GetControllerName<ICaptchaController<string>>()}/{nameof(ICaptchaController<string>.Get)}?id={ActionCaptchaId.ToString()}";
            }
        }
    }
}
