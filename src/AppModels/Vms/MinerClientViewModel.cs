using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase, IMinerData, ISpeedData, IEntity<string> {
        public static readonly SolidColorBrush Blue = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush DefaultForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5A));

        private double _incomeMainCoinPerDay;
        private double _incomeMainCoinUsdPerDay;
        private double _incomeMainCoinCnyPerDay;
        private double _incomeDualCoinPerDay;
        private double _incomeDualCoinUsdPerDay;
        private double _incomeDualCoinCnyPerDay;
        private MinerGroupViewModel _selectedMinerGroup;
        private SolidColorBrush _dualCoinRejectPercentForeground;
        private SolidColorBrush _mainCoinRejectPercentForeground;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand RemoteDesktop { get; private set; }
        // ReSharper disable once InconsistentNaming
        public ICommand RestartNTMiner { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand Remove { get; private set; }
        public ICommand Refresh { get; private set; }

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
            RefreshMainCoinIncome();
            RefreshDualCoinIncome();
            this.Remove = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除该矿机吗？", title: "确认", onYes: () => {
                    RpcRoot.Server.ClientService.RemoveClientsAsync(new List<string> { this.Id }, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                        else {
                            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                        }
                    });
                }));
            });
            this.Refresh = new DelegateCommand(() => {
                RpcRoot.Server.ClientService.RefreshClientsAsync(new List<string> { this.Id }, (response, e) => {
                    if (!response.IsSuccess()) {
                        Write.UserFail(response.ReadMessage(e));
                    }
                    else {
                        var data = response.Data.FirstOrDefault(a => a.Id == this.Id);
                        if (data != null) {
                            this.Update(data);
                        }
                    }
                });
            });
            this.RemoteDesktop = new DelegateCommand(() => {
                if (!SpeedData.TryGetFirstIp(this.LocalIp, out string ip)) {
                    if (SpeedData.TryGetFirstIp(this.MinerIp, out string minerIp) && Net.IpUtil.IsInnerIp(minerIp)) {
                        ip = minerIp;
                    }
                    else {
                        VirtualRoot.Out.ShowWarn("Ip地址不能为空", autoHideSeconds: 4);
                        return;
                    }
                }
                if (string.IsNullOrEmpty(this.WindowsLoginName)) {
                    VirtualRoot.Execute(new ShowRemoteDesktopLoginDialogCommand(new RemoteDesktopLoginViewModel {
                        Ip = ip,
                        OnOk = vm => {
                            this.WindowsLoginName = vm.LoginName;
                            this.WindowsPassword = vm.Password;
                            Rdp.RemoteDesktop?.Invoke(new RdpInput(ip, this.WindowsLoginName, this.WindowsPassword, this.MinerName, onDisconnected: message => {
                                VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
                            }));
                        }
                    }));
                }
                else {
                    Rdp.RemoteDesktop?.Invoke(new RdpInput(ip, this.WindowsLoginName, this.WindowsPassword, this.MinerName, onDisconnected: message => {
                        VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
                    }));
                }
            });
            this.RestartWindows = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定重启{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    RpcRoot.Server.MinerClientService.RestartWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }));
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定关闭{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    RpcRoot.Server.MinerClientService.ShutdownWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }));
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定重启{this.MinerName}({this.MinerIp})挖矿客户端吗？", title: "确认", onYes: () => {
                    RpcRoot.Server.MinerClientService.RestartNTMinerAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }));
            });
            this.StartMine = new DelegateCommand(() => {
                IsMining = true;
                RpcRoot.Server.MinerClientService.StartMineAsync(this, WorkId, (response, e) => {
                    if (!response.IsSuccess()) {
                        Write.UserFail($"{this.MinerIp} {response.ReadMessage(e)}");
                    }
                });
                RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(IsMining), IsMining, null);
            });
            this.StopMine = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"{this.MinerName}({this.MinerIp})：确定停止挖矿吗？", title: "确认", onYes: () => {
                    IsMining = false;
                    RpcRoot.Server.MinerClientService.StopMineAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail($"{this.MinerIp} {response.ReadMessage(e)}");
                        }
                    });
                    RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(IsMining), IsMining, null);
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
            ReflectionUpdate.Update(this, data);
        }

        public AppContext.MineWorkViewModels MineWorkVms {
            get { return AppContext.Instance.MineWorkVms; }
        }

        public AppContext.MinerGroupViewModels MinerGroupVms {
            get { return AppContext.Instance.MinerGroupVms; }
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
                    if (AppContext.Instance.MineWorkVms.TryGetMineWorkVm(WorkId, out _selectedMineWork)) {
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
                    try {
                        RpcRoot.Server.ClientService.UpdateClientAsync(
                            this.Id, nameof(WorkId), value.Id, (response, exception) => {
                                if (!response.IsSuccess()) {
                                    _selectedMineWork = old;
                                    this.WorkId = old.Id;
                                    Write.UserFail($"{this.MinerIp} {response.ReadMessage(exception)}");
                                }
                                OnPropertyChanged(nameof(SelectedMineWork));
                            });
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
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

        public DateTime ModifiedOn {
            get => _data.ModifiedOn;
            set {
                if (_data.ModifiedOn != value) {
                    _data.ModifiedOn = value;
                    OnPropertyChanged(nameof(ModifiedOn));
                }
                OnPropertyChanged(nameof(IsMining));
                OnPropertyChanged(nameof(LastActivedOnText));
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        public string LastActivedOnText {
            get {
                if (ModifiedOn <= Timestamp.UnixBaseTime) {
                    return string.Empty;
                }
                TimeSpan timeSpan = DateTime.Now - ModifiedOn;
                if (timeSpan.Days >= 1) {
                    return timeSpan.Days + " 天前";
                }
                if (timeSpan.Hours > 0) {
                    return timeSpan.Hours + " 小时前";
                }
                if (timeSpan.Minutes > 2) {
                    return timeSpan.Minutes + " 分钟前";
                }
                return (int)timeSpan.TotalSeconds + " 秒前";
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
                if (BootOn <= Timestamp.UnixBaseTime || !IsOnline) {
                    return string.Empty;
                }
                return TimeSpanToString(DateTime.Now - BootOn);
            }
        }

        private readonly bool _isInnerIp = Net.IpUtil.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
        public bool IsOnline {
            get {
                if (_isInnerIp) {
                    return this.ModifiedOn.AddSeconds(20) > DateTime.Now;
                }
                return this.ModifiedOn.AddSeconds(130) > DateTime.Now;
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
                    return string.Empty;
                }

                return TimeSpanToString(DateTime.Now - MineStartedOn.Value);
            }
        }

        public bool IsMining {
            get {
                if (this.ModifiedOn.AddSeconds(130) < DateTime.Now) {
                    return false;
                }
                return _data.IsMining;
            }
            set {
                if (_data.IsMining != value) {
                    _data.IsMining = value;
                    OnPropertyChanged(nameof(IsMining));
                }
            }
        }

        public string ClientName {
            get { return _data.ClientName; }
            set {
                if (_data.ClientName != value) {
                    _data.ClientName = value;
                    OnPropertyChanged(nameof(ClientName));
                }
            }
        }

        public void UpdateMinerName(string minerName) {
            _data.MinerName = minerName;
        }

        public string MinerName {
            get => _data.MinerName;
            set {
                if (_data.MinerName != value) {
                    var old = _data.MinerName;
                    _data.MinerName = value;
                    RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(MinerName), value, (response, e) => {
                        if (!response.IsSuccess()) {
                            _data.MinerName = old;
                            Write.UserFail($"{this.MinerIp} {response.ReadMessage(e)}");
                        }
                        OnPropertyChanged(nameof(MinerName));
                    });
                }
                OnPropertyChanged(nameof(MinerName));
            }
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
                    AppContext.Instance.MinerGroupVms.TryGetMineWorkVm(GroupId, out _selectedMinerGroup);
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
                    try {
                        RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(GroupId), value.Id, (response, exception) => {
                            if (!response.IsSuccess()) {
                                _selectedMinerGroup = old;
                                this.GroupId = old.Id;
                                Write.UserFail($"{this.MinerIp} {response.ReadMessage(exception)}");
                            }
                            OnPropertyChanged(nameof(SelectedMinerGroup));
                        });
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
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
                    RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(WindowsLoginName), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsLoginName = old;
                            Write.UserFail($"{this.MinerIp} {response.ReadMessage(exception)}");
                        }
                        OnPropertyChanged(nameof(WindowsLoginName));
                    });
                    OnPropertyChanged(nameof(WindowsLoginName));
                }
            }
        }

        [IgnoreReflectionSet]
        public string WindowsPassword {
            get {
                if (string.IsNullOrEmpty(_data.WindowsPassword)) {
                    return string.Empty;
                }
                return HashUtil.EncDecInOne(_data.WindowsPassword);
            }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    value = HashUtil.EncDecInOne(value);
                }
                if (_data.WindowsPassword != value) {
                    var old = _data.WindowsPassword;
                    _data.WindowsPassword = value;
                    RpcRoot.Server.ClientService.UpdateClientAsync(this.Id, nameof(WindowsPassword), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsPassword = old;
                            Write.UserFail($"{this.MinerIp} {response.ReadMessage(exception)}");
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
            IncomePerDay incomePerDay = NTMinerRoot.Instance.CalcConfigSet.GetIncomePerHashPerDay(this.MainCoinCode);
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
                    OnPropertyChanged(nameof(MainCoinRejectPercentText));
                    OnPropertyChanged(nameof(MainCoinRejectPercent));
                }
            }
        }

        public int MainCoinRejectShare {
            get { return _data.MainCoinRejectShare; }
            set {
                if (_data.MainCoinRejectShare != value) {
                    _data.MainCoinRejectShare = value;
                    OnPropertyChanged(nameof(MainCoinRejectShare));
                    OnPropertyChanged(nameof(MainCoinRejectPercentText));
                    OnPropertyChanged(nameof(MainCoinRejectPercent));
                }
            }
        }

        public double MainCoinRejectPercent {
            get {
                if (MainCoinTotalShare == 0) {
                    return 0;
                }
                return (MainCoinRejectShare * 100.0 / MainCoinTotalShare);
            }
        }

        public string MainCoinRejectPercentText {
            get {
                if (MainCoinTotalShare == 0) {
                    return string.Empty;
                }
                return (MainCoinRejectShare * 100.0 / MainCoinTotalShare).ToString("f1") + "%";
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
            IncomePerDay incomePerDay = NTMinerRoot.Instance.CalcConfigSet.GetIncomePerHashPerDay(this.DualCoinCode);
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
                    OnPropertyChanged(nameof(DualCoinRejectPercentText));
                    OnPropertyChanged(nameof(DualCoinRejectPercent));
                }
            }
        }

        public int DualCoinRejectShare {
            get { return _data.DualCoinRejectShare; }
            set {
                if (_data.DualCoinRejectShare != value) {
                    _data.DualCoinRejectShare = value;
                    OnPropertyChanged(nameof(DualCoinRejectShare));
                    OnPropertyChanged(nameof(DualCoinRejectPercentText));
                    OnPropertyChanged(nameof(DualCoinRejectPercent));
                }
            }
        }

        public double DualCoinRejectPercent {
            get {
                if (DualCoinTotalShare == 0) {
                    return 0;
                }

                return (DualCoinRejectShare * 100.0 / DualCoinTotalShare);
            }
        }

        public string DualCoinRejectPercentText {
            get {
                if (DualCoinTotalShare == 0) {
                    return string.Empty;
                }
                return (DualCoinRejectShare * 100.0 / DualCoinTotalShare).ToString("f1") + "%";
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
                }
            }
        }

        public string DualCoinPoolDelay {
            get { return _data.DualCoinPoolDelay; }
            set {
                if (_data.DualCoinPoolDelay != value) {
                    _data.DualCoinPoolDelay = value;
                    OnPropertyChanged(nameof(DualCoinPoolDelay));
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
            get { return _data.KernelSelfRestartCount; }
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

        public void RefreshGpusForeground(uint minTemp, uint maxTemp) {
            if (GpuTableVm == null) {
                return;
            }
            foreach (var gpuSpeedData in GpuTableVm.List) {
                if (gpuSpeedData.Temperature >= maxTemp) {
                    gpuSpeedData.TemperatureForeground = WpfUtil.RedBrush;
                }
                else if (gpuSpeedData.Temperature < minTemp) {
                    gpuSpeedData.TemperatureForeground = Blue;
                }
                else {
                    gpuSpeedData.TemperatureForeground = DefaultForeground;
                }
            }
        }
        #endregion IClientData
    }
}
