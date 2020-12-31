using NTMiner.Core;
using NTMiner.Core.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolViewModel : ViewModelBase, IPool, IEditableViewModel, ISortable {
        public static readonly PoolViewModel Empty = new PoolViewModel(Guid.Empty) {
            _coinId = Guid.Empty,
            _name = "无"
        };
        public static readonly PoolViewModel PleaseSelect = new PoolViewModel(Guid.Empty) {
            _coinId = Guid.Empty,
            _name = "不指定"
        };
        private Guid _id;
        private Guid _brandId;
        private string _name;
        private Guid _coinId;
        private string _server;
        private string _url;
        private string _website;
        private int _sortNumber;
        private string _notice;
        private string _userName;
        private string _passWord;
        private bool _isUserMode;
        private string _tutorialUrl;
        private bool _noPool1;
        private bool _notPool1;
        private string _minerNamePrefix;
        private string _minerNamePostfix;
        private CoinViewModel _coinVm;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public ICommand ViewPoolIncome { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public PoolViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public PoolViewModel(IPool data) : this(data.GetId()) {
            this._dataLevel = data.GetDataLevel();
            _brandId = data.BrandId;
            _name = data.Name;
            _coinId = data.CoinId;
            _server = data.Server;
            _url = data.Url;
            _website = data.Website;
            _sortNumber = data.SortNumber;
            _notice = data.Notice;
            _userName = data.UserName;
            _passWord = data.Password;
            _isUserMode = data.IsUserMode;
            _tutorialUrl = data.TutorialUrl;
            _noPool1 = data.NoPool1;
            _notPool1 = data.NotPool1;
            _minerNamePrefix = data.MinerNamePrefix;
            _minerNamePostfix = data.MinerNamePostfix;
        }

        public PoolViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerContext.Instance.ServerContext.PoolSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddPoolCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditPoolCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}矿池吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemovePoolCommand(this.Id));
                }));
            });
            this.SortUp = new DelegateCommand(() => {
                PoolViewModel upOne = AppRoot.PoolVms.AllPools.GetUpOne(SortNumber, a => a.CoinId == CoinId);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Pools));
                        coinVm.OnPropertyChanged(nameof(coinVm.OptionPools));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                PoolViewModel nextOne = AppRoot.PoolVms.AllPools.GetNextOne(SortNumber, a => a.CoinId == CoinId);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Pools));
                        coinVm.OnPropertyChanged(nameof(coinVm.OptionPools));
                    }
                }
            });
            this.ViewPoolIncome = new DelegateCommand(() => {
                bool openWebsite = string.IsNullOrEmpty(this.Url);
                var wallet = Wallet;
                if (!openWebsite) {
                    if (this.IsUserMode) {
                        var poolProfile = this.PoolProfileVm;
                        if (poolProfile == null || string.IsNullOrEmpty(poolProfile.UserName)) {
                            openWebsite = true;
                        }
                    }
                    else {
                        if (wallet == null || string.IsNullOrEmpty(wallet.Address)) {
                            openWebsite = true;
                        }
                    }
                }
                if (openWebsite) {
                    if (!string.IsNullOrEmpty(Website)) {
                        VirtualRoot.Execute(new UnTopmostCommand());
                        Process.Start(Website);
                    }
                    return;
                }
                string url = this.Url;
                if (this.IsUserMode) {
                    url = url.Replace("{userName}", this.PoolProfileVm.UserName);
                }
                else {
                    url = url.Replace("{wallet}", wallet.Address);
                }
                url = url.Replace("{worker}", NTMinerContext.Instance.MinerProfile.MinerName);
                VirtualRoot.Execute(new UnTopmostCommand());
                Process.Start(url);
            });
        }

        public bool IsNew {
            get {
                return !NTMinerContext.Instance.ServerContext.PoolSet.Contains(this.Id);
            }
        }

        public List<PoolKernelViewModel> PoolKernels {
            get {
                return AppRoot.PoolKernelVms.AllPoolKernels.Where(a => a.PoolId == this.Id).OrderBy(a => a.Kernel.Code + a.Kernel.Version).ToList();
            }
        }

        private DataLevel _dataLevel;
        public DataLevel GetDataLevel() {
            return _dataLevel;
        }

        public bool IsReadOnly {
            get {
                if (!DevMode.IsDevMode && this._dataLevel == DataLevel.Global) {
                    return true;
                }
                return false;
            }
        }

        public string DataLevelText {
            get {
                return this._dataLevel.GetDescription();
            }
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this._dataLevel = dataLevel;
        }

        public WalletViewModel Wallet {
            get {
                var coin = CoinVm;
                if (coin == null || coin == CoinViewModel.PleaseSelect) {
                    return null;
                }
                // 主币
                if (coin == MinerProfileViewModel.Instance.CoinVm) {
                    return coin.CoinProfile?.SelectedWallet;
                }
                // 辅币
                else if (coin == MinerProfileViewModel.Instance.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin) {
                    return coin.CoinProfile?.SelectedDualCoinWallet;
                }
                return null;
            }
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public Guid BrandId {
            get { return _brandId; }
            set {
                if (_brandId != value) {
                    _brandId = value;
                    OnPropertyChanged(nameof(BrandId));
                    OnPropertyChanged(nameof(BrandItem));
                }
            }
        }

        public SysDicItemViewModel BrandItem {
            get {
                if (this.BrandId == Guid.Empty) {
                    return SysDicItemViewModel.PleaseSelect;
                }
                if (AppRoot.SysDicItemVms.TryGetValue(this.BrandId, out SysDicItemViewModel item)) {
                    return item;
                }
                return SysDicItemViewModel.PleaseSelect;
            }
            set {
                if (value == null) {
                    value = SysDicItemViewModel.PleaseSelect;
                }
                this.BrandId = value.Id;
            }
        }

        public AppRoot.SysDicItemViewModels SysDicItemVms {
            get {
                return AppRoot.SysDicItemVms;
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this.Id == PleaseSelect.Id) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (AppRoot.PoolVms.AllPools.Any(a => a.Name == value && a.Id != this.Id && a.CoinId == this.CoinId)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public Guid CoinId {
            get {
                return _coinId;
            }
            set {
                if (_coinId != value) {
                    _coinId = value;
                    OnPropertyChanged(nameof(CoinId));
                    OnPropertyChanged(nameof(CoinVm));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (_coinVm == null || _coinVm.Id != this.CoinId) {
                    AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out _coinVm);
                    if (_coinVm == null) {
                        _coinVm = CoinViewModel.PleaseSelect;
                    }
                }
                return _coinVm;
            }
        }

        public string CoinCode {
            get {
                if (NTMinerContext.Instance.ServerContext.CoinSet.TryGetCoin(this.CoinId, out ICoin coin)) {
                    return coin.Code;
                }
                return string.Empty;
            }
        }

        public string Server {
            get => _server;
            set {
                if (_server != value) {
                    _server = value;
                    OnPropertyChanged(nameof(Server));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("矿池地址是必须的");
                    }
                    RefreshArgsAssembly("矿池的矿池地址发生了变更");
                }
            }
        }
        
        /// <summary>
        /// <see cref="IPool.Url"/>
        /// </summary>
        public string Url {
            get => _url;
            set {
                if (_url != value) {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        public string Website {
            get { return _website; }
            set {
                _website = value;
                OnPropertyChanged(nameof(Website));
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }

        public string Notice {
            get { return _notice; }
            set {
                if (_notice != value) {
                    _notice = value;
                    OnPropertyChanged(nameof(Notice));
                }
            }
        }

        public string TutorialUrl {
            get => _tutorialUrl;
            set {
                if (_tutorialUrl != value) {
                    _tutorialUrl = value;
                    OnPropertyChanged(nameof(TutorialUrl));
                }
            }
        }

        public bool IsUserMode {
            get { return _isUserMode; }
            set {
                if (_isUserMode != value) {
                    _isUserMode = value;
                    OnPropertyChanged(nameof(IsUserMode));
                    AppRoot.MinerProfileVm.OnPropertyChanged(nameof(AppRoot.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
                    RefreshArgsAssembly("矿池的是否是用户挖矿模式发生了变更");
                }
            }
        }

        private static ICoinProfile GetDualCoinProfile() {
            var workProfile = NTMinerContext.Instance.MinerProfile;
            var mainCoinProfile = workProfile.GetCoinProfile(workProfile.CoinId);
            var coinKernelProfile = workProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
            return workProfile.GetCoinProfile(coinKernelProfile.DualCoinId);
        }

        private void RefreshArgsAssembly(string reason) {
            var mainCoinProfile = NTMinerContext.Instance.MinerProfile.GetCoinProfile(NTMinerContext.Instance.MinerProfile.CoinId);
            if (mainCoinProfile.PoolId == this.Id || mainCoinProfile.PoolId1 == this.Id) {
                NTMinerContext.RefreshArgsAssembly.Invoke(reason);
            }
            else {
                var dualCoinProfile = GetDualCoinProfile();
                if (dualCoinProfile.PoolId == this.Id || dualCoinProfile.PoolId1 == this.Id) {
                    NTMinerContext.RefreshArgsAssembly.Invoke(reason);
                }
            }
        }

        public string UserName {
            get { return _userName; }
            set {
                if (_userName != value) {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string Password {
            get { return _passWord; }
            set {
                if (_passWord != value) {
                    _passWord = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public bool NoPool1 {
            get { return _noPool1; }
            set {
                _noPool1 = value;
                OnPropertyChanged(nameof(NoPool1));
                RefreshArgsAssembly("矿池的是否不能有备用矿池发生了变更");
            }
        }

        public bool NotPool1 {
            get { return _notPool1; }
            set {
                _notPool1 = value;
                OnPropertyChanged(nameof(NotPool1));
                RefreshArgsAssembly("矿池的是否不能作为备用矿池发生了变更");
            }
        }

        public string MinerNamePrefix {
            get => _minerNamePrefix;
            set {
                _minerNamePrefix = value;
                OnPropertyChanged(nameof(MinerNamePrefix));
                RefreshArgsAssembly("矿池上放置的矿机名前缀发生了变更");
            }
        }

        public string MinerNamePostfix {
            get => _minerNamePostfix;
            set {
                _minerNamePostfix = value;
                OnPropertyChanged(nameof(MinerNamePostfix));
                RefreshArgsAssembly("矿池上放置的矿机名后缀发生了变更");
            }
        }

        public PoolProfileViewModel PoolProfileVm {
            get {
                return AppRoot.PoolProfileVms.GetOrCreatePoolProfile(this.Id);
            }
        }
    }
}
