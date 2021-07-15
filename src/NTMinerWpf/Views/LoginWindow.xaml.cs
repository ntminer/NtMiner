using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : BlankWindow {
        public LoginWindowViewModel Vm { get; private set; }

        private readonly Action _onLoginSuccess;
        private readonly Action _btnCloseClick;
        internal LoginWindow(Action onLoginSuccess, string serverHost, Action btnCloseClick) {
            _onLoginSuccess = onLoginSuccess;
            _btnCloseClick = btnCloseClick;
            this.Vm = new LoginWindowViewModel(serverHost);
            this.DataContext = Vm;
            InitializeComponent();
            this.TbUcName.Text = nameof(LoginWindow);
            // 1个是通知窗口，1个是本窗口
            NotiCenterWindow.Bind(this);
            PasswordFocus();
        }

        public void PasswordFocus() {
            if (string.IsNullOrEmpty(this.TbLoginName.Text)) {
                this.TbLoginName.Focus();
            }
            else {
                this.PbPassword.Focus();
            }
        }

        private bool _isLogined = false;
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            bool isNoOwnerWindow = this.Owner == null || this.Owner.GetType() == typeof(NotiCenterWindow);
            if (isNoOwnerWindow && !_isLogined && Application.Current.ShutdownMode != ShutdownMode.OnMainWindowClose) {
                Application.Current.Shutdown();
            }
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(Vm.ServerHost)) {
                Vm.ShowMessage("服务器地址不能为空");
                return;
            }
            string passwordSha1 = HashUtil.Sha1(Vm.Password);
            // 内网免登录
            if (Vm.IsInnerIp) {
                RpcRoot.Login(RpcUser.Empty);
                RpcRoot.SetIsOuterNet(false);
                _isLogined = true;
                this.Close();
                // 回调可能弹窗，弹窗可能有父窗口，父窗口是顶层窗口，如果在this.Close()之前回调
                // 则会导致弹窗的父窗口是本窗口，而本窗口随后立即关闭导致作为子窗口的弹窗也会被关闭。
                _onLoginSuccess?.Invoke();
                return;
            }
            else if (string.IsNullOrEmpty(Vm.LoginName)) {
                Vm.ShowMessage("没有填写用户名");
                return;
            }
            else if (string.IsNullOrEmpty(Vm.Password)) {
                Vm.ShowMessage("没有填写密码");
                return;
            }
            RpcRoot.OfficialServer.UserService.LoginAsync(Vm.LoginName, passwordSha1, (response, exception) => {
                if (response == null) {
                    Vm.ShowMessage("服务器忙");
                    return;
                }
                if (response.IsSuccess()) {
                    RpcRoot.Login(new RpcUser(response.Data, passwordSha1));
                    RpcRoot.SetIsOuterNet(true);
                    _isLogined = true;
                    UIThread.Execute(() => {
                        this.Close();
                    });
                    // 回调可能弹窗，弹窗可能有父窗口，父窗口是顶层窗口，如果在this.Close()之前回调
                    // 则会导致弹窗的父窗口是本窗口，而本窗口随后立即关闭导致作为子窗口的弹窗也会被关闭。
                    _onLoginSuccess?.Invoke();
                }
                else {
                    Vm.ShowMessage(response.ReadMessage(exception), autoHideSeconds: 0);
                }
            });
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            _btnCloseClick?.Invoke();
        }
    }
}
