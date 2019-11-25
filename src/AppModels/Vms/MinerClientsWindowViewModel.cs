using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientsWindowViewModel : ViewModelBase {
        public static readonly MinerClientsWindowViewModel Instance = new MinerClientsWindowViewModel(isInDesignMode: false);

        private ColumnsShowViewModel _columnsShow;
        private int _countDown;
        private List<NTMinerFileData> _ntminerFileList;
        private List<MinerClientViewModel> _minerClients = new List<MinerClientViewModel>();
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
        private CoinViewModel _coinVm;
        private string _pool;
        private PoolViewModel _poolVm;
        private MineWorkViewModel _selectedMineWork;
        private MinerGroupViewModel _selectedMinerGroup;
        private int _miningCount;
        private uint _maxTemp = 80;
        private readonly List<int> _frozenColumns = new List<int> { 8, 7, 6, 5, 4, 3, 2 };
        private int _frozenColumnCount = 8;
        private uint _minTemp = 40;
        private int _rejectPercent = 10;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        // ReSharper disable once InconsistentNaming
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
        public ICommand OneKeyWork { get; private set; }
        public ICommand OneKeyGroup { get; private set; }
        public ICommand OneKeyOverClock { get; private set; }
        public ICommand OneKeyUpgrade { get; private set; }
        public ICommand EditMineWork { get; private set; }
        public ICommand OneKeyMinerNames { get; private set; }
        public ICommand OneKeyWindowsLoginName { get; private set; }
        public ICommand OneKeyWindowsLoginPassword { get; private set; }
        public ICommand OneKeySetting { get; private set; }

        #region ctor
        public MinerClientsWindowViewModel(bool isInDesignMode = true) {
            if (WpfUtil.IsInDesignMode || isInDesignMode) {
                return;
            }
            var appSettings = NTMinerRoot.Instance.ServerAppSettingSet;
            Guid columnsShowId = ColumnsShowData.PleaseSelect.Id;
            if (appSettings.TryGetAppSetting(NTKeyword.ColumnsShowIdAppSettingKey, out IAppSetting columnsShowAppSetting) && columnsShowAppSetting.Value != null) {
                if (Guid.TryParse(columnsShowAppSetting.Value.ToString(), out Guid guid)) {
                    columnsShowId = guid;
                }
            }
            if (appSettings.TryGetAppSetting(NTKeyword.FrozenColumnCountAppSettingKey, out IAppSetting frozenColumnCountAppSetting) && frozenColumnCountAppSetting.Value != null) {
                if (int.TryParse(frozenColumnCountAppSetting.Value.ToString(), out int frozenColumnCount)) {
                    _frozenColumnCount = frozenColumnCount;
                }
            }
            if (appSettings.TryGetAppSetting(NTKeyword.MaxTempAppSettingKey, out IAppSetting maxTempAppSetting) && maxTempAppSetting.Value != null) {
                if (uint.TryParse(maxTempAppSetting.Value.ToString(), out uint maxTemp)) {
                    _maxTemp = maxTemp;
                }
            }
            if (appSettings.TryGetAppSetting(NTKeyword.MinTempAppSettingKey, out IAppSetting minTempAppSetting) && minTempAppSetting.Value != null) {
                if (uint.TryParse(minTempAppSetting.Value.ToString(), out uint minTemp)) {
                    _minTemp = minTemp;
                }
            }
            if (appSettings.TryGetAppSetting(NTKeyword.RejectPercentAppSettingKey, out IAppSetting rejectPercentAppSetting) && rejectPercentAppSetting.Value != null) {
                if (int.TryParse(rejectPercentAppSetting.Value.ToString(), out int rejectPercent)) {
                    _rejectPercent = rejectPercent;
                }
            }
            this._columnsShow = this.ColumnsShows.List.FirstOrDefault(a => a.Id == columnsShowId);
            if (this._columnsShow == null) {
                this._columnsShow = this.ColumnsShows.List.FirstOrDefault();
            }
            this._mineStatusEnumItem = NTMinerRoot.MineStatusEnumItems.FirstOrDefault(a => a.Value == MineStatus.All);
            this._coinVm = CoinViewModel.PleaseSelect;
            this._selectedMineWork = MineWorkViewModel.PleaseSelect;
            this._selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
            this._pool = string.Empty;
            // 至少会有一个PleaseSelect所以可以First
            this._poolVm = _coinVm.OptionPools.First();
            this._wallet = string.Empty;
            this.OneKeySetting = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowMinerClientSettingCommand(new MinerClientSettingViewModel(this.SelectedMinerClients)));
            }, CanCommand);
            this.OneKeyMinerNames = new DelegateCommand(() => {
                if (this.SelectedMinerClients.Length == 1) {
                    var selectedMinerClient = this.SelectedMinerClients[0];
                    WpfUtil.ShowInputDialog("群控矿工名 注意：重新开始挖矿时生效", selectedMinerClient.MinerName, null, minerName => {
                        selectedMinerClient.MinerName = minerName;
                        VirtualRoot.Out.ShowSuccess("设置群控矿工名成功，重新开始挖矿时生效。");
                    });
                    return;
                }
                MinerNamesSeterViewModel vm = new MinerNamesSeterViewModel(
                    prefix: "miner",
                    suffix: "01",
                    namesByObjectId: this.SelectedMinerClients.Select(a => new Tuple<string, string>(a.Id, string.Empty)).ToList());
                VirtualRoot.Execute(new ShowMinerNamesSeterCommand(vm));
                if (vm.IsOk) {
                    this.CountDown = 10;
                    Server.ClientService.UpdateClientsAsync(nameof(MinerClientViewModel.MinerName), vm.NamesByObjectId.ToDictionary(a => a.Item1, a => (object)a.Item2), callback: (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
                        }
                        else {
                            foreach (var kv in vm.NamesByObjectId) {
                                var item = this.SelectedMinerClients.FirstOrDefault(a => a.Id == kv.Item1);
                                if (item != null) {
                                    item.UpdateMinerName(kv.Item2);
                                }
                            }
                            QueryMinerClients();
                        }
                    });
                }
            }, CanCommand);
            this.OneKeyWindowsLoginName = new DelegateCommand(() => {
                WpfUtil.ShowInputDialog("远程桌面用户名", string.Empty, null, loginName => {
                    foreach (var item in SelectedMinerClients) {
                        item.WindowsLoginName = loginName;
                    }
                    VirtualRoot.Out.ShowSuccess("设置远程桌面用户名成功，双击矿机可打开远程桌面。");
                });
            }, CanCommand);
            this.OneKeyWindowsLoginPassword = new DelegateCommand(() => {
                WpfUtil.ShowInputDialog("远程桌面密码", string.Empty, null, password => {
                    foreach (var item in SelectedMinerClients) {
                        item.WindowsPassword = password;
                    }
                    VirtualRoot.Out.ShowSuccess("设置远程桌面密码成功，双击矿机可打开远程桌面。");
                });
            }, CanCommand);
            this.EditMineWork = new DelegateCommand(() => {
                this.SelectedMinerClients[0].SelectedMineWork.Edit.Execute(null);
            }, () => OnlySelectedOne() && this.SelectedMinerClients[0].SelectedMineWork != null
                    && this.SelectedMinerClients[0].SelectedMineWork != MineWorkViewModel.PleaseSelect);
            this.OneKeyWork = new DelegateCommand<MineWorkViewModel>((work) => {
                foreach (var item in SelectedMinerClients) {
                    item.SelectedMineWork = work;
                }
            });
            this.OneKeyGroup = new DelegateCommand<MinerGroupViewModel>((group) => {
                foreach (var item in SelectedMinerClients) {
                    item.SelectedMinerGroup = group;
                }
            });
            this.OneKeyOverClock = new DelegateCommand(() => {
                if (this.SelectedMinerClients.Length == 1) {
                    VirtualRoot.Execute(new ShowGpuProfilesPageCommand(this));
                }
            }, OnlySelectedOne);
            this.OneKeyUpgrade = new DelegateCommand<NTMinerFileData>((ntminerFileData) => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: "确定升级到该版本吗？", title: "确认", onYes: () => {
                    foreach (var item in SelectedMinerClients) {
                        Server.MinerClientService.UpgradeNTMinerAsync(item, ntminerFileData.FileName, (response, e) => {
                            if (!response.IsSuccess()) {
                                Write.UserFail($"{item.MinerName} {item.MinerIp} {response.ReadMessage(e)}");
                            }
                        });
                    }
                }));
            }, (ntminerFileData) => this.SelectedMinerClients != null && this.SelectedMinerClients.Length != 0);
            this.AddMinerClient = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowMinerClientAddCommand());
            });
            this.RemoveMinerClients = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除选中的矿机吗？", title: "确认", onYes: () => {
                        this.CountDown = 10;
                        Server.ClientService.RemoveClientsAsync(SelectedMinerClients.Select(a => a.Id).ToList(), (response, e) => {
                            if (!response.IsSuccess()) {
                                Write.UserFail(response.ReadMessage(e));
                            }
                            else {
                                QueryMinerClients();
                            }
                        });
                    }));
                }
            }, CanCommand);
            this.RefreshMinerClients = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    Server.ClientService.RefreshClientsAsync(SelectedMinerClients.Select(a => a.Id).ToList(), (response, e) => {
                        if (!response.IsSuccess()) {
                            Write.UserFail(response.ReadMessage(e));
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
                }
            }, CanCommand);
            this.RestartWindows = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定重启选中的电脑吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.RestartWindowsAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    Write.UserFail(response.ReadMessage(e));
                                }
                            });
                        }
                    }));
                }
            }, CanCommand);
            this.ShutdownWindows = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定关闭选中的电脑吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.ShutdownWindowsAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    Write.UserFail(response.ReadMessage(e));
                                }
                            });
                        }
                    }));
                }
            }, CanCommand);
            this.RestartNTMiner = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定重启选中的挖矿客户端吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            Server.MinerClientService.RestartNTMinerAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    Write.UserFail(response.ReadMessage(e));
                                }
                            });
                        }
                    }));
                }
            }, CanCommand);
            this.StartMine = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    foreach (var item in SelectedMinerClients) {
                        item.IsMining = true;
                        Server.MinerClientService.StartMineAsync(item, item.WorkId, (response, e) => {
                            if (!response.IsSuccess()) {
                                Write.UserFail($"{item.MinerIp} {response.ReadMessage(e)}");
                            }
                        });
                        Server.ClientService.UpdateClientAsync(item.Id, nameof(item.IsMining), item.IsMining, null);
                    }
                }
            }, CanCommand);
            this.StopMine = new DelegateCommand(() => {
                if (SelectedMinerClients.Length == 0) {
                    ShowNoRecordSelected();
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定将选中的矿机停止挖矿吗？", title: "确认", onYes: () => {
                        foreach (var item in SelectedMinerClients) {
                            item.IsMining = false;
                            Server.MinerClientService.StopMineAsync(item, (response, e) => {
                                if (!response.IsSuccess()) {
                                    Write.UserFail($"{item.MinerIp} {response.ReadMessage(e)}");
                                }
                            });
                            Server.ClientService.UpdateClientAsync(item.Id, nameof(item.IsMining), item.IsMining, null);
                        }
                    }));
                }
            }, CanCommand);
            this.PageUp = new DelegateCommand(() => {
                this.MinerClientPageIndex -= 1;
            });
            this.PageDown = new DelegateCommand(() => {
                this.MinerClientPageIndex += 1;
            });
            this.PageFirst = new DelegateCommand(() => {
                this.MinerClientPageIndex = 1;
            });
            this.PageLast = new DelegateCommand(() => {
                this.MinerClientPageIndex = MinerClientPageCount;
            });
            this.PageRefresh = new DelegateCommand(QueryMinerClients);
        }
        #endregion

        private bool CanCommand() {
            return this.SelectedMinerClients != null && this.SelectedMinerClients.Length != 0;
        }

        private bool OnlySelectedOne() {
            return this.SelectedMinerClients != null
                    && this.SelectedMinerClients.Length == 1;
        }

        public List<NTMinerFileData> NTMinerFileList {
            get {
                return _ntminerFileList;
            }
            set {
                _ntminerFileList = value;
                OnPropertyChanged(nameof(NTMinerFileList));
            }
        }

        public int FrozenColumnCount {
            get => _frozenColumnCount;
            set {
                if (value >= 2) {
                    _frozenColumnCount = value;
                    OnPropertyChanged(nameof(FrozenColumnCount));
                }
            }
        }

        public List<int> FrozenColumns {
            get { return _frozenColumns; }
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
                    item.MainCoinRejectPercentForeground = WpfUtil.RedBrush;
                }
                else {
                    item.MainCoinRejectPercentForeground = MinerClientViewModel.DefaultForeground;
                }

                if (item.DualCoinRejectPercent >= this.RejectPercent) {
                    item.DualCoinRejectPercentForeground = WpfUtil.RedBrush;
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
                if (item.GpuTableVm == null) {
                    continue;
                }
                if (item.GpuTableVm.MaxTemp >= this.MaxTemp) {
                    item.GpuTableVm.TempForeground = WpfUtil.RedBrush;
                }
                else if (item.GpuTableVm.MaxTemp < this.MinTemp) {
                    item.GpuTableVm.TempForeground = MinerClientViewModel.Blue;
                }
                else {
                    item.GpuTableVm.TempForeground = MinerClientViewModel.DefaultForeground;
                }
                item.RefreshGpusForeground(this.MinTemp, this.MaxTemp);
            }
        }

        private void ShowNoRecordSelected() {
            VirtualRoot.Out.ShowError("没有选中记录", 2);
        }

        public ColumnsShowViewModel ColumnsShow {
            get {
                return _columnsShow;
            }
            set {
                if (_columnsShow != value && value != null) {
                    _columnsShow = value;
                    OnPropertyChanged(nameof(ColumnsShow));
                    VirtualRoot.Execute(new SetServerAppSettingCommand(new AppSettingData {
                        Key = NTKeyword.ColumnsShowIdAppSettingKey,
                        Value = value.Id
                    }));
                }
            }
        }

        public AppContext.ColumnsShowViewModels ColumnsShows {
            get {
                return AppContext.Instance.ColumnsShowVms;
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
                _minerClientPageIndex = value;
                OnPropertyChanged(nameof(MinerClientPageIndex));
                QueryMinerClients();
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
            if (SelectedMinerGroup == null) {
                _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                OnPropertyChanged(nameof(SelectedMinerGroup));
            }
            if (SelectedMinerGroup != MinerGroupViewModel.PleaseSelect) {
                groupId = SelectedMinerGroup.Id;
            }
            Guid? workId = null;
            if (SelectedMineWork == null) {
                _selectedMineWork = MineWorkViewModel.PleaseSelect;
                OnPropertyChanged(nameof(SelectedMineWork));
            }
            if (SelectedMineWork != MineWorkViewModel.PleaseSelect) {
                workId = SelectedMineWork.Id;
            }
            string coin = string.Empty;
            string wallet = string.Empty;
            if (workId == null || workId.Value == Guid.Empty) {
                if (this.CoinVm != CoinViewModel.PleaseSelect && this.CoinVm != null) {
                    coin = this.CoinVm.Code;
                }
                if (!string.IsNullOrEmpty(Wallet)) {
                    wallet = this.Wallet;
                }
            }
            Server.ClientService.QueryClientsAsync(
                this.MinerClientPageIndex,
                this.MinerClientPageSize,
                groupId,
                workId,
                this.MinerIp,
                this.MinerName,
                this.MineStatusEnumItem.Value,
                coin,
                this.Pool,
                wallet,
                this.Version, this.Kernel, (response, exception) => {
                    this.CountDown = 10;
                    if (response != null) {
                        UIThread.Execute(() => {
                            if (response.Data.Count == 0) {
                                _minerClients = new List<MinerClientViewModel>();
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
                                _minerClients = _minerClients.ToList();
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

        public List<MinerClientViewModel> MinerClients {
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

        public AppContext.CoinViewModels MineCoinVms {
            get {
                return AppContext.Instance.CoinVms;
            }
        }

        private IEnumerable<CoinViewModel> GetDualCoinVmItems() {
            yield return CoinViewModel.PleaseSelect;
            yield return CoinViewModel.DualCoinEnabled;
            foreach (var item in AppContext.Instance.CoinVms.AllCoins) {
                yield return item;
            }
        }
        public List<CoinViewModel> DualCoinVmItems {
            get {
                return GetDualCoinVmItems().ToList();
            }
        }

        public CoinViewModel CoinVm {
            get { return _coinVm; }
            set {
                if (_coinVm != value) {
                    _coinVm = value;
                    OnPropertyChanged(nameof(CoinVm));
                    this._pool = string.Empty;
                    this._poolVm = PoolViewModel.PleaseSelect;
                    OnPropertyChanged(nameof(PoolVm));
                    OnPropertyChanged(nameof(IsMainCoinSelected));
                    this.MinerClientPageIndex = 1;
                }
            }
        }

        public bool IsMainCoinSelected {
            get {
                if (CoinVm == CoinViewModel.PleaseSelect) {
                    return false;
                }
                return true;
            }
        }

        public string Pool {
            get { return _pool; }
            set {
                _pool = value;
                OnPropertyChanged(nameof(Pool));
                this.MinerClientPageIndex = 1;
            }
        }

        public PoolViewModel PoolVm {
            get => _poolVm;
            set {
                if (_poolVm != value) {
                    _poolVm = value;
                    if (value == null) {
                        Pool = string.Empty;
                    }
                    else {
                        Pool = value.Server;
                    }
                    OnPropertyChanged(nameof(PoolVm));
                }
            }
        }

        public string Wallet {
            get => _wallet;
            set {
                if (_wallet != value) {
                    _wallet = value;
                    OnPropertyChanged(nameof(Wallet));
                    this.MinerClientPageIndex = 1;
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
                        if (!IPAddress.TryParse(value, out IPAddress _)) {
                            throw new ValidationException("IP地址格式不正确");
                        }
                    }
                    this.MinerClientPageIndex = 1;
                }
            }
        }
        public string MinerName {
            get => _minerName;
            set {
                if (_minerName != value) {
                    _minerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    this.MinerClientPageIndex = 1;
                }
            }
        }

        public string Version {
            get => _version;
            set {
                if (_version != value) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    this.MinerClientPageIndex = 1;
                }
            }
        }

        public string Kernel {
            get => _kernel;
            set {
                if (_kernel != value) {
                    _kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                    this.MinerClientPageIndex = 1;
                }
            }
        }

        public AppContext.MineWorkViewModels MineWorkVms {
            get {
                return AppContext.Instance.MineWorkVms;
            }
        }

        public AppContext.MinerGroupViewModels MinerGroupVms {
            get {
                return AppContext.Instance.MinerGroupVms;
            }
        }

        public MineWorkViewModel SelectedMineWork {
            get => _selectedMineWork;
            set {
                _selectedMineWork = value;
                OnPropertyChanged(nameof(SelectedMineWork));
                OnPropertyChanged(nameof(IsMineWorkSelected));
                this.MinerClientPageIndex = 1;
            }
        }

        public bool IsMineWorkSelected {
            get {
                if (SelectedMineWork != MineWorkViewModel.PleaseSelect) {
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
                this.MinerClientPageIndex = 1;
            }
        }

        public EnumItem<MineStatus> MineStatusEnumItem {
            get => _mineStatusEnumItem;
            set {
                if (_mineStatusEnumItem != value) {
                    _mineStatusEnumItem = value;
                    OnPropertyChanged(nameof(MineStatusEnumItem));
                    this.MinerClientPageIndex = 1;
                }
            }
        }
    }
}
