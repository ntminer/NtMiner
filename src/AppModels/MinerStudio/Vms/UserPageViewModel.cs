using NTMiner.User;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class UserPageViewModel : ViewModelBase {
        private ObservableCollection<UserViewModel> _queryResults = new ObservableCollection<UserViewModel>();
        private int _pageIndex = 1;
        private int _pageSize = 20;
        private int _total;
        private string _loginName = string.Empty;
        private string _email = string.Empty;
        private string _mobile = string.Empty;
        private string _role = string.Empty;
        private EnumItem<UserStatus> _userStatusEnumItem;
        private UserViewModel _selectedUserVm;

        public ICommand PageUp { get; private set; }
        public ICommand PageDown { get; private set; }
        public ICommand PageFirst { get; private set; }
        public ICommand PageLast { get; private set; }
        public ICommand PageRefresh { get; private set; }
        public ICommand Remove { get; private set; }
        public ICommand AddAdminRole { get; private set; }
        public ICommand RemoveAdminRole { get; private set; }

        public UserPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this._userStatusEnumItem = NTMinerContext.UserStatusEnumItems.FirstOrDefault(a => a.Value == UserStatus.All);
            this.PageUp = new DelegateCommand(() => {
                this.PageIndex -= 1;
            });
            this.PageDown = new DelegateCommand(() => {
                this.PageIndex += 1;
            });
            this.PageFirst = new DelegateCommand(() => {
                this.PageIndex = 1;
            });
            this.PageLast = new DelegateCommand(() => {
                this.PageIndex = PageCount;
            });
            this.PageRefresh = new DelegateCommand(Refresh);
            this.Remove = new DelegateCommand<UserViewModel>(vm => {
                if (!ClientAppType.IsMinerStudio) {
                    return;
                }
                if (string.IsNullOrEmpty(vm.LoginName)) {
                    return;
                }
                if (vm.LoginName == RpcRoot.RpcUser.LoginName) {
                    VirtualRoot.Out.ShowWarn("不能删除自己", header: "提示", autoHideSeconds: 4);
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{vm.LoginName}吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.UserService.RemoveUserAsync(vm.LoginName, (response, e) => {
                        if (response.IsSuccess()) {
                            Refresh();
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }));
            });
            this.AddAdminRole = new DelegateCommand<UserViewModel>(vm => {
                if (!ClientAppType.IsMinerStudio) {
                    return;
                }
                if (string.IsNullOrEmpty(vm.LoginName)) {
                    return;
                }
                if (vm.LoginName == RpcRoot.RpcUser.LoginName) {
                    VirtualRoot.Out.ShowWarn("不能操作自己", header: "提示", autoHideSeconds: 4);
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定把{vm.LoginName}设成超管吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.UserService.AddAdminRoleAsync(vm.LoginName, (response, e) => {
                        if (response.IsSuccess()) {
                            Refresh();
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }));
            }, vm => this.SelectedUserVm != null && !this.SelectedUserVm.IsAdmin());
            this.RemoveAdminRole = new DelegateCommand<UserViewModel>(vm => {
                if (!ClientAppType.IsMinerStudio) {
                    return;
                }
                if (string.IsNullOrEmpty(vm.LoginName)) {
                    return;
                }
                if (vm.LoginName == RpcRoot.RpcUser.LoginName) {
                    VirtualRoot.Out.ShowWarn("不能操作自己", header: "提示", autoHideSeconds: 4);
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定移除{vm.LoginName}的超管角色吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.UserService.RemoveAdminRoleAsync(vm.LoginName, (response, e) => {
                        if (response.IsSuccess()) {
                            Refresh();
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }));
            }, vm => this.SelectedUserVm != null && this.SelectedUserVm.IsAdmin());
            Refresh();
        }

        public void Refresh() {
            RpcRoot.OfficialServer.UserService.QueryUsersAsync(new QueryUsersRequest {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                LoginName = this.LoginName,
                Email = this.Email,
                Mobile = this.Mobile,
                Role = this.Role,
                UserStatus = UserStatusEnumItem.Value
            }, (response, e) => {
                if (response.IsSuccess()) {
                    UIThread.Execute(() => {
                        for (int i = 0; i < response.Data.Count; i++) {
                            var item = response.Data[i];
                            if (_queryResults.Count > i) {
                                var exist = _queryResults[i];
                                if (exist.LoginName != item.LoginName) {
                                    _queryResults.Insert(i, new UserViewModel(item));
                                }
                                else {
                                    exist.Update(item);
                                }
                            }
                            else {
                                _queryResults.Add(new UserViewModel(item));
                            }
                        }
                        while (_queryResults.Count > response.Data.Count) {
                            _queryResults.RemoveAt(_queryResults.Count - 1);
                        }
                        OnPropertyChanged(nameof(IsNodeRecordVisible));
                        RefreshPagingUi(response.Total);
                    });
                }
                else {
                    VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                }
            });
        }

        private void RefreshPagingUi(int total) {
            _total = total;
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(PageCount));
            OnPropertyChanged(nameof(IsPageDownEnabled));
            OnPropertyChanged(nameof(IsPageUpEnabled));
            if (Total == 0) {
                _pageIndex = 0;
                OnPropertyChanged(nameof(PageIndex));
            }
            else if (PageIndex == 0) {
                _pageIndex = 1;
                OnPropertyChanged(nameof(PageIndex));
            }
        }

        public UserViewModel SelectedUserVm {
            get => _selectedUserVm;
            set {
                if (_selectedUserVm != value) {
                    _selectedUserVm = value;
                    OnPropertyChanged(nameof(SelectedUserVm));
                }
            }
        }

        public int PageIndex {
            get => _pageIndex;
            set {
                _pageIndex = value;
                OnPropertyChanged(nameof(PageIndex));
                Refresh();
            }
        }

        public int PageCount {
            get {
                return (int)Math.Ceiling((double)this.Total / this.PageSize);
            }
        }

        public int PageSize {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged(nameof(PageSize));
                    this.PageIndex = 1;
                }
            }
        }

        private static readonly List<int> _pageSizeItems = new List<int>() { 10, 20, 30, 40 };
        public List<int> PageSizeItems {
            get {
                return _pageSizeItems;
            }
        }

        public bool IsPageUpEnabled {
            get {
                if (this.PageIndex <= 1) {
                    return false;
                }
                return true;
            }
        }

        public bool IsPageDownEnabled {
            get {
                if (this.PageIndex >= this.PageCount) {
                    return false;
                }
                return true;
            }
        }

        public int Total {
            get => _total;
            set {
                if (_total != value) {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public string LoginName {
            get => _loginName;
            set {
                if (_loginName != value) {
                    _loginName = value;
                    OnPropertyChanged(nameof(LoginName));
                    this.PageIndex = 1;
                }
            }
        }

        public string Email {
            get { return _email; }
            set {
                if (_email != value) {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                    this.PageIndex = 1;
                }
            }
        }

        public string Mobile {
            get { return _mobile; }
            set {
                if (_mobile != value) {
                    _mobile = value;
                    OnPropertyChanged(nameof(Mobile));
                    this.PageIndex = 1;
                }
            }
        }

        public EnumItem<UserStatus> UserStatusEnumItem {
            get => _userStatusEnumItem;
            set {
                if (_userStatusEnumItem != value) {
                    _userStatusEnumItem = value;
                    OnPropertyChanged(nameof(UserStatusEnumItem));
                    this.PageIndex = 1;
                }
            }
        }

        public string Role {
            get { return _role; }
            set {
                if (_role != value) {
                    _role = value;
                    OnPropertyChanged(nameof(Role));
                    this.PageIndex = 1;
                }
            }
        }

        public ObservableCollection<UserViewModel> QueryResults {
            get => _queryResults;
            set {
                _queryResults = value;
                OnPropertyChanged(nameof(QueryResults));
            }
        }

        public Visibility IsNodeRecordVisible {
            get {
                if (QueryResults.Count == 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
