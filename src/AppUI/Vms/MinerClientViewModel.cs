using NTMiner.Hashrate;
using NTMiner.MinerServer;
using NTMiner.Notifications;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase, IClientData {
        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand RemoteDesktop { get; private set; }
        public ICommand StartNTMiner { get; private set; }
        public ICommand RestartNTMiner { get; private set; }
        public ICommand CloseNTMiner { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private readonly ClientData _data;
        public MinerClientViewModel(ClientData clientData) {
            _data = clientData;
            this.RemoteDesktop = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.RemoteUserName) || string.IsNullOrEmpty(this.RemotePassword)) {
                    RemoteLogin.ShowEditWindow(new RemoteLoginViewModel(this.Id, this.MinerName, this.MinerIp, this) {
                        UserName = this.RemoteUserName,
                        Password = this.RemotePassword
                    });
                }
                else {
                    VirtualRoot.RemoteDesktop.OpenRemoteDesktop(this.MinerIp, this.RemoteUserName, this.RemotePassword, this.MinerName, onDisconnected: message => {
                        MinerClientsViewModel.Current.Manager.CreateMessage()
                                .Accent("#1751C3")
                                .Background("Red")
                                .HasBadge("Error")
                                .HasMessage(message)
                                .Dismiss()
                                .WithDelay(TimeSpan.FromSeconds(5))
                                .Queue();
                    });
                }
            });
            this.RestartWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定重启{this.MinerName}电脑吗？", title: "确认", onYes: () => {
                    // TODO:通过中控调用
                    NTMinerDaemonService.Instance.RestartWindowsAsync(this.MinerIp, null);
                }, icon: "Icon_Confirm");
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关机{this.MinerName}电脑吗？", title: "确认", onYes: () => {
                    // TODO:通过中控调用
                    NTMinerDaemonService.Instance.ShutdownWindowsAsync(this.MinerIp, null);
                }, icon: "Icon_Confirm");
            });
            this.StartNTMiner = new DelegateCommand(() => {
                // TODO:通过中控调用
                NTMinerDaemonService.Instance.OpenNTMinerAsync(this.MinerIp, this.WorkId, null);
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                MinerClientRestart.ShowWindow(this);
            });
            this.CloseNTMiner = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关闭{this.MinerName}挖矿客户端吗？关闭客户端软件，并非关闭电脑。", title: "确认", onYes: () => {
                    // TODO:通过中控调用
                    NTMinerDaemonService.Instance.CloseNTMinerAsync(this.MinerIp, null);
                }, icon: "Icon_Confirm");
            });
            this.StartMine = new DelegateCommand(() => {
                IsMining = true;
                MinerClientService.Instance.StartMineAsync(this.MinerIp, WorkId, response => {
                    if (!response.IsSuccess()) {
                        string message = $"{this.MinerIp} {response?.Description}";
                        Write.UserLine(message, ConsoleColor.Red);
                        UIThread.Execute(() => {
                            MinerClientsViewModel.Current.Manager.CreateMessage()
                                .Accent("#1751C3")
                                .Background("Red")
                                .HasBadge("Error")
                                .HasMessage(message)
                                .Dismiss()
                                .WithDelay(TimeSpan.FromSeconds(4))
                                .Queue();
                        });
                    }
                    else {
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            Refresh();
                        });
                    }
                });
            });
            this.StopMine = new DelegateCommand(() => {
                IsMining = false;
                MinerClientService.Instance.StopMineAsync(this.MinerIp, response => {
                    if (!response.IsSuccess()) {
                        string message = $"{this.MinerIp} {response?.Description}";
                        Write.UserLine(message, ConsoleColor.Red);
                        UIThread.Execute(() => {
                            MinerClientsViewModel.Current.Manager.CreateMessage()
                                .Accent("#1751C3")
                                .Background("Red")
                                .HasBadge("Error")
                                .HasMessage(message)
                                .Dismiss()
                                .WithDelay(TimeSpan.FromSeconds(4))
                                .Queue();
                        });
                    }
                    else {
                        TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                            Refresh();
                        });
                    }
                });
            });
        }

        private void Refresh() {
            Server.ControlCenterService.LoadClientAsync(this.GetId(), data => {
                if (data != null) {
                    this.Update(data);
                }
            });
        }

        #region IClientData

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _data.Id;
            set {
                if (_data.Id != value) {
                    _data.Id = value;
                    OnPropertyChanged(nameof(Id));
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

        public MineWorkViewModel SelectedMineWork {
            get {
                if (WorkId == Guid.Empty) {
                    return MineWorkViewModel.FreeMineWork;
                }
                MineWorkViewModel vm;
                if (MineWorkViewModels.Current.TryGetMineWorkVm(WorkId, out vm)) {
                    return vm;
                }
                return MineWorkViewModel.FreeMineWork;
            }
            set {
                if (WorkId != value.Id) {
                    WorkId = value.Id;
                    OnPropertyChanged(nameof(SelectedMineWork));
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(IClientData.WorkId), value.Id, null);
                }
            }
        }

        public string WorkName {
            get {
                if (this.WorkId == Guid.Empty) {
                    return "自由作业";
                }
                IMineWork mineWork;
                if (NTMinerRoot.Current.MineWorkSet.TryGetMineWork(this.WorkId, out mineWork)) {
                    return mineWork.Name;
                }
                return "未知作业";
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
                OnPropertyChanged(nameof(IsClientOnline));
                OnPropertyChanged(nameof(IsMining));
                OnPropertyChanged(nameof(LastActivedOnText));
            }
        }

        public bool IsClientOnline {
            get {
                return this.ModifiedOn.AddSeconds(121) >= DateTime.Now;
            }
        }

        public string ModifiedOnText {
            get {
                return this.ModifiedOn.ToString("HH:mm:ss");
            }
        }

        public string LastActivedOnText {
            get {
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

        public string BootTimeSpanText {
            get {
                TimeSpan time = DateTime.Now - BootOn;
                TimeSpan time1 = new TimeSpan(time.Hours, time.Minutes, time.Seconds);
                if (time.Days > 0) {
                    return $"{time.Days}天{time1.ToString()}";
                }
                else {
                    return time1.ToString();
                }
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
                if (!MineStartedOn.HasValue || MineStartedOn.Value <= Timestamp.UnixBaseTime) {
                    return string.Empty;
                }
                TimeSpan time = DateTime.Now - MineStartedOn.Value;
                TimeSpan time1 = new TimeSpan(time.Hours, time.Minutes, time.Seconds);
                if (time.Days > 0) {
                    return $"{time1.Days}天{time1.ToString()}";
                }
                else {
                    return time1.ToString();
                }
            }
        }

        public bool IsMining {
            get {
                if (!IsClientOnline) {
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
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(MinerName), value, response1 => {
                        if (response1.IsSuccess()) {
                            MinerClientService.Instance.SetMinerProfilePropertyAsync(this.MinerIp, nameof(MinerName), value, response2 => {
                                if (!response2.IsSuccess()) {
                                    _data.MinerName = old;
                                    Write.UserLine($"{this.MinerIp} {response2?.Description}", ConsoleColor.Red);
                                }
                                else {
                                    OnPropertyChanged(nameof(MinerName));
                                }
                            });
                        }
                        else {
                            _data.MinerName = old;
                        }
                    });
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

        private MinerGroupViewModel _selectedMinerGroup;
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
                    try {
                        Server.ControlCenterService.UpdateClientAsync(
                            this.Id, nameof(GroupId), value.Id, response => {
                                if (!response.IsSuccess()) {
                                    this.GroupId = old.Id;
                                    Write.UserLine($"{this.MinerIp} {response?.Description}", ConsoleColor.Red);
                                }
                                else {
                                    this.GroupId = value.Id;
                                    OnPropertyChanged(nameof(SelectedMinerGroup));
                                }
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

        public string RemoteUserName {
            get { return _data.RemoteUserName; }
            set {
                if (_data.RemoteUserName != value) {
                    var old = _data.RemoteUserName;
                    _data.RemoteUserName = value;
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(RemoteUserName), value, response => {
                        if (!response.IsSuccess()) {
                            _data.RemoteUserName = old;
                        }
                        else {
                            OnPropertyChanged(nameof(RemoteUserName));
                        }
                    });
                }
            }
        }

        public string RemotePassword {
            get { return _data.RemotePassword; }
            set {
                if (_data.RemotePassword != value) {
                    var old = _data.RemotePassword;
                    _data.RemotePassword = value;
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(RemotePassword), value, response => {
                        if (!response.IsSuccess()) {
                            _data.RemotePassword = old;
                        }
                        else {
                            OnPropertyChanged(nameof(RemotePassword));
                            OnPropertyChanged(nameof(RemotePasswordStar));
                        }
                    });
                }
            }
        }

        public string RemotePasswordStar {
            get {
                if (string.IsNullOrEmpty(this.RemotePassword)) {
                    return string.Empty;
                }
                return new string('●', this.RemotePassword.Length);
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

        public CoinViewModel MainCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.MainCoinCode, out coinVm);
                return coinVm;
            }
        }

        public double MainCoinSpeed {
            get => _data.MainCoinSpeed;
            set {
                _data.MainCoinSpeed = value;
                OnPropertyChanged(nameof(MainCoinSpeed));
                OnPropertyChanged(nameof(MainCoinSpeedText));
            }
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
            }
        }

        public int MainCoinRejectShare {
            get { return _data.MainCoinRejectShare; }
            set {
                _data.MainCoinRejectShare = value;
                OnPropertyChanged(nameof(MainCoinRejectShare));
                OnPropertyChanged(nameof(MainCoinRejectPercentText));
            }
        }

        public string MainCoinRejectPercentText {
            get {
                if (MainCoinTotalShare == 0) {
                    return "0%";
                }
                return (MainCoinRejectShare * 100.0 / MainCoinTotalShare).ToString("f1") + "%";
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
            }
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
            }
        }

        public int DualCoinRejectShare {
            get { return _data.DualCoinRejectShare; }
            set {
                _data.DualCoinRejectShare = value;
                OnPropertyChanged(nameof(DualCoinRejectShare));
                OnPropertyChanged(nameof(DualCoinRejectPercentText));
            }
        }

        public string DualCoinRejectPercentText {
            get {
                if (DualCoinTotalShare == 0) {
                    return "0%";
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

        public double OSVirtualMemory {
            get => _data.OSVirtualMemory;
            set {
                _data.OSVirtualMemory = value;
                OnPropertyChanged(nameof(OSVirtualMemory));
            }
        }

        public GpuType GpuType {
            get => _data.GpuType;
            set {
                _data.GpuType = value;
                OnPropertyChanged(nameof(GpuType));
            }
        }

        public string GpuDriver {
            get => _data.GpuDriver;
            set {
                _data.GpuDriver = value;
                OnPropertyChanged(nameof(GpuDriver));
            }
        }

        public GpuSpeedData[] GpuTable {
            get => _data.GpuTable;
            set {
                _data.GpuTable = value;
                OnPropertyChanged(nameof(GpuTable));
            }
        }
        #endregion IClientData
    }
}
