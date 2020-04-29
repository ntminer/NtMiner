using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class LoginWindow : Window {
        public static void Login(Action onLoginSuccess, Action btnCloseClick = null) {
            if (!RpcRoot.IsLogined) {
                var parent = WpfUtil.GetTopWindow();
                LoginWindow window = new LoginWindow(onLoginSuccess, btnCloseClick);
                if (parent != null && parent.GetType() != typeof(NotiCenterWindow)) {
                    window.Owner = parent;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.ShowInTaskbar = false;
                }
                window.ShowSoftDialog();
                window.PasswordFocus();
            }
            else {
                onLoginSuccess?.Invoke();
            }
        }

        public LoginWindowViewModel Vm { get; private set; }

        private readonly Action _onLoginSuccess;
        private readonly Action _btnCloseClick;
        private LoginWindow(Action onLoginSuccess, Action btnCloseClick) {
            _onLoginSuccess = onLoginSuccess;
            _btnCloseClick = btnCloseClick;
            this.Vm = new LoginWindowViewModel();
            this.DataContext = Vm;
            InitializeComponent();
            this.TbUcName.Text = nameof(LoginWindow);
            // 1个是通知窗口，1个是本窗口
            NotiCenterWindow.Bind(this, isNoOtherWindow: Application.Current.Windows.Count <= 2);
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

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
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
            NTMinerRegistry.SetControlCenterAddress(Vm.ServerHost);
            var list = NTMinerRegistry.GetControlCenterAddresses();
            if (!list.Contains(Vm.ServerHost)) {
                list.Insert(0, Vm.ServerHost);
            }
            NTMinerRegistry.SetControlCenterAddresses(list);
            // 内网免登录
            if (Net.IpUtil.IsInnerIp(Vm.ServerHost)) {
                RpcRoot.SetRpcUser(RpcUser.Empty);
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
                    RpcRoot.SetRpcUser(new RpcUser(response.Data, passwordSha1));
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
                    Vm.ShowMessage(response.ReadMessage(exception));
                }
            });
        }

        private void OpenServerHostsPopup() {
            var popup = PopupServerHosts;
            popup.IsOpen = true;
            var selected = Vm.ServerHost;
            var vm = new ServerHostSelectViewModel(selected, onOk: selectedResult => {
                if (selectedResult != null) {
                    if (Vm.ServerHost != selectedResult.IpOrHost) {
                        Vm.ServerHost = selectedResult.IpOrHost;
                    }
                    popup.IsOpen = false;
                }
            }) {
                HideView = new DelegateCommand(() => {
                    popup.IsOpen = false;
                })
            };
            popup.Child = new ServerHostSelect(vm);
        }

        private void ButtonServerHost_Click(object sender, RoutedEventArgs e) {
            OpenServerHostsPopup();
            e.Handled = true;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            _btnCloseClick?.Invoke();
        }
    }
}
