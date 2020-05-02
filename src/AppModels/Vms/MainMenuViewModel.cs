using NTMiner.User;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MainMenuViewModel : ViewModelBase {
        public static readonly MainMenuViewModel Instance = new MainMenuViewModel();

        public MainMenuViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.AddEventPath<MinerStudioServiceSwitchedEvent>("群控后台客户端服务类型切换后刷新菜单的展示状态", LogEnum.DevConsole, action: message => {
                this.OnPropertyChanged(nameof(LoginName));
                this.OnPropertyChanged(nameof(IsMinerStudioLocalOrOuterAdminVisible));
                this.OnPropertyChanged(nameof(IsMinerStudioOuterAdmin));
                this.OnPropertyChanged(nameof(IsMinerStudioOuterAdminVisible));
                this.OnPropertyChanged(nameof(IsMinerStudioOuterVisible));
                this.OnPropertyChanged(nameof(IsMinerStudioLocalVisible));
            }, this.GetType());
        }

        public string LoginName {
            get {
                return JsonRpcRoot.RpcUser.LoginName;
            }
        }

        public SolidColorBrush TopItemForeground {
            get {
                if (ClientAppType.IsMinerClient) {
                    return WpfUtil.WhiteBrush;
                }
                return WpfUtil.BlackBrush;
            }
        }

        public Visibility IsMinerStudioLocalOrOuterAdminVisible {
            get {
                if (JsonRpcRoot.IsOuterNet) {
                    return IsMinerStudioOuterAdminVisible;
                }
                return IsMinerStudioLocalVisible;
            }
        }

        public bool IsMinerStudioOuterAdmin {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return true;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (!JsonRpcRoot.IsLogined) {
                        return false;
                    }
                    if (!JsonRpcRoot.RpcUser.LoginedUser.IsAdmin()) {
                        return false;
                    }
                    if (JsonRpcRoot.IsOuterNet) {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// 登录的是外网群控管理员
        /// </summary>
        public Visibility IsMinerStudioOuterAdminVisible {
            get {
                return IsMinerStudioOuterAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是外网登录用户，包括普通用户和Admin
        /// </summary>
        public Visibility IsMinerStudioOuterVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (JsonRpcRoot.IsLogined && JsonRpcRoot.IsOuterNet) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是内网群控
        /// </summary>
        public Visibility IsMinerStudioLocalVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (JsonRpcRoot.IsInnerNet) {
                        return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
