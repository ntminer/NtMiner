using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.RemoteDesktop;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.MinerStudio.Vms {
    public class MinerClientViewModel : ViewModelBase, IClientData, IEntity<string> {
        private double _incomeMainCoinPerDay;
        private double _incomeMainCoinUsdPerDay;
        private double _incomeMainCoinCnyPerDay;
        private double _incomeDualCoinPerDay;
        private double _incomeDualCoinUsdPerDay;
        private double _incomeDualCoinCnyPerDay;
        private MinerGroupViewModel _selectedMinerGroup;
        private SolidColorBrush _dualCoinRejectPercentForeground;
        private SolidColorBrush _mainCoinRejectPercentForeground;
        private string _mainCoinRejectPercentText;
        private string _dualCoinRejectPercentText;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand RemoteDesktop { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand Remove { get; private set; }

        private readonly ClientData _data;
        #region ctor
        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public MinerClientViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public MinerClientViewModel(ClientData clientData) {
            _data = clientData;
            this._mainCoinRejectPercentText = clientData.MainCoinRejectPercent.ToString("f1") + " %";
            this._dualCoinRejectPercentText = clientData.DualCoinRejectPercent.ToString("f1") + " %";
            RefreshMainCoinIncome();
            RefreshDualCoinIncome();
            this.Remove = new DelegateCommand(() => {
                #region
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除该矿机吗？", title: "确认", onYes: () => {
                    MinerStudioRoot.MinerStudioService.RemoveClientsAsync(new List<string> { this.Id }, (response, e) => {
                        if (!response.IsSuccess()) {
                            VirtualRoot.Out.ShowError("删除矿机失败：" + response.ReadMessage(e), autoHideSeconds: 4, toConsole: true);
                        }
                        else {
                            MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
                        }
                    });
                }));
                #endregion
            });
            this.RemoteDesktop = new DelegateCommand(() => {
                #region
                if (!MinerIpExtensions.TryGetFirstIp(this.LocalIp, out string ip)) {
                    if (MinerIpExtensions.TryGetFirstIp(this.MinerIp, out string minerIp) && Net.IpUtil.IsInnerIp(minerIp)) {
                        ip = minerIp;
                    }
                    else {
                        VirtualRoot.Out.ShowWarn("Ip地址不能为空", autoHideSeconds: 4);
                        return;
                    }
                }
                if (string.IsNullOrEmpty(this.WindowsLoginName)) {
                    VirtualRoot.Execute(new ShowRemoteDesktopLoginDialogCommand(new RemoteDesktopLoginViewModel {
                        Title = "连接远程桌面 - " + ip,
                        OnOk = vm => {
                            this.WindowsLoginName = vm.LoginName;
                            this.WindowsPassword = vm.Password;
                            AppRoot.RemoteDesktop?.Invoke(new RdpInput(ip, this.WindowsLoginName, this.WindowsPassword, this.MinerName, onDisconnected: message => {
                                VirtualRoot.Out.ShowError(message, autoHideSeconds: 4, toConsole: true);
                            }));
                        }
                    }));
                }
                else {
                    AppRoot.RemoteDesktop?.Invoke(new RdpInput(ip, this.WindowsLoginName, this.WindowsPassword, this.MinerName, onDisconnected: message => {
                        VirtualRoot.Out.ShowError(message, autoHideSeconds: 4, toConsole: true);
                    }));
                }
                #endregion
            });
            this.RestartWindows = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定重启{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    MinerStudioRoot.MinerStudioService.RestartWindowsAsync(this);
                }));
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定关闭{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    MinerStudioRoot.MinerStudioService.ShutdownWindowsAsync(this);
                }));
            });
            this.StartMine = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"{this.MinerName}({this.MinerIp})：确定开始挖矿吗？", title: "确认", onYes: () => {
                    MinerStudioRoot.MinerStudioService.StartMineAsync(this, WorkId);
                }));
            });
            this.StopMine = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"{this.MinerName}({this.MinerIp})：确定停止挖矿吗？", title: "确认", onYes: () => {
                    MinerStudioRoot.MinerStudioService.StopMineAsync(this);
                }));
            });
        }
        #endregion

        public string GetRemoteDesktopIp() {
            if (string.IsNullOrEmpty(LocalIp)) {
                return MinerIp;
            }
            return LocalIp;
        }

        // 便于工具追踪代码
        public void Update(ClientData data) {
            UpdateByReflection.Update(this, data);
        }

        public MinerStudioRoot.MineWorkViewModels MineWorkVms {
            get { return MinerStudioRoot.MineWorkVms; }
        }

        public MinerStudioRoot.MinerGroupViewModels MinerGroupVms {
            get { return MinerStudioRoot.MinerGroupVms; }
        }

        public MainMenuViewModel MainMenu {
            get {
                return MainMenuViewModel.Instance;
            }
        }

        #region IClientData

        public string GetId() {
            return this.Id;
        }

        public string Id {
            get => _data.Id;
            set {
                if (_data.Id != value) {
                    _data.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public Guid MineContextId {
            get { return _data.MineContextId; }
            set {
                if (_data.MineContextId != value) {
                    _data.MineContextId = value;
                    OnPropertyChanged(nameof(MineContextId));
                }
            }
        }

        public Guid ClientId {
            get { return _data.ClientId; }
            set {
                if (_data.ClientId != value) {
                    _data.ClientId = value;
                    OnPropertyChanged(nameof(ClientId));
                }
            }
        }

        public string MACAddress {
            get { return _data.MACAddress; }
            set {
                if (_data.MACAddress != value) {
                    _data.MACAddress = value;
                    OnPropertyChanged(nameof(MACAddress));
                }
            }
        }

        public string LocalIp {
            get { return _data.LocalIp; }
            set {
                if (_data.LocalIp != value) {
                    _data.LocalIp = value;
                    OnPropertyChanged(nameof(LocalIp));
                }
            }
        }

        public bool IsAutoBoot {
            get { return _data.IsAutoBoot; }
            set {
                if (_data.IsAutoBoot != value) {
                    _data.IsAutoBoot = value;
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public bool IsAutoStart {
            get { return _data.IsAutoStart; }
            set {
                if (_data.IsAutoStart != value) {
                    _data.IsAutoStart = value;
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public int AutoStartDelaySeconds {
            get { return _data.AutoStartDelaySeconds; }
            set {
                if (_data.AutoStartDelaySeconds != value) {
                    _data.AutoStartDelaySeconds = value;
                    OnPropertyChanged(nameof(AutoStartDelaySeconds));
                }
            }
        }

        public Guid WorkId {
            get => _data.WorkId;
            set {
                if (_data.WorkId != value) {
                    _data.WorkId = value;
                    OnPropertyChanged(nameof(WorkId));
                    OnPropertyChanged(nameof(SelectedMineWork));
                }
            }
        }

        public Guid MineWorkId {
            get => _data.MineWorkId;
            set {
                if (_data.MineWorkId != value) {
                    _data.MineWorkId = value;
                    OnPropertyChanged(nameof(MineWorkId));
                }
            }
        }

        public string MineWorkName {
            get => _data.MineWorkName;
            set {
                if (_data.MineWorkName != value) {
                    _data.MineWorkName = value;
                    OnPropertyChanged(nameof(MineWorkName));
                }
            }
        }

        private MineWorkViewModel _selectedMineWork;
        [IgnoreReflectionSet]
        public MineWorkViewModel SelectedMineWork {
            get {
                if (WorkId == Guid.Empty) {
                    return MineWorkViewModel.PleaseSelect;
                }
                if (_selectedMineWork == null || _selectedMineWork.Id != WorkId) {
                    if (MinerStudioRoot.MineWorkVms.TryGetMineWorkVm(WorkId, out _selectedMineWork)) {
                        return _selectedMineWork;
                    }
                }
                return _selectedMineWork;
            }
            set {
                if (_selectedMineWork != value) {
                    if (value == null) {
                        value = MineWorkViewModel.PleaseSelect;
                    }
                    var old = _selectedMineWork;
                    this.WorkId = value.Id;
                    _selectedMineWork = value;
                    MinerStudioRoot.MinerStudioService.UpdateClientAsync(
                        this.Id, nameof(WorkId), value.Id, (response, exception) => {
                            if (!response.IsSuccess()) {
                                _selectedMineWork = old;
                                this.WorkId = old.Id;
                                VirtualRoot.Out.ShowError($"{this.MinerName} {this.MinerIp} {response.ReadMessage(exception)}", toConsole: true);
                            }
                            OnPropertyChanged(nameof(SelectedMineWork));
                        });
                }
            }
        }

        public string Version {
            get => _data.Version;
            set {
                if (_data.Version != value) {
                    _data.Version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }

        public DateTime MinerActiveOn {
            get => _data.MinerActiveOn;
            set {
                if (_data.MinerActiveOn != value) {
                    _data.MinerActiveOn = value;
                    OnPropertyChanged(nameof(MinerActiveOn));
                }
                OnPropertyChanged(nameof(IsMining));
                OnPropertyChanged(nameof(LastActivedOnText));
            }
        }

        public bool IsOnline {
            get => _data.IsOnline;
            set {
                if (_data.IsOnline != value) {
                    _data.IsOnline = value;
                    OnPropertyChanged(nameof(IsOnline));
                }
            }
        }

        public DateTime NetActiveOn {
            get => _data.NetActiveOn;
            set {
                if (_data.NetActiveOn != value) {
                    _data.NetActiveOn = value;
                    OnPropertyChanged(nameof(NetActiveOn));
                }
                // 因为以下和时间有关，所以时间不等也是不等
                OnPropertyChanged(nameof(VmIsOnline));
                OnPropertyChanged(nameof(VmIsOnlineText));
                OnPropertyChanged(nameof(IsMining));
                OnPropertyChanged(nameof(BootTimeSpanText));
                OnPropertyChanged(nameof(MineTimeSpanText));
            }
        }

        public string LastActivedOnText {
            get {
                if (MinerActiveOn <= Timestamp.UnixBaseTime) {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanText(MinerActiveOn);
            }
        }

        public DateTime BootOn {
            get { return _data.BootOn; }
            set {
                if (_data.BootOn != value) {
                    _data.BootOn = value;
                    OnPropertyChanged(nameof(BootOn));
                    OnPropertyChanged(nameof(BootTimeSpanText));
                }
            }
        }

        private static string TimeSpanToString(TimeSpan timeSpan) {
            if (timeSpan.Days >= 1) {
                return $"{timeSpan.Days.ToString()}天{timeSpan.Hours.ToString()}小时{timeSpan.Minutes.ToString()}分钟";
            }
            if (timeSpan.Hours > 0) {
                return $"{timeSpan.Hours.ToString()}小时{timeSpan.Minutes.ToString()}分钟";
            }
            if (timeSpan.Minutes > 2) {
                return $"{timeSpan.Minutes.ToString()}分钟";
            }
            return (int)timeSpan.TotalSeconds + "秒";
        }

        public string BootTimeSpanText {
            get {
                if (BootOn <= Timestamp.UnixBaseTime) {
                    return "-";
                }
                else {
                    if (RpcRoot.IsOuterNet) {
                        // 即使启用了外网群控也是每2分钟才会上报一次算力，但群控客户端会向矿机列表当页的矿机发起获取算力的请求，
                        // 但当用户刚翻倒第二页的时候或刚打开看到第一页的时候并未提前刷新，所以MinerActiveOn的周期必须大于2分钟。
                        if (MinerActiveOn.AddSeconds(180) < DateTime.Now) {
                            return "-";
                        }
                    }
                    else if (MinerActiveOn.AddSeconds(20) < DateTime.Now) {
                        return "-";
                    }
                }
                return TimeSpanToString(DateTime.Now - BootOn);
            }
        }

        public bool VmIsOnline {
            get {
                if (!IsOnline) {
                    return false;
                }
                if (RpcRoot.IsOuterNet) {
                    if (this.IsOuterUserEnabled) {
                        if (NetActiveOn.AddSeconds(60) < DateTime.Now) {
                            return false;
                        }
                    }
                    else if (NetActiveOn.AddSeconds(180) < DateTime.Now) {
                        return false;
                    }
                    return true;
                }
                if (NetActiveOn.AddSeconds(20) < DateTime.Now) {
                    return false;
                }
                return true;
            }
        }

        public string VmIsOnlineText {
            get {
                if (VmIsOnline) {
                    return "在线";
                }
                return "离线";
            }
        }

        public Visibility VmIsOnlineVisible {
            get {
                if (RpcRoot.IsOuterNet && !this.IsOuterUserEnabled) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public DateTime? MineStartedOn {
            get { return _data.MineStartedOn; }
            set {
                if (_data.MineStartedOn != value) {
                    _data.MineStartedOn = value;
                    OnPropertyChanged(nameof(MineStartedOn));
                    OnPropertyChanged(nameof(MineTimeSpanText));
                }
            }
        }

        public string MineTimeSpanText {
            get {
                if (!MineStartedOn.HasValue || MineStartedOn.Value <= Timestamp.UnixBaseTime || !this.IsMining) {
                    return "-";
                }

                return TimeSpanToString(DateTime.Now - MineStartedOn.Value);
            }
        }

        public bool IsMining {
            get {
                if (RpcRoot.IsOuterNet) {
                    if (this.IsOuterUserEnabled) {
                        if (NetActiveOn.AddSeconds(60) < DateTime.Now) {
                            return false;
                        }
                    }
                    else if (MinerActiveOn.AddSeconds(180) < DateTime.Now) {
                        return false;
                    }
                }
                else if (NetActiveOn.AddSeconds(20) < DateTime.Now) {
                    return false;
                }
                return _data.IsMining;
            }
            set {
                if (_data.IsMining != value) {
                    _data.IsMining = value;
                    OnPropertyChanged(nameof(IsMining));
                    OnPropertyChanged(nameof(IsMiningText));
                }
            }
        }

        public string IsMiningText {
            get {
                if (IsMining) {
                    return "挖矿中";
                }
                return "未挖矿";
            }
        }

        public string WorkerName {
            get {
                return _data.WorkerName;
            }
            set {
                if (_data.WorkerName != value) {
                    var old = _data.WorkerName;
                    _data.WorkerName = value;
                    MinerStudioRoot.MinerStudioService.UpdateClientAsync(this.Id, nameof(WorkerName), value, (response, e) => {
                        if (!response.IsSuccess()) {
                            _data.WorkerName = old;
                            VirtualRoot.Out.ShowError($"设置群控名失败：{this.WorkerName} {this.MinerIp} {response.ReadMessage(e)}", toConsole: true);
                        }
                        OnPropertyChanged(nameof(WorkerName));
                        OnPropertyChanged(nameof(WorkerNameText));
                    });
                }
            }
        }

        public string WorkerNameText {
            get {
                if (string.IsNullOrEmpty(_data.WorkerName)) {
                    return string.Empty;
                }
                return _data.WorkerName;
            }
        }

        public string MinerName {
            get => _data.MinerName;
            set {
                if (_data.MinerName != value) {
                    _data.MinerName = value;
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public string GetMinerOrClientName() {
            if (string.IsNullOrEmpty(MinerName)) {
                return WorkerName;
            }
            return MinerName;
        }

        public Guid GroupId {
            get { return _data.GroupId; }
            set {
                if (_data.GroupId != value) {
                    _data.GroupId = value;
                    OnPropertyChanged(nameof(GroupId));
                    OnPropertyChanged(nameof(SelectedMinerGroup));
                }
            }
        }

        [IgnoreReflectionSet]
        public MinerGroupViewModel SelectedMinerGroup {
            get {
                if (_selectedMinerGroup == null || _selectedMinerGroup.Id != GroupId) {
                    MinerStudioRoot.MinerGroupVms.TryGetMineWorkVm(GroupId, out _selectedMinerGroup);
                    if (_selectedMinerGroup == null) {
                        _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                    }
                }
                return _selectedMinerGroup;
            }
            set {
                if (_selectedMinerGroup != value) {
                    if (value == null) {
                        value = MinerGroupViewModel.PleaseSelect;
                    }
                    var old = _selectedMinerGroup;
                    _selectedMinerGroup = value;
                    this.GroupId = value.Id;
                    MinerStudioRoot.MinerStudioService.UpdateClientAsync(this.Id, nameof(GroupId), value.Id, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _selectedMinerGroup = old;
                            this.GroupId = old.Id;
                            VirtualRoot.Out.ShowError($"加入分组失败：{this.MinerName} {this.MinerIp} {response.ReadMessage(exception)}", toConsole: true);
                        }
                        OnPropertyChanged(nameof(SelectedMinerGroup));
                    });
                }
            }
        }

        public string MinerIp {
            get => _data.MinerIp;
            set {
                if (_data.MinerIp != value) {
                    _data.MinerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                }
            }
        }

        [IgnoreReflectionSet]
        public string WindowsLoginName {
            get { return _data.WindowsLoginName; }
            set {
                if (_data.WindowsLoginName != value) {
                    var old = _data.WindowsLoginName;
                    _data.WindowsLoginName = value;
                    MinerStudioRoot.MinerStudioService.UpdateClientAsync(this.Id, nameof(WindowsLoginName), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsLoginName = old;
                            VirtualRoot.Out.ShowError($"设置Windows远程登录用户名失败：{this.MinerName} {this.MinerIp} {response.ReadMessage(exception)}", toConsole: true);
                        }
                        OnPropertyChanged(nameof(WindowsLoginName));
                    });
                }
            }
        }

        [IgnoreReflectionSet]
        public string WindowsPassword {
            get {
                if (string.IsNullOrEmpty(_data.WindowsPassword)) {
                    return string.Empty;
                }
                return Cryptography.QuickUtil.TextDecrypt(Convert.FromBase64String(_data.WindowsPassword), RpcRoot.RpcUser.Password);
            }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    // 不在网络上传输windows密码原文，传输的是密文
                    value = Convert.ToBase64String(Cryptography.QuickUtil.TextEncrypt(value, RpcRoot.RpcUser.Password));
                }
                if (_data.WindowsPassword != value) {
                    var old = _data.WindowsPassword;
                    _data.WindowsPassword = value;
                    MinerStudioRoot.MinerStudioService.UpdateClientAsync(this.Id, nameof(WindowsPassword), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsPassword = old;
                            VirtualRoot.Out.ShowError($"设置Widnows远程登录密码失败：{this.MinerName} {this.MinerIp} {response.ReadMessage(exception)}", toConsole: true);
                        }
                        OnPropertyChanged(nameof(WindowsPassword));
                        OnPropertyChanged(nameof(WindowsPasswordStar));
                    });
                    OnPropertyChanged(nameof(WindowsPassword));
                    OnPropertyChanged(nameof(WindowsPasswordStar));
                }
            }
        }

        public string WindowsPasswordStar {
            get {
                if (string.IsNullOrEmpty(this.WindowsPassword)) {
                    return string.Empty;
                }
                return new string('●', 6);
            }
        }

        public string MainCoinCode {
            get => _data.MainCoinCode ?? string.Empty;
            set {
                if (_data.MainCoinCode != value) {
                    _data.MainCoinCode = value;
                    OnPropertyChanged(nameof(MainCoinCode));
                }
            }
        }

        public double MainCoinSpeed {
            get => _data.MainCoinSpeed;
            set {
                if (Math.Abs(_data.MainCoinSpeed - value) > 0.01) {
                    _data.MainCoinSpeed = value;
                    OnPropertyChanged(nameof(MainCoinSpeed));
                    OnPropertyChanged(nameof(MainCoinSpeedText));
                    RefreshMainCoinIncome();
                }
            }
        }

        private void RefreshMainCoinIncome() {
            IncomePerDay incomePerDay = NTMinerContext.Instance.CalcConfigSet.GetIncomePerHashPerDay(this.MainCoinCode);
            IncomeMainCoinPerDay = MainCoinSpeed * incomePerDay.IncomeCoin;
            IncomeMainCoinUsdPerDay = MainCoinSpeed * incomePerDay.IncomeUsd;
            IncomeMainCoinCnyPerDay = MainCoinSpeed * incomePerDay.IncomeCny;
        }

        public string MainCoinSpeedText {
            get {
                return this.MainCoinSpeed.ToUnitSpeedText();
            }
        }

        public int MainCoinTotalShare {
            get { return _data.MainCoinTotalShare; }
            set {
                if (_data.MainCoinTotalShare != value) {
                    _data.MainCoinTotalShare = value;
                    OnPropertyChanged(nameof(MainCoinTotalShare));
                }
            }
        }

        public int MainCoinRejectShare {
            get { return _data.MainCoinRejectShare; }
            set {
                if (_data.MainCoinRejectShare != value) {
                    _data.MainCoinRejectShare = value;
                    OnPropertyChanged(nameof(MainCoinRejectShare));
                }
            }
        }

        public double MainCoinRejectPercent {
            get {
                return _data.MainCoinRejectPercent;
            }
            set {
                if (_data.MainCoinRejectPercent != value) {
                    _data.MainCoinRejectPercent = value;
                    OnPropertyChanged(nameof(MainCoinRejectPercent));
                    SetMainCoinRejectPercentText(value);
                }
            }
        }

        private void SetMainCoinRejectPercentText(double value) {
            this.MainCoinRejectPercentText = value.ToString("f1") + " %";
        }

        public string MainCoinRejectPercentText {
            get {
                return _mainCoinRejectPercentText;
            }
            set {
                _mainCoinRejectPercentText = value;
                OnPropertyChanged(nameof(MainCoinRejectPercentText));
            }
        }

        public double IncomeMainCoinPerDay {
            get => _incomeMainCoinPerDay;
            set {
                if (_incomeMainCoinPerDay != value) {
                    _incomeMainCoinPerDay = value;
                    OnPropertyChanged(nameof(IncomeMainCoinPerDay));
                    OnPropertyChanged(nameof(IncomeMainCoinPerDayText));
                }
            }
        }

        public string IncomeMainCoinPerDayText {
            get {
                return IncomeMainCoinPerDay.ToString("f7");
            }
        }

        public double IncomeMainCoinUsdPerDay {
            get { return _incomeMainCoinUsdPerDay; }
            set {
                if (_incomeMainCoinUsdPerDay != value) {
                    _incomeMainCoinUsdPerDay = value;
                    OnPropertyChanged(nameof(IncomeMainCoinUsdPerDay));
                    OnPropertyChanged(nameof(IncomeMainCoinUsdPerDayText));
                }
            }
        }

        public string IncomeMainCoinUsdPerDayText {
            get {
                return IncomeMainCoinUsdPerDay.ToString("f2");
            }
        }

        public double IncomeMainCoinCnyPerDay {
            get { return _incomeMainCoinCnyPerDay; }
            set {
                if (_incomeMainCoinCnyPerDay != value) {
                    _incomeMainCoinCnyPerDay = value;
                    OnPropertyChanged(nameof(IncomeMainCoinCnyPerDay));
                    OnPropertyChanged(nameof(IncomeMainCoinCnyPerDayText));
                }
            }
        }

        public string IncomeMainCoinCnyPerDayText {
            get {
                return IncomeMainCoinCnyPerDay.ToString("f2");
            }
        }

        public double IncomeDualCoinPerDay {
            get => _incomeDualCoinPerDay;
            set {
                if (_incomeDualCoinPerDay != value) {
                    _incomeDualCoinPerDay = value;
                    OnPropertyChanged(nameof(IncomeDualCoinPerDay));
                    OnPropertyChanged(nameof(IncomeDualCoinPerDayText));
                }
            }
        }

        public string IncomeDualCoinPerDayText {
            get {
                return IncomeDualCoinPerDay.ToString("f7");
            }
        }

        public double IncomeDualCoinUsdPerDay {
            get { return _incomeDualCoinUsdPerDay; }
            set {
                if (_incomeDualCoinUsdPerDay != value) {
                    _incomeDualCoinUsdPerDay = value;
                    OnPropertyChanged(nameof(IncomeDualCoinUsdPerDay));
                    OnPropertyChanged(nameof(IncomeDualCoinUsdPerDayText));
                }
            }
        }

        public string IncomeDualCoinUsdPerDayText {
            get {
                return IncomeDualCoinUsdPerDay.ToString("f2");
            }
        }

        public double IncomeDualCoinCnyPerDay {
            get { return _incomeDualCoinCnyPerDay; }
            set {
                if (_incomeDualCoinCnyPerDay != value) {
                    _incomeDualCoinCnyPerDay = value;
                    OnPropertyChanged(nameof(IncomeDualCoinCnyPerDay));
                    OnPropertyChanged(nameof(IncomeDualCoinCnyPerDayText));
                }
            }
        }

        public string IncomeDualCoinCnyPerDayText {
            get {
                return IncomeDualCoinCnyPerDay.ToString("f2");
            }
        }

        public string MainCoinPool {
            get => _data.MainCoinPool;
            set {
                if (_data.MainCoinPool != value) {
                    _data.MainCoinPool = value;
                    OnPropertyChanged(nameof(MainCoinPool));
                }
            }
        }

        public string MainCoinWallet {
            get => _data.MainCoinWallet;
            set {
                if (_data.MainCoinWallet != value) {
                    _data.MainCoinWallet = value;
                    OnPropertyChanged(nameof(MainCoinWallet));
                }
            }
        }

        public string Kernel {
            get => _data.Kernel;
            set {
                if (_data.Kernel != value) {
                    _data.Kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get => _data.IsDualCoinEnabled;
            set {
                if (_data.IsDualCoinEnabled != value) {
                    _data.IsDualCoinEnabled = value;
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                }
            }
        }

        public string DualCoinCode {
            get => _data.DualCoinCode ?? string.Empty;
            set {
                if (_data.DualCoinCode != value) {
                    _data.DualCoinCode = value;
                    OnPropertyChanged(nameof(DualCoinCode));
                }
            }
        }

        public double DualCoinSpeed {
            get => _data.DualCoinSpeed;
            set {
                if (_data.DualCoinSpeed != value) {
                    _data.DualCoinSpeed = value;
                    OnPropertyChanged(nameof(DualCoinSpeed));
                    OnPropertyChanged(nameof(DualCoinSpeedText));
                    RefreshDualCoinIncome();
                }
            }
        }

        private void RefreshDualCoinIncome() {
            IncomePerDay incomePerDay = NTMinerContext.Instance.CalcConfigSet.GetIncomePerHashPerDay(this.DualCoinCode);
            IncomeDualCoinPerDay = DualCoinSpeed * incomePerDay.IncomeCoin;
            IncomeDualCoinUsdPerDay = DualCoinSpeed * incomePerDay.IncomeUsd;
            IncomeDualCoinCnyPerDay = DualCoinSpeed * incomePerDay.IncomeCny;
        }

        public string DualCoinSpeedText {
            get {
                return this.DualCoinSpeed.ToUnitSpeedText();
            }
        }

        public int DualCoinTotalShare {
            get { return _data.DualCoinTotalShare; }
            set {
                if (_data.DualCoinTotalShare != value) {
                    _data.DualCoinTotalShare = value;
                    OnPropertyChanged(nameof(DualCoinTotalShare));
                }
            }
        }

        public int DualCoinRejectShare {
            get { return _data.DualCoinRejectShare; }
            set {
                if (_data.DualCoinRejectShare != value) {
                    _data.DualCoinRejectShare = value;
                    OnPropertyChanged(nameof(DualCoinRejectShare));
                }
            }
        }

        public double DualCoinRejectPercent {
            get {
                return _data.DualCoinRejectPercent;
            }
            set {
                if (_data.DualCoinRejectPercent != value) {
                    _data.DualCoinRejectPercent = value;
                    OnPropertyChanged(nameof(DualCoinRejectPercent));
                    SetDualCoinRejectPercentText(value);
                }
            }
        }

        private void SetDualCoinRejectPercentText(double value) {
            this.DualCoinRejectPercentText = value.ToString("f1") + " %";
        }

        public string DualCoinRejectPercentText {
            get {
                return _dualCoinRejectPercentText;
            }
            set {
                _dualCoinRejectPercentText = value;
                OnPropertyChanged(nameof(DualCoinRejectPercentText));
            }
        }

        public string DualCoinPool {
            get => _data.DualCoinPool;
            set {
                if (_data.DualCoinPool != value) {
                    _data.DualCoinPool = value;
                    OnPropertyChanged(nameof(DualCoinPool));
                }
            }
        }

        public string DualCoinWallet {
            get => _data.DualCoinWallet;
            set {
                if (_data.DualCoinWallet != value) {
                    _data.DualCoinWallet = value;
                    OnPropertyChanged(nameof(DualCoinWallet));
                }
            }
        }

        public string GpuInfo {
            get => _data.GpuInfo;
            set {
                if (_data.GpuInfo != value) {
                    _data.GpuInfo = value;
                    OnPropertyChanged(nameof(GpuInfo));
                }
            }
        }

        public string OSName {
            get { return _data.OSName; }
            set {
                if (_data.OSName != value) {
                    _data.OSName = value;
                    OnPropertyChanged(nameof(OSName));
                }
            }
        }

        public int OSVirtualMemoryMb {
            get => _data.OSVirtualMemoryMb;
            set {
                if (_data.OSVirtualMemoryMb != value) {
                    _data.OSVirtualMemoryMb = value;
                    OnPropertyChanged(nameof(OSVirtualMemoryMb));
                    OnPropertyChanged(nameof(OSVirtualMemoryGbText));
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public string OSVirtualMemoryGbText {
            get {
                return (this.OSVirtualMemoryMb / 1024.0).ToString("f1") + " Gb";
            }
        }

        public int TotalPhysicalMemoryMb {
            get {
                return _data.TotalPhysicalMemoryMb;
            }
            set {
                if (_data.TotalPhysicalMemoryMb != value) {
                    _data.TotalPhysicalMemoryMb = value;
                    OnPropertyChanged(nameof(TotalPhysicalMemoryMb));
                    OnPropertyChanged(nameof(TotalPhysicalMemoryGbText));
                }
            }
        }

        public string TotalPhysicalMemoryGbText {
            get {
                return (this.TotalPhysicalMemoryMb / 1024.0).ToString("f1") + " Gb";
            }
        }

        public string DiskSpace {
            get { return _data.DiskSpace; }
            set {
                if (_data.DiskSpace != value) {
                    _data.DiskSpace = value;
                    OnPropertyChanged(nameof(DiskSpace));
                }
            }
        }

        public GpuType GpuType {
            get => _data.GpuType;
            set {
                if (_data.GpuType != value) {
                    _data.GpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    OnPropertyChanged(nameof(IsNvidiaIconVisible));
                    OnPropertyChanged(nameof(IsAMDIconVisible));
                }
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                return GpuType == GpuType.NVIDIA ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // ReSharper disable once InconsistentNaming
        public Visibility IsAMDIconVisible {
            get {
                return GpuType == GpuType.AMD ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public int GpuCount {
            get { return GpuTable.Length; }
        }

        public string GpuDriver {
            get => _data.GpuDriver;
            set {
                if (_data.GpuDriver != value) {
                    _data.GpuDriver = value;
                    OnPropertyChanged(nameof(GpuDriver));
                }
            }
        }

        public string KernelCommandLine {
            get => _data.KernelCommandLine;
            set {
                if (_data.KernelCommandLine != value) {
                    _data.KernelCommandLine = value;
                    OnPropertyChanged(nameof(KernelCommandLine));
                }
            }
        }

        public uint TotalPower {
            get { return (uint)GpuTable.Sum(a => a.PowerUsage); }
        }

        public string TotalPowerText {
            get { return $"{GpuTable.Sum(a => a.PowerUsage).ToString("f0")}W"; }
        }

        public SolidColorBrush MainCoinRejectPercentForeground {
            get => _mainCoinRejectPercentForeground;
            set {
                if (_mainCoinRejectPercentForeground != value) {
                    _mainCoinRejectPercentForeground = value;
                    OnPropertyChanged(nameof(MainCoinRejectPercentForeground));
                }
            }
        }

        public SolidColorBrush DualCoinRejectPercentForeground {
            get => _dualCoinRejectPercentForeground;
            set {
                if (_dualCoinRejectPercentForeground != value) {
                    _dualCoinRejectPercentForeground = value;
                    OnPropertyChanged(nameof(DualCoinRejectPercentForeground));
                }
            }
        }

        public GpuSpeedData[] GpuTable {
            get => _data.GpuTable;
            set {
                _data.GpuTable = value;
                OnPropertyChanged(nameof(GpuTable));
                OnPropertyChanged(nameof(TotalPower));
                OnPropertyChanged(nameof(TotalPowerText));
                OnPropertyChanged(nameof(GpuCount));
                int maxTemperature = _data.GpuTable.Length == 0 ? 0 : _data.GpuTable.Max(a => a.Temperature);
                this.GpuTableVm = new GpuSpeedDataViewModels(
                    MainCoinCode, DualCoinCode, MainCoinSpeedText,
                    DualCoinSpeedText, TotalPowerText,
                    IsRejectOneGpuShare, IsFoundOneGpuShare, IsGotOneIncorrectGpuShare,
                    CpuPerformance, CpuTemperature, maxTemperature, value);
            }
        }

        public bool IsRejectOneGpuShare {
            get => _data.IsRejectOneGpuShare;
            set {
                if (value != _data.IsRejectOneGpuShare) {
                    _data.IsRejectOneGpuShare = value;
                    OnPropertyChanged(nameof(IsRejectOneGpuShare));
                }
            }
        }

        public bool IsFoundOneGpuShare {
            get => _data.IsFoundOneGpuShare;
            set {
                if (_data.IsFoundOneGpuShare != value) {
                    _data.IsFoundOneGpuShare = value;
                    OnPropertyChanged(nameof(IsFoundOneGpuShare));
                }
            }
        }

        public bool IsGotOneIncorrectGpuShare {
            get => _data.IsGotOneIncorrectGpuShare;
            set {
                if (_data.IsGotOneIncorrectGpuShare != value) {
                    _data.IsGotOneIncorrectGpuShare = value;
                    OnPropertyChanged(nameof(IsGotOneIncorrectGpuShare));
                }
            }
        }

        public DateTime CreatedOn {
            get { return _data.CreatedOn; }
            set {
                if (_data.CreatedOn != value) {
                    _data.CreatedOn = value;
                    OnPropertyChanged(nameof(CreatedOn));
                }
            }
        }

        private GpuSpeedDataViewModels _gpuTableVm;
        public GpuSpeedDataViewModels GpuTableVm {
            get {
                return _gpuTableVm;
            }
            set {
                _gpuTableVm = value;
                OnPropertyChanged(nameof(GpuTableVm));
            }
        }

        public bool IsAutoRestartKernel {
            get { return _data.IsAutoRestartKernel; }
            set {
                if (_data.IsAutoRestartKernel != value) {
                    _data.IsAutoRestartKernel = value;
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public int AutoRestartKernelTimes {
            get { return _data.AutoRestartKernelTimes; }
            set {
                if (_data.AutoRestartKernelTimes != value) {
                    _data.AutoRestartKernelTimes = value;
                    OnPropertyChanged(nameof(AutoRestartKernelTimes));
                }
            }
        }

        public bool IsNoShareRestartKernel {
            get { return _data.IsNoShareRestartKernel; }
            set {
                if (_data.IsNoShareRestartKernel != value) {
                    _data.IsNoShareRestartKernel = value;
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public bool IsNoShareRestartComputer {
            get { return _data.IsNoShareRestartComputer; }
            set {
                if (_data.IsNoShareRestartComputer != value) {
                    _data.IsNoShareRestartComputer = value;
                    OnPropertyChanged(nameof(IsNoShareRestartComputer));
                }
            }
        }

        public int NoShareRestartComputerMinutes {
            get { return _data.NoShareRestartComputerMinutes; }
            set {
                if (_data.NoShareRestartComputerMinutes != value) {
                    _data.NoShareRestartComputerMinutes = value;
                    OnPropertyChanged(nameof(NoShareRestartComputerMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get { return _data.IsPeriodicRestartKernel; }
            set {
                if (_data.IsPeriodicRestartKernel != value) {
                    _data.IsPeriodicRestartKernel = value;
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get { return _data.IsPeriodicRestartComputer; }
            set {
                if (_data.IsPeriodicRestartComputer != value) {
                    _data.IsPeriodicRestartComputer = value;
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get { return _data.NoShareRestartKernelMinutes; }
            set {
                if (_data.NoShareRestartKernelMinutes != value) {
                    _data.NoShareRestartKernelMinutes = value;
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get { return _data.PeriodicRestartKernelHours; }
            set {
                if (_data.PeriodicRestartKernelHours != value) {
                    _data.PeriodicRestartKernelHours = value;
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get { return _data.PeriodicRestartComputerHours; }
            set {
                if (_data.PeriodicRestartComputerHours != value) {
                    _data.PeriodicRestartComputerHours = value;
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public int PeriodicRestartKernelMinutes {
            get { return _data.PeriodicRestartKernelMinutes; }
            set {
                if (_data.PeriodicRestartKernelMinutes != value) {
                    _data.PeriodicRestartKernelMinutes = value;
                    OnPropertyChanged(nameof(PeriodicRestartKernelMinutes));
                }
            }
        }

        public int PeriodicRestartComputerMinutes {
            get { return _data.PeriodicRestartComputerMinutes; }
            set {
                if (_data.PeriodicRestartComputerMinutes != value) {
                    _data.PeriodicRestartComputerMinutes = value;
                    OnPropertyChanged(nameof(PeriodicRestartComputerMinutes));
                }
            }
        }

        public string MainCoinPoolDelay {
            get { return _data.MainCoinPoolDelay; }
            set {
                if (_data.MainCoinPoolDelay != value) {
                    _data.MainCoinPoolDelay = value;
                    OnPropertyChanged(nameof(MainCoinPoolDelay));
                    OnPropertyChanged(nameof(MainCoinPoolDelayNumber));
                }
            }
        }

        public string DualCoinPoolDelay {
            get { return _data.DualCoinPoolDelay; }
            set {
                if (_data.DualCoinPoolDelay != value) {
                    _data.DualCoinPoolDelay = value;
                    OnPropertyChanged(nameof(DualCoinPoolDelay));
                    OnPropertyChanged(nameof(DualCoinPoolDelayNumber));
                }
            }
        }

        public int MainCoinPoolDelayNumber {
            get {
                return _data.MainCoinPoolDelayNumber;
            }
            set {
                if (_data.MainCoinPoolDelayNumber != value) {
                    _data.MainCoinPoolDelayNumber = value;
                    OnPropertyChanged(nameof(MainCoinPoolDelayNumber));
                }
            }
        }

        public int DualCoinPoolDelayNumber {
            get {
                return _data.DualCoinPoolDelayNumber;
            }
            set {
                if (_data.DualCoinPoolDelayNumber != value) {
                    _data.DualCoinPoolDelayNumber = value;
                    OnPropertyChanged(nameof(DualCoinPoolDelayNumber));
                }
            }
        }

        public int CpuPerformance {
            get { return _data.CpuPerformance; }
            set {
                if (_data.CpuPerformance != value) {
                    _data.CpuPerformance = value;
                    OnPropertyChanged(nameof(CpuPerformance));
                }
            }
        }

        public int CpuTemperature {
            get { return _data.CpuTemperature; }
            set {
                if (_data.CpuTemperature != value) {
                    _data.CpuTemperature = value;
                    OnPropertyChanged(nameof(CpuTemperature));
                }
            }
        }

        public bool IsAutoStopByCpu {
            get { return _data.IsAutoStopByCpu; }
            set {
                if (_data.IsAutoStopByCpu != value) {
                    _data.IsAutoStopByCpu = value;
                    OnPropertyChanged(nameof(IsAutoStopByCpu));
                }
            }
        }

        public int CpuGETemperatureSeconds {
            get { return _data.CpuGETemperatureSeconds; }
            set {
                if (_data.CpuGETemperatureSeconds != value) {
                    _data.CpuGETemperatureSeconds = value;
                    OnPropertyChanged(nameof(CpuGETemperatureSeconds));
                }
            }
        }

        public int CpuStopTemperature {
            get { return _data.CpuStopTemperature; }
            set {
                if (_data.CpuStopTemperature != value) {
                    _data.CpuStopTemperature = value;
                    OnPropertyChanged(nameof(CpuStopTemperature));
                }
            }
        }

        public bool IsAutoStartByCpu {
            get { return _data.IsAutoStartByCpu; }
            set {
                if (_data.IsAutoStartByCpu != value) {
                    _data.IsAutoStartByCpu = value;
                    OnPropertyChanged(nameof(IsAutoStartByCpu));
                }
            }
        }

        public int CpuLETemperatureSeconds {
            get { return _data.CpuLETemperatureSeconds; }
            set {
                if (_data.CpuLETemperatureSeconds != value) {
                    _data.CpuLETemperatureSeconds = value;
                    OnPropertyChanged(nameof(CpuLETemperatureSeconds));
                }
            }
        }

        public int CpuStartTemperature {
            get { return _data.CpuStartTemperature; }
            set {
                if (_data.CpuStartTemperature != value) {
                    _data.CpuStartTemperature = value;
                    OnPropertyChanged(nameof(CpuStartTemperature));
                }
            }
        }

        public int KernelSelfRestartCount {
            get {
                if (_data.KernelSelfRestartCount < 0) {
                    return 0;
                }
                return _data.KernelSelfRestartCount;
            }
            set {
                if (_data.KernelSelfRestartCount != value) {
                    _data.KernelSelfRestartCount = value;
                    OnPropertyChanged(nameof(KernelSelfRestartCount));
                }
            }
        }

        public DateTime LocalServerMessageTimestamp {
            get { return _data.LocalServerMessageTimestamp; }
            set {
                if (_data.LocalServerMessageTimestamp != value) {
                    _data.LocalServerMessageTimestamp = value;
                    OnPropertyChanged(nameof(LocalServerMessageTimestamp));
                }
            }
        }

        public bool IsRaiseHighCpuEvent {
            get { return _data.IsRaiseHighCpuEvent; }
            set {
                if (_data.IsRaiseHighCpuEvent != value) {
                    _data.IsRaiseHighCpuEvent = value;
                    OnPropertyChanged(nameof(IsRaiseHighCpuEvent));
                }
            }
        }

        public int HighCpuPercent {
            get { return _data.HighCpuPercent; }
            set {
                if (_data.HighCpuPercent != value) {
                    _data.HighCpuPercent = value;
                    OnPropertyChanged(nameof(HighCpuPercent));
                }
            }
        }

        public int HighCpuSeconds {
            get { return _data.HighCpuSeconds; }
            set {
                if (_data.HighCpuSeconds != value) {
                    _data.HighCpuSeconds = value;
                    OnPropertyChanged(nameof(HighCpuSeconds));
                }
            }
        }

        public bool IsOuterUserEnabled {
            get { return _data.IsOuterUserEnabled; }
            set {
                if (_data.IsOuterUserEnabled != value) {
                    _data.IsOuterUserEnabled = value;
                    OnPropertyChanged(nameof(IsOuterUserEnabled));
                }
            }
        }

        public string LoginName {
            get { return _data.LoginName; }
            set {
                if (_data.LoginName != value) {
                    _data.LoginName = value;
                    OnPropertyChanged(nameof(LoginName));
                }
            }
        }

        public string OuterUserId {
            get { return _data.OuterUserId; }
            set {
                if (_data.OuterUserId != value) {
                    _data.OuterUserId = value;
                    OnPropertyChanged(nameof(OuterUserId));
                }
            }
        }

        public string AESPassword {
            get { return _data.AESPassword; }
            set {
                if (_data.AESPassword != value) {
                    _data.AESPassword = value;
                    OnPropertyChanged(nameof(AESPassword));
                }
            }
        }

        public DateTime AESPasswordOn {
            get { return _data.AESPasswordOn; }
            set {
                if (_data.AESPasswordOn != value) {
                    _data.AESPasswordOn = value;
                    OnPropertyChanged(nameof(AESPasswordOn));
                }
            }
        }

        public bool IsAutoDisableWindowsFirewall {
            get { return _data.IsAutoDisableWindowsFirewall; }
            set {
                if (_data.IsAutoDisableWindowsFirewall != value) {
                    _data.IsAutoDisableWindowsFirewall = value;
                    OnPropertyChanged(nameof(IsAutoDisableWindowsFirewall));
                }
            }
        }

        public bool IsDisableUAC {
            get { return _data.IsDisableUAC; }
            set {
                if (_data.IsDisableUAC != value) {
                    _data.IsDisableUAC = value;
                    OnPropertyChanged(nameof(IsDisableUAC));
                }
            }
        }

        public bool IsDisableWAU {
            get { return _data.IsDisableWAU; }
            set {
                if (_data.IsDisableWAU != value) {
                    _data.IsDisableWAU = value;
                    OnPropertyChanged(nameof(IsDisableWAU));
                }
            }
        }

        public bool IsDisableAntiSpyware {
            get { return _data.IsDisableAntiSpyware; }
            set {
                if (_data.IsDisableAntiSpyware != value) {
                    _data.IsDisableAntiSpyware = value;
                    OnPropertyChanged(nameof(IsDisableAntiSpyware));
                }
            }
        }

        public DateTime MainCoinSpeedOn {
            get { return _data.MainCoinSpeedOn; }
            set {
                if (_data.MainCoinSpeedOn != value) {
                    _data.MainCoinSpeedOn = value;
                    OnPropertyChanged(nameof(MainCoinSpeedOn));
                }
            }
        }

        public DateTime DualCoinSpeedOn {
            get { return _data.DualCoinSpeedOn; }
            set {
                if (_data.DualCoinSpeedOn != value) {
                    _data.DualCoinSpeedOn = value;
                    OnPropertyChanged(nameof(DualCoinSpeedOn));
                }
            }
        }

        public void RefreshGpusForeground(uint minTemp, uint maxTemp) {
            if (GpuTableVm == null) {
                return;
            }
            foreach (var gpuSpeedData in GpuTableVm.List) {
                if (gpuSpeedData.Temperature >= maxTemp) {
                    gpuSpeedData.TemperatureForeground = WpfUtil.RedBrush;
                }
                else if (gpuSpeedData.Temperature < minTemp) {
                    gpuSpeedData.TemperatureForeground = WpfUtil.BlueBrush;
                }
                else {
                    gpuSpeedData.TemperatureForeground = WpfUtil.BlackBrush;
                }
            }
        }
        #endregion IClientData
    }
}
