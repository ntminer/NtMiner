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
            VirtualRoot.AddEventPath<RpcUserLoginedEvent>("登录成功后刷新菜单", LogEnum.DevConsole, action: message => {
                this.OnPropertyChanged(nameof(IsMinerStudioOuterAdminLoginedVisible));
                this.OnPropertyChanged(nameof(IsMinerStudioLocalLoginedVisible));
            }, this.GetType());
        }

        public string LoginName {
            get {
                return RpcRoot.RpcUser.LoginName;
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

        public Visibility IsMinerStudioLocalOrOuterAdminLoginedVisible {
            get {
                if (RpcRoot.IsOuterNet) {
                    return IsMinerStudioOuterAdminLoginedVisible;
                }
                return IsMinerStudioLocalLoginedVisible;
            }
        }

        public bool IsMinerStudioOuterAdminLogined {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return true;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (!RpcRoot.IsLogined) {
                        return false;
                    }
                    if (!RpcRoot.RpcUser.LoginedUser.IsAdmin()) {
                        return false;
                    }
                    if (IsOfficialServerHost) {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        public bool IsOfficialServerHost {
            get {
                return NTMinerRegistry.GetControlCenterAddress().StartsWith(RpcRoot.OfficialServerHost);
            }
        }

        /// <summary>
        /// 登录的是外网群控管理员
        /// </summary>
        public Visibility IsMinerStudioOuterAdminLoginedVisible {
            get {
                return IsMinerStudioOuterAdminLogined ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 登录的是外网群控普通用户
        /// </summary>
        public Visibility IsMinerStudioOuterUserLoginedVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (!RpcRoot.IsLogined) {
                        return Visibility.Collapsed;
                    }
                    if (RpcRoot.RpcUser.LoginedUser.IsAdmin()) {
                        return Visibility.Collapsed;
                    }
                    if (IsOfficialServerHost) {
                        return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是外网登录用户，包括普通用户和Admin
        /// </summary>
        public Visibility IsMinerStudioOuterLoginedVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (RpcRoot.IsLogined && RpcRoot.IsOuterNet) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是内网群控
        /// </summary>
        public Visibility IsMinerStudioLocalLoginedVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerClient) {
                    return Visibility.Collapsed;
                }
                if (ClientAppType.IsMinerStudio) {
                    if (string.IsNullOrEmpty(RpcRoot.RpcUser.LoginName)) {
                        return Visibility.Collapsed;
                    }
                    if (Net.IpUtil.IsLocalhost(NTMinerRegistry.GetControlCenterAddress())) {
                        return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
