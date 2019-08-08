using NTMiner.Core;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase, IClientData {
        public static readonly SolidColorBrush Blue = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush DefaultForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5A));

        private double _incomeMainCoinPerDay;
        private double _incomeMainCoinUsdPerDay;
        private double _incomeMainCoinCnyPerDay;
        private double _incomeDualCoinPerDay;
        private double _incomeDualCoinUsdPerDay;
        private double _incomeDualCoinCnyPerDay;
        private MinerGroupViewModel _selectedMinerGroup;
        private SolidColorBrush _tempForeground;
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
        public ICommand OneKeyOverClock { get; private set; }
        public ICommand OneKeyUpgrade { get; private set; }

        private readonly ClientData _data;
        #region ctor
        public MinerClientViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public MinerClientViewModel(ClientData clientData) {
            _data = clientData;
            RefreshMainCoinIncome();
            RefreshDualCoinIncome();
            this.OneKeyOverClock = new DelegateCommand(() => {

            });
            this.OneKeyUpgrade = new DelegateCommand(() => {

            });
            this.Remove = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定删除该矿机吗？", title: "确认", onYes: () => {
                    Server.ControlCenterService.RemoveClientsAsync(new List<string> { this.Id }, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                        else {
                            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                        }
                    });
                }, icon: IconConst.IconConfirm);
            });
            this.Refresh = new DelegateCommand(() => {
                Server.ControlCenterService.RefreshClientsAsync(new List<string> { this.Id }, (response, e) => {
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
                if (string.IsNullOrEmpty(this.WindowsLoginName) || string.IsNullOrEmpty(this.WindowsPassword)) {
                    NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage("没有填写远程桌面用户名密码", 4);
                    return;
                }
                AppContext.RemoteDesktop?.Invoke(new RemoteDesktopInput(this.MinerIp, this.WindowsLoginName, this.WindowsPassword, this.MinerName, message => {
                    NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message, 4);
                }));
            });
            this.RestartWindows = new DelegateCommand(() => {
                this.ShowDialog(message: $"您确定重启{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.RestartWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }, icon: IconConst.IconConfirm);
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定关闭{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.ShutdownWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }, icon: IconConst.IconConfirm);
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定重启{this.MinerName}({this.MinerIp})挖矿客户端吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.RestartNTMinerAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                }, icon: IconConst.IconConfirm);
            });
            this.StartMine = new DelegateCommand(() => {
                IsMining = true;
                Server.MinerClientService.StartMineAsync(this, WorkId, (response, e) => {
                    if (!response.IsSuccess()) {
                        Write.UserFail($"{this.MinerIp} {response.ReadMessage(e)}");
                    }
                });
                Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(IsMining), IsMining, null);
            });
            this.StopMine = new DelegateCommand(() => {
                this.ShowDialog(message: $"{this.MinerName}({this.MinerIp})：确定停止挖矿吗？", title: "确认", onYes: () => {
                    IsMining = false;
                    Server.MinerClientService.StopMineAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail($"{this.MinerIp} {response.ReadMessage(e)}");
                        }
                    });
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(IsMining), IsMining, null);
                }, icon: IconConst.IconConfirm);
            });
        }
        #endregion

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
                _data.ClientId = value;
                OnPropertyChanged(nameof(ClientId));
            }
        }

        public bool IsAutoBoot {
            get { return _data.IsAutoBoot; }
            set {
                _data.IsAutoBoot = value;
                OnPropertyChanged(nameof(IsAutoBoot));
            }
        }

        public bool IsAutoStart {
            get { return _data.IsAutoStart; }
            set {
                _data.IsAutoStart = value;
                OnPropertyChanged(nameof(IsAutoStart));
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
                        Server.ControlCenterService.UpdateClientAsync(
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
                _data.BootOn = value;
                OnPropertyChanged(nameof(BootOn));
                OnPropertyChanged(nameof(BootTimeSpanText));
            }
        }

        private static string TimeSpanToString(TimeSpan timeSpan) {
            if (timeSpan.Days >= 1) {
                return $"{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟";
            }
            if (timeSpan.Hours > 0) {
                return $"{timeSpan.Hours}小时{timeSpan.Minutes}分钟";
            }
            if (timeSpan.Minutes > 2) {
                return $"{timeSpan.Minutes}分钟";
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

        private readonly bool _isInnerIp = Ip.Util.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
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
                _data.MineStartedOn = value;
                OnPropertyChanged(nameof(MineStartedOn));
                OnPropertyChanged(nameof(MineTimeSpanText));
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
                _data.ClientName = value;
                OnPropertyChanged(nameof(ClientName));
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
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(MinerName), value, (response, e) => {
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
                        Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(GroupId), value.Id, (response, exception) => {
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
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(WindowsLoginName), value, (response, exception) => {
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
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(WindowsPassword), value, (response, exception) => {
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
                _data.MainCoinTotalShare = value;
                OnPropertyChanged(nameof(MainCoinTotalShare));
                OnPropertyChanged(nameof(MainCoinRejectPercentText));
                OnPropertyChanged(nameof(MainCoinRejectPercent));
            }
        }

        public int MainCoinRejectShare {
            get { return _data.MainCoinRejectShare; }
            set {
                _data.MainCoinRejectShare = value;
                OnPropertyChanged(nameof(MainCoinRejectShare));
                OnPropertyChanged(nameof(MainCoinRejectPercentText));
                OnPropertyChanged(nameof(MainCoinRejectPercent));
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
                _incomeMainCoinPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinPerDayText));
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
                _incomeMainCoinUsdPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinUsdPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinUsdPerDayText));
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
                _incomeMainCoinCnyPerDay = value;
                OnPropertyChanged(nameof(IncomeMainCoinCnyPerDay));
                OnPropertyChanged(nameof(IncomeMainCoinCnyPerDayText));
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
                _incomeDualCoinPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinPerDayText));
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
                _incomeDualCoinUsdPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinUsdPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinUsdPerDayText));
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
                _incomeDualCoinCnyPerDay = value;
                OnPropertyChanged(nameof(IncomeDualCoinCnyPerDay));
                OnPropertyChanged(nameof(IncomeDualCoinCnyPerDayText));
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
                _data.DualCoinSpeed = value;
                OnPropertyChanged(nameof(DualCoinSpeed));
                OnPropertyChanged(nameof(DualCoinSpeedText));
                RefreshDualCoinIncome();
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
                _data.DualCoinTotalShare = value;
                OnPropertyChanged(nameof(DualCoinTotalShare));
                OnPropertyChanged(nameof(DualCoinRejectPercentText));
                OnPropertyChanged(nameof(DualCoinRejectPercent));
            }
        }

        public int DualCoinRejectShare {
            get { return _data.DualCoinRejectShare; }
            set {
                _data.DualCoinRejectShare = value;
                OnPropertyChanged(nameof(DualCoinRejectShare));
                OnPropertyChanged(nameof(DualCoinRejectPercentText));
                OnPropertyChanged(nameof(DualCoinRejectPercent));
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
                _data.OSName = value;
                OnPropertyChanged(nameof(OSName));
            }
        }

        public int OSVirtualMemoryMb {
            get => _data.OSVirtualMemoryMb;
            set {
                _data.OSVirtualMemoryMb = value;
                OnPropertyChanged(nameof(OSVirtualMemoryMb));
                OnPropertyChanged(nameof(OSVirtualMemoryGbText));
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
                _data.DiskSpace = value;
                OnPropertyChanged(nameof(DiskSpace));
            }
        }

        public GpuType GpuType {
            get => _data.GpuType;
            set {
                _data.GpuType = value;
                OnPropertyChanged(nameof(GpuType));
                OnPropertyChanged(nameof(IsNvidiaIconVisible));
                OnPropertyChanged(nameof(IsAMDIconVisible));
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
                _data.GpuDriver = value;
                OnPropertyChanged(nameof(GpuDriver));
            }
        }

        public string KernelCommandLine {
            get => _data.KernelCommandLine;
            set {
                _data.KernelCommandLine = value;
                OnPropertyChanged(nameof(KernelCommandLine));
            }
        }

        public uint TotalPower {
            get { return (uint)GpuTable.Sum(a => a.PowerUsage); }
        }

        public string TotalPowerText {
            get { return $"{GpuTable.Sum(a => a.PowerUsage).ToString("f0")}W"; }
        }

        public int MaxTemp {
            get {
                if (GpuTable == null || GpuTable.Length == 0) {
                    return 0;
                }
                return GpuTable.Max(a => a.Temperature);
            }
        }

        public string MaxTempText {
            get {
                if (GpuTable == null || GpuTable.Length == 0) {
                    return "0";
                }
                return GpuTable.Max(a => a.Temperature).ToString("f0") + "℃";
            }
        }

        public SolidColorBrush TempForeground {
            get => _tempForeground;
            set {
                _tempForeground = value;
                OnPropertyChanged(nameof(TempForeground));
            }
        }

        public SolidColorBrush MainCoinRejectPercentForeground {
            get => _mainCoinRejectPercentForeground;
            set {
                _mainCoinRejectPercentForeground = value;
                OnPropertyChanged(nameof(MainCoinRejectPercentForeground));
            }
        }

        public SolidColorBrush DualCoinRejectPercentForeground {
            get => _dualCoinRejectPercentForeground;
            set {
                _dualCoinRejectPercentForeground = value;
                OnPropertyChanged(nameof(DualCoinRejectPercentForeground));
            }
        }

        public GpuSpeedData[] GpuTable {
            get => _data.GpuTable;
            set {
                _data.GpuTable = value;
                OnPropertyChanged(nameof(GpuTable));
                OnPropertyChanged(nameof(TotalPower));
                OnPropertyChanged(nameof(TotalPowerText));
                OnPropertyChanged(nameof(MaxTemp));
                OnPropertyChanged(nameof(MaxTempText));
                OnPropertyChanged(nameof(GpuCount));
                this.GpuTableVm = new GpuSpeedDataViewModels(MainCoinCode, DualCoinCode, MainCoinSpeedText, DualCoinSpeedText, TotalPowerText, value);
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
                _data.IsAutoRestartKernel = value;
                OnPropertyChanged(nameof(IsAutoRestartKernel));
            }
        }

        public bool IsNoShareRestartKernel {
            get { return _data.IsNoShareRestartKernel; }
            set {
                _data.IsNoShareRestartKernel = value;
                OnPropertyChanged(nameof(IsNoShareRestartKernel));
            }
        }

        public bool IsNoShareRestartComputer {
            get { return _data.IsNoShareRestartComputer; }
            set {
                _data.IsNoShareRestartComputer = value;
                OnPropertyChanged(nameof(IsNoShareRestartComputer));
            }
        }

        public bool IsPeriodicRestartKernel {
            get { return _data.IsPeriodicRestartKernel; }
            set {
                _data.IsPeriodicRestartKernel = value;
                OnPropertyChanged(nameof(IsPeriodicRestartKernel));
            }
        }

        public bool IsPeriodicRestartComputer {
            get { return _data.IsPeriodicRestartComputer; }
            set {
                _data.IsPeriodicRestartComputer = value;
                OnPropertyChanged(nameof(IsPeriodicRestartComputer));
            }
        }

        public int NoShareRestartKernelMinutes {
            get { return _data.NoShareRestartKernelMinutes; }
            set {
                _data.NoShareRestartKernelMinutes = value;
                OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
            }
        }

        public int PeriodicRestartKernelHours {
            get { return _data.PeriodicRestartKernelHours; }
            set {
                _data.PeriodicRestartKernelHours = value;
                OnPropertyChanged(nameof(PeriodicRestartKernelHours));
            }
        }

        public int PeriodicRestartComputerHours {
            get { return _data.PeriodicRestartComputerHours; }
            set {
                _data.PeriodicRestartComputerHours = value;
                OnPropertyChanged(nameof(PeriodicRestartComputerHours));
            }
        }

        public void RefreshGpusForeground(uint minTemp, uint maxTemp) {
            if (GpuTableVm == null) {
                return;
            }
            foreach (var gpuSpeedData in GpuTableVm.List) {
                if (gpuSpeedData.Temperature >= maxTemp) {
                    gpuSpeedData.TemperatureForeground = Wpf.Util.RedBrush;
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
