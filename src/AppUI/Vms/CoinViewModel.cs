using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinViewModel : ViewModelBase, ICoin {
        public static readonly CoinViewModel Empty = new CoinViewModel(Guid.Empty) {
            _algo = string.Empty,
            _code = string.Empty,
            _enName = string.Empty,
            _cnName = string.Empty,
            _id = Guid.Empty,
            _testWallet = string.Empty,
            _sortNumber = 0,
            _justAsDualCoin = false,
            _isCurrentCoin = false,
            _walletRegexPattern = string.Empty
        };
        public static readonly CoinViewModel PleaseSelect = new CoinViewModel(Guid.Empty) {
            _code = "请选择"
        };
        public static readonly CoinViewModel DualCoinEnabled = new CoinViewModel(Guid.Empty) {
            _code = "启用了双挖"
        };

        private Guid _id;
        private string _code;
        private int _sortNumber;
        private string _algo;
        private string _testWallet;
        private string _enName;
        private string _cnName;
        private string _walletRegexPattern;
        private bool _justAsDualCoin;

        private bool _isCurrentCoin;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand AddPool { get; private set; }
        public ICommand AddWallet { get; private set; }
        public ICommand AddCoinKernel { get; private set; }
        public ICommand ViewCoinInfo { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public CoinViewModel() {
            if (!NTMinerRoot.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public CoinViewModel(ICoin data) : this(data.GetId()) {
            _code = data.Code;
            _sortNumber = data.SortNumber;
            _algo = data.Algo;
            _testWallet = data.TestWallet;
            _enName = data.EnName;
            _cnName = data.CnName;
            _walletRegexPattern = data.WalletRegexPattern;
            _justAsDualCoin = data.JustAsDualCoin;
        }

        public CoinViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.CoinSet.Contains(this.Id)) {
                    Global.Execute(new UpdateCoinCommand(this));
                }
                else {
                    Global.Execute(new AddCoinCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.ViewCoinInfo = new DelegateCommand(() => {
                Process.Start("https://www.feixiaohao.com/currencies/" + this.EnName + "/");
            });
            this.Edit = new DelegateCommand(() => {
                CoinEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Code}币种吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveCoinCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.SortUp = new DelegateCommand(() => {
                CoinViewModel upOne = CoinViewModels.Current.AllCoins.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateCoinCommand(upOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateCoinCommand(this));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.AllCoins));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                CoinViewModel nextOne = CoinViewModels.Current.AllCoins.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateCoinCommand(nextOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateCoinCommand(this));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.AllCoins));
                }
            });

            this.AddPool = new DelegateCommand(() => {
                int sortNumber = this.Pools.Count == 0 ? 1 : this.Pools.Max(a => a.SortNumber) + 1;
                new PoolViewModel(Guid.NewGuid()) {
                    CoinId = Id,
                    SortNumber = sortNumber
                }.Edit.Execute(null);
            });
            this.AddWallet = new DelegateCommand(() => {
                int sortNumber = this.Wallets.Count == 0 ? 1 : this.Wallets.Max(a => a.SortNumber) + 1;
                new WalletViewModel(Guid.NewGuid()) {
                    CoinId = Id,
                    SortNumber = sortNumber
                }.Edit.Execute(null);
            });
            this.AddCoinKernel = new DelegateCommand(() => {
                KernelSelect.ShowWindow(this);
            });
        }

        public ShareViewModel ShareVm {
            get {
                return ShareViewModels.Current.GetOrCreate(this.Id);
            }
        }

        private INTMinerRoot RootObj {
            get {
                return NTMinerRoot.Current;
            }
        }

        public bool IsCurrentCoin {
            get { return _isCurrentCoin; }
            set {
                _isCurrentCoin = value;
                OnPropertyChanged(nameof(IsCurrentCoin));
            }
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public bool IsSupported {
            get {
                return this.IsSupported();
            }
        }

        public string Code {
            get => _code;
            set {
                if (_code != value) {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                    if (this == Empty || this == PleaseSelect || this == DualCoinEnabled) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                    CoinViewModel coinVm;
                    if (CoinViewModels.Current.TryGetCoinVm(value, out coinVm) && coinVm.Id != this.Id) {
                        throw new ValidationException("重复的币种编码");
                    }
                }
            }
        }

        public string EnName {
            get => _enName;
            set {
                _enName = value;
                OnPropertyChanged(nameof(EnName));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string CnName {
            get => _cnName;
            set {
                _cnName = value;
                OnPropertyChanged(nameof(CnName));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string FullName {
            get {
                return $"{EnName}-{CnName}";
            }
        }

        public string Algo {
            get => _algo;
            set {
                _algo = value;
                OnPropertyChanged(nameof(Algo));
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                _sortNumber = value;
                OnPropertyChanged(nameof(SortNumber));
            }
        }

        public string TestWallet {
            get => _testWallet;
            set {
                if (_testWallet != value) {
                    _testWallet = value ?? string.Empty;
                    OnPropertyChanged(nameof(TestWallet));
                    if (!string.IsNullOrEmpty(WalletRegexPattern)) {
                        Regex regex = new Regex(WalletRegexPattern);
                        if (!regex.IsMatch(value ?? string.Empty)) {
                            throw new ValidationException("钱包地址格式不正确。");
                        }
                    }
                }
            }
        }

        public string WalletRegexPattern {
            get => _walletRegexPattern;
            set {
                _walletRegexPattern = value;
                OnPropertyChanged(nameof(WalletRegexPattern));
                OnPropertyChanged(nameof(TestWallet));
            }
        }

        public bool JustAsDualCoin {
            get => _justAsDualCoin;
            set {
                _justAsDualCoin = value;
                OnPropertyChanged(nameof(JustAsDualCoin));
            }
        }

        public CoinProfileViewModel CoinProfile {
            get {
                if (!NTMinerRoot.Current.CoinSet.Contains(this.Id)) {
                    return null;
                }
                return CoinProfileViewModels.Current.GetOrCreateCoinProfile(this.Id);
            }
        }

        public List<PoolViewModel> Pools {
            get {
                var list = PoolViewModels.Current.AllPools.Where(a => a.CoinId == this.Id).ToList();
                if (CoinProfile != null) {
                    foreach (var pool in list) {
                        pool.IsCurrentPool = false;
                    }
                    PoolViewModel poolVm = list.FirstOrDefault(a => a.Id == CoinProfile.PoolId);
                    if (poolVm != null) {
                        poolVm.IsCurrentPool = true;
                    }
                }
                return list.OrderBy(a => a.SortNumber).ToList();
            }
        }

        private IEnumerable<PoolViewModel> GetOptionPools() {
            if (this == PleaseSelect) {
                yield return PoolViewModel.PleaseSelect;
            }
            else {
                yield return PoolViewModel.PleaseSelect;
                foreach (var item in Pools) {
                    yield return item;
                }
            }
        }

        public List<PoolViewModel> OptionPools {
            get {
                return GetOptionPools().ToList();
            }
        }

        private static readonly Dictionary<Guid, WalletViewModel> _testWallets = new Dictionary<Guid, WalletViewModel>();
        public WalletViewModel TestWalletVm {
            get {
                if (!_testWallets.ContainsKey(this.GetId())) {
                    _testWallets.Add(this.GetId(), new WalletViewModel(this.GetId()) {
                        Address = this.TestWallet,
                        CoinId = this.GetId(),
                        Name = this.Code + "测试地址",
                        SortNumber = -100
                    });
                }
                return _testWallets[this.GetId()];
            }
        }

        private IEnumerable<WalletViewModel> GetWallets() {
            yield return TestWalletVm;
            foreach (var item in WalletViewModels.Current.WalletList.Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).ToList()) {
                yield return item;
            }
        }

        public List<WalletViewModel> Wallets {
            get {
                if (NTMinerRoot.IsInDesignMode) {
                    return new List<WalletViewModel>();
                }
                return GetWallets().ToList();
            }
        }

        private IEnumerable<WalletViewModel> GetWalletItems() {
            yield return WalletViewModel.PleaseSelect;
            foreach (var item in Wallets) {
                yield return item;
            }
        }
        public List<WalletViewModel> WalletItems {
            get {
                return GetWalletItems().ToList();
            }
        }

        public List<CoinKernelViewModel> CoinKernels {
            get {
                return CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.CoinId == this.Id && a.Kernel.PublishState == PublishStatus.Published).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public CoinKernelViewModel CoinKernel {
            get {
                CoinKernelViewModel coinKernel = CoinKernels.FirstOrDefault(a => a.Id == CoinProfile.CoinKernelId);
                if (coinKernel == null || !coinKernel.Kernel.IsSupported) {
                    coinKernel = CoinKernels.FirstOrDefault(a => a.Kernel.IsSupported);
                    if (coinKernel != null) {
                        CoinProfile.CoinKernelId = coinKernel.Id;
                    }
                }
                return coinKernel;
            }
            set {
                if (value != null && value.Id != Guid.Empty) {
                    CoinProfile.CoinKernelId = value.Id;
                    OnPropertyChanged(nameof(CoinKernel));
                    Global.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }
    }
}
