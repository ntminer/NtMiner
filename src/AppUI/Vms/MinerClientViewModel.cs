using NTMiner.Core;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Notifications;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiteDB;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase, IClientData {
        public static readonly SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush Blue = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush DefaultForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x5A));

        private bool _isChecked = false;
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
        public ICommand RemoteLogin { get; private set; }
        // ReSharper disable once InconsistentNaming
        public ICommand RestartNTMiner { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand Details { get; private set; }
        public ICommand Remove { get; private set; }

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
            this.Details = new DelegateCommand(() => {
                MinerClientUc.ShowWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"确定删除该矿工吗？", title: "确认", onYes: () => {
                    Server.ControlCenterService.RemoveClientsAsync(new List<Guid> { this.ClientId }, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response.Description);
                            }
                        }
                        else {
                            MinerClientsWindowViewModel.Current.Manager.ShowSuccessMessage("操作成功，等待刷新");
                        }
                    });
                }, icon: "Icon_Confirm");
            });
            this.RemoteLogin = new DelegateCommand(() => {
                WindowsLogin.ShowWindow(new WindowsLoginViewModel(this.ClientId, this.MinerName, this.MinerIp, this));
            });
            this.RemoteDesktop = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.WindowsLoginName) || string.IsNullOrEmpty(this.WindowsPassword)) {
                    WindowsLogin.ShowWindow(new WindowsLoginViewModel(this.ClientId, this.MinerName, this.MinerIp, this));
                }
                else {
                    AppHelper.RemoteDesktop?.Invoke(new RemoteDesktopInput(this.MinerIp, this.WindowsLoginName, this.WindowsPassword, this.MinerName, message => {
                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(message);
                    }));
                }
            });
            this.RestartWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定重启{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.RestartWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response.Description);
                            }
                        }
                    });
                }, icon: "Icon_Confirm");
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"确定关闭{this.MinerName}({this.MinerIp})电脑吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.ShutdownWindowsAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response.Description);
                            }
                        }
                    });
                }, icon: "Icon_Confirm");
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"确定重启{this.MinerName}({this.MinerIp})挖矿客户端吗？", title: "确认", onYes: () => {
                    Server.MinerClientService.RestartNTMinerAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response.Description);
                            }
                        }
                    });
                }, icon: "Icon_Confirm");
            });
            this.StartMine = new DelegateCommand(() => {
                IsMining = true;
                Server.MinerClientService.StartMineAsync(this, WorkId, (response, e) => {
                    if (!response.IsSuccess()) {
                        string message = $"{this.MinerIp} {response?.Description}";
                        Write.UserLine(message, ConsoleColor.Red);
                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response?.Description);
                    }
                });
                Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(IsMining), IsMining, null);
            });
            this.StopMine = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"确定停止挖矿{this.MinerName}({this.MinerIp})挖矿端吗？", title: "确认", onYes: () => {
                    IsMining = false;
                    Server.MinerClientService.StopMineAsync(this, (response, e) => {
                        if (!response.IsSuccess()) {
                            string message = $"{this.MinerIp} {response?.Description}";
                            Write.UserLine(message, ConsoleColor.Red);
                            MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(response?.Description);
                        }
                    });
                    Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(IsMining), IsMining, null);
                }, icon: "Icon_Confirm");
            });
        }
        #endregion

        public MineWorkViewModels MineWorkVms {
            get { return MineWorkViewModels.Current; }
        }

        public MinerGroupViewModels MinerGroupVms {
            get { return MinerGroupViewModels.Current; }
        }

        private bool _isShovelEmpty = true;
        [IgnoreReflectionSet]
        public bool IsShovelEmpty {
            get => _isShovelEmpty;
            set {
                if (_isShovelEmpty != value) {
                    _isShovelEmpty = value;
                    OnPropertyChanged(nameof(IsShovelEmpty));
                }
            }
        }

        #region IClientData

        [IgnoreReflectionSet]
        public bool IsChecked {
            get { return _isChecked; }
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

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
                    return MineWorkViewModel.FreeMineWork;
                }
                if (_selectedMineWork == null || _selectedMineWork.Id != WorkId) {
                    if (MineWorkViewModels.Current.TryGetMineWorkVm(WorkId, out _selectedMineWork)) {
                        return _selectedMineWork;
                    }
                }
                return _selectedMineWork;
            }
            set {
                if (_selectedMineWork != value) {
                    var old = _selectedMineWork;
                    this.WorkId = value.Id;
                    _selectedMineWork = value;
                    try {
                        Server.ControlCenterService.UpdateClientAsync(
                            this.ClientId, nameof(WorkId), value.Id, (response, exception) => {
                                if (!response.IsSuccess()) {
                                    _selectedMineWork = old;
                                    this.WorkId = old.Id;
                                    if (response != null) {
                                        Write.UserLine($"{this.MinerIp} {response.Description}", ConsoleColor.Red);
                                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response.Description}");
                                    }
                                    else {
                                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}切换作业失败，已撤销");
                                    }
                                }
                                OnPropertyChanged(nameof(SelectedMineWork));
                            });
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
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
                    OnPropertyChanged(nameof(ModifiedOnText));
                }
                OnPropertyChanged(nameof(IsMining));
                OnPropertyChanged(nameof(LastActivedOnText));
            }
        }

        public string ModifiedOnText {
            get {
                return this.ModifiedOn.ToString("HH:mm:ss");
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
                if (BootOn <= Timestamp.UnixBaseTime) {
                    return string.Empty;
                }
                return TimeSpanToString(DateTime.Now - BootOn);
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

        public string MinerName {
            get => _data.MinerName;
            set {
                if (_data.MinerName != value) {
                    var old = _data.MinerName;
                    _data.MinerName = value;
                    Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(MinerName), value, (response, e) => {
                        if (response.IsSuccess()) {
                            var request = new SetMinerNameRequest {
                                LoginName = SingleUser.LoginName,
                                MinerName = value
                            };
                            request.SignIt(SingleUser.GetRemotePassword(this.ClientId));
                            Client.NTMinerDaemonService.SetMinerNameAsync(this.MinerIp, request, (response2, exception) => {
                                if (!response2.IsSuccess()) {
                                    _data.MinerName = old;
                                    if (response2 != null) {
                                        Write.UserLine($"{this.MinerIp} {response2.Description}", ConsoleColor.Red);
                                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response2.Description}");
                                    }
                                    else {
                                        MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}更改矿工名失败失败，已撤销");
                                    }
                                }
                                OnPropertyChanged(nameof(MinerName));
                            });
                        }
                        else {
                            _data.MinerName = old;
                            if (response != null) {
                                Write.UserLine($"{this.MinerIp} {response.Description}", ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response.Description}");
                            }
                            else {
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}更改矿工名失败，已撤销");
                            }
                        }
                        OnPropertyChanged(nameof(MinerName));
                    });
                    OnPropertyChanged(nameof(MinerName));
                }
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
                    MinerGroupViewModels.Current.TryGetMineWorkVm(GroupId, out _selectedMinerGroup);
                    if (_selectedMinerGroup == null) {
                        _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                    }
                }
                return _selectedMinerGroup;
            }
            set {
                if (_selectedMinerGroup != value) {
                    var old = _selectedMinerGroup;
                    _selectedMinerGroup = value;
                    this.GroupId = value.Id;
                    try {
                        Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(GroupId), value.Id, (response, exception) => {
                            if (!response.IsSuccess()) {
                                _selectedMinerGroup = old;
                                this.GroupId = old.Id;
                                if (response != null) {
                                    Write.UserLine($"{this.MinerIp} {response.Description}", ConsoleColor.Red);
                                    MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response.Description}");
                                }
                                else {
                                    MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}更改分组失败，已撤销");
                                }
                            }
                            OnPropertyChanged(nameof(SelectedMinerGroup));
                        });
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
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
                    Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(WindowsLoginName), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsLoginName = old;
                            if (response != null) {
                                Write.UserLine($"{this.MinerIp} {response.Description}", ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response.Description}");
                            }
                            else {
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}更改登录名失败，已撤销");
                            }
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
                    Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(WindowsPassword), value, (response, exception) => {
                        if (!response.IsSuccess()) {
                            _data.WindowsPassword = old;
                            if (response != null) {
                                Write.UserLine($"{this.MinerIp} {response.Description}", ConsoleColor.Red);
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp} {response.Description}");
                            }
                            else {
                                MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage($"{this.MinerIp}更改登录密码失败，已撤销");
                            }
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
            IncomePerDay incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(this.MainCoinCode);
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

        public CoinViewModel DualCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.DualCoinCode, out coinVm);
                return coinVm;
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
            IncomePerDay incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(this.DualCoinCode);
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

        public uint MaxTemp {
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
                OnPropertyChanged(nameof(GpuTableTrs));
                OnPropertyChanged(nameof(TotalPower));
                OnPropertyChanged(nameof(TotalPowerText));
                OnPropertyChanged(nameof(MaxTemp));
                OnPropertyChanged(nameof(MaxTempText));
                OnPropertyChanged(nameof(GpuCount));
            }
        }

        public void RefreshGpusForeground(uint minTemp, uint maxTemp) {
            for (int i = 0; i < GpuTable.Length; i++) {
                GpuSpeedData gpuSpeedData = GpuTable[i];
                PropertyInfo propertyInfo = GpuRowData.ForegroundProperties[i];
                if (gpuSpeedData.Temperature >= maxTemp) {
                    propertyInfo.SetValue(GpuTableTempRow, Red, null);
                }
                else if (gpuSpeedData.Temperature < minTemp) {
                    propertyInfo.SetValue(GpuTableTempRow, Blue, null);
                }
                else {
                    propertyInfo.SetValue(GpuTableTempRow, DefaultForeground, null);
                }
            }
        }

        public GpuRowData GpuTableTempRow { get; set; } = new GpuRowData {
            RowHeader = "温度"
        };

        public GpuRowData GpuTableFanRow = new GpuRowData {
            RowHeader = "风扇"
        };

        public GpuRowData[] GpuTableTrs {
            get {
                List<GpuRowData> list = new List<GpuRowData> {
                    new GpuRowData {
                        RowHeader = $"{MainCoinCode} {MainCoinSpeedText}"
                    },
                    new GpuRowData {
                        RowHeader = $"{DualCoinCode} {DualCoinSpeedText}"
                    },
                    GpuTableTempRow,
                    GpuTableFanRow,
                    new GpuRowData {
                        RowHeader = $"功耗 {GpuTable.Sum(a=>a.PowerUsage).ToString("f0")}W"
                    }
                };
                for (int i = 0; i < GpuTable.Length; i++) {
                    GpuSpeedData gpuSpeedData = GpuTable[i];
                    PropertyInfo propertyInfo = GpuRowData.ValueProperties[i];
                    propertyInfo.SetValue(list[0], gpuSpeedData.MainCoinSpeed.ToUnitSpeedText(), null);
                    propertyInfo.SetValue(list[1], gpuSpeedData.DualCoinSpeed.ToUnitSpeedText(), null);
                    propertyInfo.SetValue(list[2], gpuSpeedData.Temperature.ToString("f0") + "℃", null);
                    propertyInfo.SetValue(list[3], gpuSpeedData.FanSpeed.ToString("f0") + "%", null);
                    propertyInfo.SetValue(list[4], gpuSpeedData.PowerUsage.ToString("f0") + "W", null);
                }
                if (!IsDualCoinEnabled) {
                    list.RemoveAt(1);
                }
                return list.ToArray();
            }
        }

        public class GpuRowData : ViewModelBase {
            public static readonly PropertyInfo[] ValueProperties = new PropertyInfo[12];
            public static readonly PropertyInfo[] ForegroundProperties = new PropertyInfo[12];

            static GpuRowData() {
                Type t = typeof(GpuRowData);
                for (int i = 0; i < 12; i++) {
                    ValueProperties[i] = t.GetProperty("Gpu" + i);
                    ForegroundProperties[i] = t.GetProperty("Gpu" + i + "Foreground");
                }
            }
            private SolidColorBrush _gpu0Foreground = DefaultForeground;
            private SolidColorBrush _gpu1Foreground = DefaultForeground;
            private SolidColorBrush _gpu2Foreground = DefaultForeground;
            private SolidColorBrush _gpu3Foreground = DefaultForeground;
            private SolidColorBrush _gpu4Foreground = DefaultForeground;
            private SolidColorBrush _gpu5Foreground = DefaultForeground;
            private SolidColorBrush _gpu6Foreground = DefaultForeground;
            private SolidColorBrush _gpu7Foreground = DefaultForeground;
            private SolidColorBrush _gpu8Foreground = DefaultForeground;
            private SolidColorBrush _gpu9Foreground = DefaultForeground;
            private SolidColorBrush _gpu10Foreground = DefaultForeground;
            private SolidColorBrush _gpu11Foreground = DefaultForeground;

            public SolidColorBrush Gpu0Foreground {
                get => _gpu0Foreground;
                set {
                    _gpu0Foreground = value;
                    OnPropertyChanged(nameof(Gpu0Foreground));
                }
            }

            public SolidColorBrush Gpu1Foreground {
                get => _gpu1Foreground;
                set {
                    _gpu1Foreground = value;
                    OnPropertyChanged(nameof(Gpu1Foreground));
                }
            }

            public SolidColorBrush Gpu2Foreground {
                get => _gpu2Foreground;
                set {
                    _gpu2Foreground = value;
                    OnPropertyChanged(nameof(Gpu2Foreground));
                }
            }

            public SolidColorBrush Gpu3Foreground {
                get => _gpu3Foreground;
                set {
                    _gpu3Foreground = value;
                    OnPropertyChanged(nameof(Gpu3Foreground));
                }
            }

            public SolidColorBrush Gpu4Foreground {
                get => _gpu4Foreground;
                set {
                    _gpu4Foreground = value;
                    OnPropertyChanged(nameof(Gpu4Foreground));
                }
            }

            public SolidColorBrush Gpu5Foreground {
                get => _gpu5Foreground;
                set {
                    _gpu5Foreground = value;
                    OnPropertyChanged(nameof(Gpu5Foreground));
                }
            }

            public SolidColorBrush Gpu6Foreground {
                get => _gpu6Foreground;
                set {
                    _gpu6Foreground = value;
                    OnPropertyChanged(nameof(Gpu6Foreground));
                }
            }

            public SolidColorBrush Gpu7Foreground {
                get => _gpu7Foreground;
                set {
                    _gpu7Foreground = value;
                    OnPropertyChanged(nameof(Gpu7Foreground));
                }
            }

            public SolidColorBrush Gpu8Foreground {
                get => _gpu8Foreground;
                set {
                    _gpu8Foreground = value;
                    OnPropertyChanged(nameof(Gpu8Foreground));
                }
            }

            public SolidColorBrush Gpu9Foreground {
                get => _gpu9Foreground;
                set {
                    _gpu9Foreground = value;
                    OnPropertyChanged(nameof(Gpu9Foreground));
                }
            }

            public SolidColorBrush Gpu10Foreground {
                get => _gpu10Foreground;
                set {
                    _gpu10Foreground = value;
                    OnPropertyChanged(nameof(Gpu10Foreground));
                }
            }

            public SolidColorBrush Gpu11Foreground {
                get => _gpu11Foreground;
                set {
                    _gpu11Foreground = value;
                    OnPropertyChanged(nameof(Gpu11Foreground));
                }
            }

            public string RowHeader { get; set; }
            public string Gpu0 { get; set; }
            public string Gpu1 { get; set; }
            public string Gpu2 { get; set; }
            public string Gpu3 { get; set; }
            public string Gpu4 { get; set; }
            public string Gpu5 { get; set; }
            public string Gpu6 { get; set; }
            public string Gpu7 { get; set; }
            public string Gpu8 { get; set; }
            public string Gpu9 { get; set; }
            public string Gpu10 { get; set; }
            public string Gpu11 { get; set; }
        }
        #endregion IClientData
    }
}
