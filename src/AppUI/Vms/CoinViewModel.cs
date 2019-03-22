using NTMiner.Core;
using NTMiner.Core.Profiles;
using NTMiner.MinerClient;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinViewModel : EntityViewModelBase<ICoin, Guid>, ICoin, IEditableViewModel {
        public static readonly CoinViewModel Empty = new CoinViewModel(Guid.Empty) {
            _algo = string.Empty,
            _code = string.Empty,
            _enName = string.Empty,
            _cnName = string.Empty,
            _id = Guid.Empty,
            _testWallet = string.Empty,
            _sortNumber = 0,
            _justAsDualCoin = false,
            _walletRegexPattern = string.Empty
        };
        public static readonly CoinViewModel PleaseSelect = new CoinViewModel(Guid.Empty) {
            _code = "不指定"
        };
        public static readonly CoinViewModel DualCoinEnabled = new CoinViewModel(Guid.Empty) {
            _code = "启用了双挖"
        };

        private string _code;
        private int _sortNumber;
        private string _algo;
        private string _testWallet;
        private string _enName;
        private string _cnName;
        private string _walletRegexPattern;
        private bool _justAsDualCoin;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand AddPool { get; private set; }
        public ICommand AddWallet { get; private set; }
        public ICommand AddCoinKernel { get; private set; }
        public ICommand ViewCoinInfo { get; private set; }
        public ICommand Save { get; private set; }

        public ICommand AddOverClockData { get; private set; }

        public ICommand ApplyOverClock { get; private set; }

        public ICommand OverClock { get; private set; }

        public Action CloseWindow { get; set; }

        public CoinViewModel() {
            if (!Design.IsInDesignMode) {
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
            this.GpuOverClockVms = new List<GpuProfileViewModel>();
            foreach (var gpu in GpuViewModels.Current) {
                IGpuProfile data = GpuProfileSet.Instance.GetGpuProfile(this.Id, gpu.Index);
                var vm = new GpuProfileViewModel(data, gpu);
                GpuOverClockVms.Add(vm);
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    GpuAllOverClockDataVm = vm;
                }
            }
            this.OverClock = new DelegateCommand<OverClockDataViewModel>((data) => {
                DialogWindow.ShowDialog(message: $"确定应用该超频设置吗？", title: "确认", onYes: () => {
                    if (IsOverClockGpuAll) {
                        GpuAllOverClockDataVm.Update(data);
                    }
                    else {
                        foreach (var item in GpuOverClockVms) {
                            if (item.Index == NTMinerRoot.GpuAllId) {
                                continue;
                            }
                            item.Update(data);
                        }
                    }
                    ApplyOverClock.Execute(null);
                }, icon: IconConst.IconConfirm);
            });
            this.AddOverClockData = new DelegateCommand(() => {
                new OverClockDataViewModel(Guid.NewGuid()) {
                    CoinId = this.Id
                }.Edit.Execute(FormType.Add);
            });
            this.ApplyOverClock = new DelegateCommand(() => {
                var list = GpuOverClockVms.ToArray();
                foreach (var item in list) {
                    VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(item));
                }
                VirtualRoot.Execute(new CoinOverClockCommand(this.Id));
                NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage("超频成功");
            });
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Current.CoinSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateCoinCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddCoinCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.ViewCoinInfo = new DelegateCommand(() => {
                Process.Start("https://www.feixiaohao.com/currencies/" + this.EnName + "/");
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                CoinEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Code}币种吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveCoinCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                CoinViewModel upOne = CoinViewModels.Current.AllCoins.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinCommand(this));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.MainCoins));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.AllCoins));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                CoinViewModel nextOne = CoinViewModels.Current.AllCoins.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinCommand(this));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.MainCoins));
                    CoinViewModels.Current.OnPropertyChanged(nameof(CoinViewModels.AllCoins));
                }
            });

            this.AddPool = new DelegateCommand(() => {
                int sortNumber = this.Pools.Count == 0 ? 1 : this.Pools.Max(a => a.SortNumber) + 1;
                new PoolViewModel(Guid.NewGuid()) {
                    CoinId = Id,
                    SortNumber = sortNumber
                }.Edit.Execute(FormType.Add);
            });
            this.AddWallet = new DelegateCommand(() => {
                int sortNumber = this.Wallets.Count == 0 ? 1 : this.Wallets.Max(a => a.SortNumber) + 1;
                new WalletViewModel(Guid.NewGuid()) {
                    CoinId = Id,
                    SortNumber = sortNumber
                }.Edit.Execute(FormType.Add);
            });
            this.AddCoinKernel = new DelegateCommand(() => {
                KernelSelect.ShowWindow(this);
            });
        }

        public bool IsOverClockEnabled {
            get { return GpuProfileSet.Instance.IsOverClockEnabled(this.Id); }
            set {
                GpuProfileSet.Instance.SetIsOverClockEnabled(this.Id, value);
                OnPropertyChanged(nameof(IsOverClockEnabled));
            }
        }

        public bool IsOverClockGpuAll {
            get { return GpuProfileSet.Instance.IsOverClockGpuAll(this.Id); }
            set {
                GpuProfileSet.Instance.SetIsOverClockGpuAll(this.Id, value);
                OnPropertyChanged(nameof(IsOverClockGpuAll));
            }
        }

        public GpuProfileViewModel GpuAllOverClockDataVm {
            get; private set;
        }

        public List<GpuProfileViewModel> GpuOverClockVms {
            get; private set;
        }

        public ShareViewModel ShareVm {
            get {
                return ShareViewModels.Current.GetOrCreate(this.Id);
            }
        }

        public bool IsSupported {
            get {
                if (this == PleaseSelect || VirtualRoot.IsControlCenter) {
                    return true;
                }
                foreach (var coinKernel in NTMinerRoot.Current.CoinKernelSet.Where(a => a.CoinId == this.Id)) {
                    if (coinKernel.SupportedGpu == SupportedGpu.Both) {
                        return true;
                    }
                    if (coinKernel.SupportedGpu == SupportedGpu.NVIDIA && NTMinerRoot.Current.GpuSet.GpuType == GpuType.NVIDIA) {
                        return true;
                    }
                    if (coinKernel.SupportedGpu == SupportedGpu.AMD && NTMinerRoot.Current.GpuSet.GpuType == GpuType.AMD) {
                        return true;
                    }
                }

                return false;
            }
        }

        public string Code {
            get => _code;
            set {
                if (_code != value) {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                    OnPropertyChanged(nameof(CodeAlgo));
                    if (this == Empty || this == PleaseSelect || this == DualCoinEnabled) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                    if (CoinViewModels.Current.TryGetCoinVm(value, out CoinViewModel coinVm) && coinVm.Id != this.Id) {
                        throw new ValidationException("重复的币种编码");
                    }
                }
            }
        }

        public string CodeAlgo {
            get {
                if (this.Id == Guid.Empty) {
                    return this.Code;
                }
                return $"{this.Code}-{this.Algo}";
            }
        }

        public string EnName {
            get => _enName;
            set {
                if (_enName != value) {
                    _enName = value;
                    OnPropertyChanged(nameof(EnName));
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string CnName {
            get => _cnName;
            set {
                if (_cnName != value) {
                    _cnName = value;
                    OnPropertyChanged(nameof(CnName));
                    OnPropertyChanged(nameof(FullName));
                }
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
                if (_algo != value) {
                    _algo = value;
                    OnPropertyChanged(nameof(Algo));
                    OnPropertyChanged(nameof(CodeAlgo));
                }
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
                if (_walletRegexPattern != value) {
                    _walletRegexPattern = value;
                    OnPropertyChanged(nameof(WalletRegexPattern));
                    OnPropertyChanged(nameof(TestWallet));
                }
            }
        }

        public bool JustAsDualCoin {
            get => _justAsDualCoin;
            set {
                if (_justAsDualCoin != value) {
                    _justAsDualCoin = value;
                    OnPropertyChanged(nameof(JustAsDualCoin));
                }
            }
        }

        public CoinProfileViewModel CoinProfile {
            get {
                return new CoinProfileViewModel(NTMinerRoot.Current.MinerProfile.GetCoinProfile(this.Id));
            }
        }

        public List<PoolViewModel> Pools {
            get {
                return NTMinerRoot.Current.PoolSet.Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).Select(a => new PoolViewModel(a)).ToList();
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

        private static readonly Dictionary<Guid, WalletViewModel> s_testWallets = new Dictionary<Guid, WalletViewModel>();
        public WalletViewModel TestWalletVm {
            get {
                if (string.IsNullOrEmpty(this.TestWallet)) {
                    return WalletViewModel.CreateEmptyWallet(this.Id);
                }
                if (!s_testWallets.ContainsKey(this.GetId())) {
                    s_testWallets.Add(this.GetId(), new WalletViewModel(this.GetId()) {
                        Address = this.TestWallet,
                        CoinId = this.GetId(),
                        Name = this.Code + "测试地址",
                        SortNumber = 0
                    });
                }
                return s_testWallets[this.GetId()];
            }
        }

        public List<OverClockDataViewModel> OverClockDatas {
            get {
                return OverClockDataViewModels.Current.Where(a => a.CoinId == this.Id).ToList();
            }
        }

        private IEnumerable<WalletViewModel> GetWallets() {
            if (!string.IsNullOrEmpty(TestWallet)) {
                yield return TestWalletVm;
            }
            foreach (var item in NTMinerRoot.Current.MinerProfile.GetWallets().Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).ToList()) {
                yield return new WalletViewModel(item);
            }
        }

        public List<WalletViewModel> Wallets {
            get {
                if (Design.IsInDesignMode) {
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
                List<CoinKernelViewModel> list = new List<CoinKernelViewModel>();
                var data = NTMinerRoot.Current.CoinKernelSet.Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).ToList();
                foreach (var item in data) {
                    IKernel kernel;
                    if (NTMinerRoot.Current.KernelSet.TryGetKernel(item.KernelId, out kernel) && kernel.PublishState == PublishStatus.Published) {
                        list.Add(new CoinKernelViewModel(item));
                    }
                }
                return list;
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
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
    }
}
