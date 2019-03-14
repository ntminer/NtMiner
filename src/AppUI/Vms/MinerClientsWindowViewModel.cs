using NTMiner.MinerServer;
using NTMiner.Notifications;
using NTMiner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using NTMiner.Views.Ucs;

namespace NTMiner.Vms {
    public class MinerClientsWindowViewModel : ViewModelBase {
        public static readonly MinerClientsWindowViewModel Current = new MinerClientsWindowViewModel();

        private ColumnsShowViewModel _columnsShow;
        private int _countDown;
        private readonly ObservableCollection<MinerClientViewModel> _minerClients = new ObservableCollection<MinerClientViewModel>();
        private MinerClientViewModel _currentMinerClient;
        private MinerClientViewModel[] _selectedMinerClients = new MinerClientViewModel[0];
        private int _minerClientPageIndex = 1;
        private int _minerClientPageSize = 20;
        private int _minerClientTotal;
        private EnumItem<MineStatus> _mineStatusEnumItem;
        private string _minerIp;
        private string _minerName;
        private string _version;
        private string _kernel;
        private string _wallet;
        private CoinViewModel _mainCoin;
        private CoinViewModel _dualCoin;
        private PoolViewModel _mainCoinPool;
        private PoolViewModel _dualCoinPool;
        private MineWorkViewModel _selectedMineWork;
        private MinerGroupViewModel _selectedMinerGroup;
        private INotificationMessageManager _manager;
        private int _miningCount;
        private uint _maxTemp = 80;
        private int _frozenColumnCount = 9;
        private uint _minTemp = 40;
        private int _rejectPercent = 10;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand RestartNTMiner { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        public ICommand PageUp { get; private set; }
        public ICommand PageDown { get; private set; }
        public ICommand PageFirst { get; private set; }
        public ICommand PageLast { get; private set; }
        public ICommand PageRefresh { get; private set; }
        public ICommand AddMinerClient { get; private set; }
        public ICommand RemoveMinerClients { get; private set; }
        public ICommand RefreshMinerClients { get; private set; }

        #region ctor
        private MinerClientsWindowViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            VirtualRoot.On<Per1SecondEvent>(
                "刷新倒计时秒表",
                LogEnum.None,
                action: message => {
                    if (this.CountDown > 0) {
                        this.CountDown = this.CountDown - 1;
                        if (this.MinerClients == null) {
                            return;
                        }
                        var minerClients = this.MinerClients.ToArray();
                        foreach (var item in minerClients) {
                            item.OnPropertyChanged(nameof(item.LastActivedOnText));
                        }
                    }
                });
            Guid columnsShowId = ColumnsShowData.PleaseSelectId;
            IAppSetting columnsShowAppSetting;
            if (NTMinerRoot.Current.AppSettingSet.TryGetAppSetting("ColumnsShowId", out columnsShowAppSetting) && columnsShowAppSetting.Value != null) {
                Guid guid;
                if (Guid.TryParse(columnsShowAppSetting.Value.ToString(), out guid)) {
                    columnsShowId = guid;
                }
            }
            this._columnsShow = this.ColumnsShows.List.FirstOrDefault(a => a.Id == columnsShowId);
            if (this._columnsShow == null) {
                this._columnsShow = this.ColumnsShows.List.FirstOrDefault();
            }
            this._mineStatusEnumItem = this.MineStatusEnumItems.FirstOrDefault(a => a.Value == MineStatus.All);
            this._mainCoin = CoinViewModel.PleaseSelect;
            this._dualCoin = CoinViewModel.PleaseSelect;
            this._selectedMineWork = MineWorkViewModel.PleaseSelect;
            this._selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
            this._mainCoinPool = _mainCoin.OptionPools.First();
            this._dualCoinPool = _dualCoin.OptionPools.First();
            this._wallet = string.Empty;
            this.AddMinerClient = new DelegateCommand(MinerClientAdd.ShowWindow);
            this.RemoveMinerClients = new DelegateCommand(() => {
                if (this.MinerClients == null) {
                    return;
                }
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    DialogWindow.ShowDialog(message: $"确定删除选中的矿工吗？", title: "确认", onYes: () => {
                        Server.ControlCenterService.RemoveClientsAsync(SelectedMinerClients.Select(a => a.Id).ToList(), (response, e) => {
                            if (!response.IsSuccess()) {
                                if (response != null) {
                                    Write.UserLine(response.Description, ConsoleColor.Red);
                                    Manager.ShowErrorMessage(response.Description);
                                }
                            }
                            else {
                                QueryMinerClients();
                            }
                        });
                    }, icon: "Icon_Confirm");
                }
            });
            this.RefreshMinerClients = new DelegateCommand(() => {
                IEnumerable<MinerClientViewModel> clients = SelectedMinerClients;
                if (!clients.Any()) {
                    clients = MinerClients;
                }
                Server.ControlCenterService.RefreshClientsAsync(clients.Select(a => a.Id).ToList(), (response, e) => {
                    if (!response.IsSuccess()) {
                        if (response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                            Manager.ShowErrorMessage(response.Description);
                        }
                    }
                    else {
                        foreach (var data in response.Data) {
                            var item = MinerClients.FirstOrDefault(a => a.Id == data.Id);
                            if (item != null) {
                                item.Update(data);
                            }
                        }
                    }
                });
            });
            this.RestartWindows = new DelegateCommand(() => {
                if (this.MinerClients == null) {
                    return;
                }
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    DialogWindow.ShowDialog(message: $"确定重启选中的电脑吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.RestartWindowsAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    if (response != null) {
                                        Write.UserLine(response.Description, ConsoleColor.Red);
                                        Manager.ShowErrorMessage(response.Description);
                                    }
                                }
                            });
                        }
                    }, icon: "Icon_Confirm");
                }
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                if (this.MinerClients == null) {
                    return;
                }
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    DialogWindow.ShowDialog(message: $"确定关闭选中的电脑吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.ShutdownWindowsAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    if (response != null) {
                                        Write.UserLine(response.Description, ConsoleColor.Red);
                                        Manager.ShowErrorMessage(response.Description);
                                    }
                                }
                            });
                        }
                    }, icon: "Icon_Confirm");
                }
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                if (this.MinerClients == null) {
                    return;
                }
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    DialogWindow.ShowDialog(message: $"确定重启选中的挖矿客户端吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.RestartNTMinerAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    if (response != null) {
                                        Write.UserLine(response.Description, ConsoleColor.Red);
                                        Manager.ShowErrorMessage(response.Description);
                                    }
                                }
                            });
                        }
                    }, icon: "Icon_Confirm");
                }
            });
            this.StartMine = new DelegateCommand(() => {
                if (this.MinerClients == null) {
                    return;
                }
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    foreach (var item in SelectedMinerClients) {
                        item.IsMining = true;
                        Server.MinerClientService.StartMineAsync(item, item.WorkId, (response, e) => {
                            if (!response.IsSuccess()) {
                                string message = $"{item.MinerIp} {response?.Description}";
                                Write.UserLine(message, ConsoleColor.Red);
                                Manager.ShowErrorMessage(message);
                            }
                        });
                        Server.ControlCenterService.UpdateClientAsync(item.Id, nameof(item.IsMining), item.IsMining, null);
                    }
                }
            });
            this.StopMine = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    DialogWindow.ShowDialog(message: $"确定停止挖矿选中的挖矿端吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            item.IsMining = false;
                            Server.MinerClientService.StopMineAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    string message = $"{item.MinerIp} {response?.Description}";
                                    Write.UserLine(message, ConsoleColor.Red);
                                    Manager.ShowErrorMessage(message);
                                }
                            });
                            Server.ControlCenterService.UpdateClientAsync(item.Id, nameof(item.IsMining), item.IsMining, null);
                        }
                    }, icon: "Icon_Confirm");
                }
            });
            this.PageUp = new DelegateCommand(() => {
                this.MinerClientPageIndex = this.MinerClientPageIndex - 1;
            });
            this.PageDown = new DelegateCommand(() => {
                this.MinerClientPageIndex = this.MinerClientPageIndex + 1;
            });
            this.PageFirst = new DelegateCommand(() => {
                this.MinerClientPageIndex = 1;
            });
            this.PageLast = new DelegateCommand(() => {
                this.MinerClientPageIndex = MinerClientPageCount;
            });
            this.PageRefresh = new DelegateCommand(QueryMinerClients);
            VirtualRoot.On<Per1SecondEvent>(
                "周期性挥动铲子表示在挖矿中",
                LogEnum.None,
                action: message => {
                    if (this.MinerClients == null) {
                        return;
                    }
                    // 周期性挥动铲子表示在挖矿中
                    var minerClients = this.MinerClients.ToArray();
                    foreach (var item in minerClients) {
                        if (item.IsMining) {
                            item.IsShovelEmpty = !item.IsShovelEmpty;
                        }
                    }
                });
        }
        #endregion

        public int FrozenColumnCount {
            get => _frozenColumnCount;
            set {
                if (value >= 2) {
                    _frozenColumnCount = value;
                    OnPropertyChanged(nameof(FrozenColumnCount));
                }
            }
        }

        public int RejectPercent {
            get => _rejectPercent;
            set {
                _rejectPercent = value;
                OnPropertyChanged(nameof(RejectPercent));
                RefreshRejectPercentForeground();
            }
        }

        private void RefreshRejectPercentForeground() {
            foreach (MinerClientViewModel item in MinerClients) {
                if (item.MainCoinRejectPercent >= this.RejectPercent) {
                    item.MainCoinRejectPercentForeground = MinerClientViewModel.Red;
                }
                else {
                    item.MainCoinRejectPercentForeground = MinerClientViewModel.DefaultForeground;
                }

                if (item.DualCoinRejectPercent >= this.RejectPercent) {
                    item.DualCoinRejectPercentForeground = MinerClientViewModel.Red;
                }
                else {
                    item.DualCoinRejectPercentForeground = MinerClientViewModel.DefaultForeground;
                }
            }
        }

        public uint MaxTemp {
            get => _maxTemp;
            set {
                if (value > this.MinTemp && value != _maxTemp) {
                    _maxTemp = value;
                    OnPropertyChanged(nameof(MaxTemp));
                    RefreshMaxTempForeground();
                }
            }
        }

        public uint MinTemp {
            get => _minTemp;
            set {
                if (value < this.MaxTemp && value != _minTemp) {
                    _minTemp = value;
                    OnPropertyChanged(nameof(MinTemp));
                    RefreshMaxTempForeground();
                }
            }
        }

        private void RefreshMaxTempForeground() {
            foreach (MinerClientViewModel item in MinerClients) {
                if (item.MaxTemp >= this.MaxTemp) {
                    item.TempForeground = MinerClientViewModel.Red;
                }
                else if (item.MaxTemp < this.MinTemp) {
                    item.TempForeground = MinerClientViewModel.Blue;
                }
                else {
                    item.TempForeground = MinerClientViewModel.DefaultForeground;
                }
                item.RefreshGpusForeground(this.MinTemp, this.MaxTemp);
            }
        }

        private void ShowNoRecordSelected() {
            Manager.CreateMessage()
                    .Error("没有选中记录")
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(2))
                    .Queue();
        }

        public INotificationMessageManager Manager {
            get {
                if (_manager == null) {
                    _manager = new NotificationMessageManager();
                    AppStatic.Managers.AddManager(_manager);
                }
                return _manager;
            }
        }

        public ColumnsShowViewModel ColumnsShow {
            get {
                return _columnsShow;
            }
            set {
                if (_columnsShow != value && value != null) {
                    _columnsShow = value;
                    OnPropertyChanged(nameof(ColumnsShow));
                    VirtualRoot.Execute(new ChangeAppSettingCommand(new AppSettingData {
                        Key = "ColumnsShowId",
                        Value = value.Id
                    }));
                }
            }
        }

        public ColumnsShowViewModels ColumnsShows {
            get {
                return ColumnsShowViewModels.Current;
            }
        }

        public int CountDown {
            get { return _countDown; }
            set {
                _countDown = value;
                OnPropertyChanged(nameof(CountDown));
            }
        }

        private static readonly List<int> SPageSizeItems = new List<int>() { 10, 20, 30, 40 };
        public List<int> PageSizeItems {
            get {
                return SPageSizeItems;
            }
        }

        public bool IsPageUpEnabled {
            get {
                if (this.MinerClientPageIndex <= 1) {
                    return false;
                }
                return true;
            }
        }

        public bool IsPageDownEnabled {
            get {
                if (this.MinerClientPageIndex >= this.MinerClientPageCount) {
                    return false;
                }
                return true;
            }
        }

        public int MinerClientPageIndex {
            get => _minerClientPageIndex;
            set {
                if (_minerClientPageIndex != value) {
                    _minerClientPageIndex = value;
                    OnPropertyChanged(nameof(MinerClientPageIndex));
                    QueryMinerClients();
                }
            }
        }

        public int MinerClientPageCount {
            get {
                return (int)Math.Ceiling((double)this.MinerClientTotal / this.MinerClientPageSize);
            }
        }

        public int MinerClientPageSize {
            get => _minerClientPageSize;
            set {
                if (_minerClientPageSize != value) {
                    _minerClientPageSize = value;
                    OnPropertyChanged(nameof(MinerClientPageSize));
                    this.MinerClientPageIndex = 1;
                }
            }
        }

        public int MinerClientTotal {
            get => _minerClientTotal;
            set {
                if (_minerClientTotal != value) {
                    _minerClientTotal = value;
                    OnPropertyChanged(nameof(MinerClientTotal));
                }
            }
        }

        public int MiningCount {
            get => _miningCount;
            set {
                _miningCount = value;
                OnPropertyChanged(nameof(MiningCount));
            }
        }

        public void QueryMinerClients() {
            Guid? groupId = null;
            if (SelectedMinerGroup != MinerGroupViewModel.PleaseSelect) {
                groupId = SelectedMinerGroup.Id;
            }
            Guid? workId = null;
            if (SelectedMineWork != MineWorkViewModel.PleaseSelect) {
                workId = SelectedMineWork.Id;
            }
            string mainCoin = string.Empty;
            string dualCoin = string.Empty;
            string mainCoinPool = string.Empty;
            string dualCoinPool = string.Empty;
            string wallet = string.Empty;
            if (workId == null || workId.Value == Guid.Empty) {
                if (this.MainCoin != CoinViewModel.PleaseSelect) {
                    mainCoin = this.MainCoin.Code;
                    if (this.MainCoinPool != null) {
                        mainCoinPool = this.MainCoinPool.Server;
                    }
                }
                if (this.DualCoin != CoinViewModel.PleaseSelect) {
                    dualCoin = this.DualCoin.Code;
                    if (this.DualCoin == CoinViewModel.DualCoinEnabled) {
                        dualCoin = "*";
                    }
                    if (this.DualCoinPool != null) {
                        dualCoinPool = this.DualCoinPool.Server;
                    }
                }
                if (!string.IsNullOrEmpty(Wallet)) {
                    wallet = this.Wallet;
                }
            }
            Server.ControlCenterService.QueryClientsAsync(
                this.MinerClientPageIndex,
                this.MinerClientPageSize,
                groupId,
                workId,
                this.MinerIp,
                this.MinerName,
                this.MineStatusEnumItem.Value,
                mainCoin,
                mainCoinPool,
                dualCoin,
                dualCoinPool,
                wallet,
                this.Version, this.Kernel, (response, exception) => {
                    this.CountDown = 10;
                    if (response != null) {
                        UIThread.Execute(() => {
                            if (response.Data.Count == 0) {
                                this.MinerClients.Clear();
                            }
                            else {
                                var toRemoves = this.MinerClients.Where(a => response.Data.All(b => b.Id != a.Id)).ToArray();
                                foreach (var item in toRemoves) {
                                    this.MinerClients.Remove(item);
                                }
                                foreach (var item in this.MinerClients) {
                                    ClientData data = response.Data.FirstOrDefault(a => a.Id == item.Id);
                                    if (data != null) {
                                        item.Update(data);
                                    }
                                }
                                var toAdds = response.Data.Where(a => this.MinerClients.All(b => b.Id != a.Id));
                                foreach (var item in toAdds) {
                                    this.MinerClients.Add(new MinerClientViewModel(item));
                                }
                            }
                            MiningCount = response.MiningCount;
                            RefreshPagingUi(response.Total);
                            // DataGrid没记录时显示无记录
                            OnPropertyChanged(nameof(MinerClients));
                            RefreshMaxTempForeground();
                            RefreshRejectPercentForeground();
                        });
                    }
                });
        }

        private void RefreshPagingUi(int total) {
            _minerClientTotal = total;
            OnPropertyChanged(nameof(MinerClientTotal));
            OnPropertyChanged(nameof(MinerClientPageCount));
            OnPropertyChanged(nameof(IsPageDownEnabled));
            OnPropertyChanged(nameof(IsPageUpEnabled));
            if (MinerClientTotal == 0) {
                _minerClientPageIndex = 0;
                OnPropertyChanged(nameof(MinerClientPageIndex));
            }
            else if (MinerClientPageIndex == 0) {
                _minerClientPageIndex = 1;
                OnPropertyChanged(nameof(MinerClientPageIndex));
            }
        }

        public ObservableCollection<MinerClientViewModel> MinerClients {
            get {
                return _minerClients;
            }
        }

        public MinerClientViewModel CurrentMinerClient {
            get { return _currentMinerClient; }
            set {
                _currentMinerClient = value;
                OnPropertyChanged(nameof(CurrentMinerClient));
            }
        }

        public MinerClientViewModel[] SelectedMinerClients {
            get { return _selectedMinerClients; }
            set {
                _selectedMinerClients = value;
                OnPropertyChanged(nameof(SelectedMinerClients));
            }
        }

        public CoinViewModels MineCoinVms {
            get {
                return CoinViewModels.Current;
            }
        }

        private IEnumerable<CoinViewModel> GetDualCoinVmItems() {
            yield return CoinViewModel.PleaseSelect;
            yield return CoinViewModel.DualCoinEnabled;
            foreach (var item in CoinViewModels.Current.AllCoins) {
                yield return item;
            }
        }
        public List<CoinViewModel> DualCoinVmItems {
            get {
                return GetDualCoinVmItems().ToList();
            }
        }

        public CoinViewModel MainCoin {
            get { return _mainCoin; }
            set {
                if (_mainCoin != value) {
                    _mainCoin = value;
                    OnPropertyChanged(nameof(MainCoin));
                    OnPropertyChanged(nameof(MainCoinPool));
                    this.MainCoinPool = PoolViewModel.PleaseSelect;
                    OnPropertyChanged(nameof(IsMainCoinSelected));
                    QueryMinerClients();
                }
            }
        }

        public bool IsMainCoinSelected {
            get {
                if (MainCoin == CoinViewModel.PleaseSelect) {
                    return false;
                }
                return true;
            }
        }

        public PoolViewModel MainCoinPool {
            get => _mainCoinPool;
            set {
                if (_mainCoinPool != value) {
                    _mainCoinPool = value;
                    OnPropertyChanged(nameof(MainCoinPool));
                    QueryMinerClients();
                }
            }
        }

        public string Wallet {
            get => _wallet;
            set {
                if (_wallet != value) {
                    _wallet = value;
                    OnPropertyChanged(nameof(Wallet));
                    QueryMinerClients();
                }
            }
        }

        public CoinViewModel DualCoin {
            get {
                return _dualCoin;
            }
            set {
                if (_dualCoin != value) {
                    _dualCoin = value;
                    OnPropertyChanged(nameof(DualCoin));
                    this.DualCoinPool = PoolViewModel.PleaseSelect;
                    this.Wallet = string.Empty;
                    OnPropertyChanged(nameof(IsDualCoinSelected));
                    QueryMinerClients();
                }
            }
        }

        public bool IsDualCoinSelected {
            get {
                if (DualCoin == CoinViewModel.PleaseSelect || DualCoin == CoinViewModel.DualCoinEnabled) {
                    return false;
                }
                return true;
            }
        }

        public PoolViewModel DualCoinPool {
            get => _dualCoinPool;
            set {
                if (_dualCoinPool != null) {
                    if (_dualCoinPool != value) {
                        _dualCoinPool = value;
                        OnPropertyChanged(nameof(DualCoinPool));
                        QueryMinerClients();
                    }
                }
            }
        }

        public string MinerIp {
            get => _minerIp;
            set {
                if (_minerIp != value) {
                    _minerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                    if (!string.IsNullOrEmpty(value)) {
                        IPAddress ip;
                        if (!IPAddress.TryParse(value, out ip)) {
                            throw new ValidationException("IP地址格式不正确");
                        }
                    }
                    QueryMinerClients();
                }
            }
        }
        public string MinerName {
            get => _minerName;
            set {
                if (_minerName != value) {
                    _minerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    QueryMinerClients();
                }
            }
        }

        public string Version {
            get => _version;
            set {
                if (_version != value) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    QueryMinerClients();
                }
            }
        }

        public string Kernel {
            get => _kernel;
            set {
                if (_kernel != value) {
                    _kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                    QueryMinerClients();
                }
            }
        }

        public MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Current;
            }
        }

        private IEnumerable<MinerGroupViewModel> GetMinerGroupVmItems() {
            yield return MinerGroupViewModel.PleaseSelect;
            foreach (var item in MinerGroupViewModels.Current.List) {
                yield return item;
            }
        }
        public List<MinerGroupViewModel> MinerGroupVmItems {
            get {
                return GetMinerGroupVmItems().ToList();
            }
        }

        public MineWorkViewModel SelectedMineWork {
            get => _selectedMineWork;
            set {
                _selectedMineWork = value;
                OnPropertyChanged(nameof(SelectedMineWork));
                OnPropertyChanged(nameof(IsMineWorkSelected));
                QueryMinerClients();
            }
        }

        public bool IsMineWorkSelected {
            get {
                if (SelectedMineWork != MineWorkViewModel.PleaseSelect && SelectedMineWork != MineWorkViewModel.FreeMineWork) {
                    return true;
                }
                return false;
            }
        }

        public MinerGroupViewModel SelectedMinerGroup {
            get => _selectedMinerGroup;
            set {
                _selectedMinerGroup = value;
                OnPropertyChanged(nameof(SelectedMinerGroup));
                QueryMinerClients();
            }
        }

        public IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return MineStatus.All.GetEnumItems();
            }
        }

        public EnumItem<MineStatus> MineStatusEnumItem {
            get => _mineStatusEnumItem;
            set {
                if (_mineStatusEnumItem != value) {
                    _mineStatusEnumItem = value;
                    OnPropertyChanged(nameof(MineStatusEnumItem));
                    QueryMinerClients();
                }
            }
        }
    }
}
