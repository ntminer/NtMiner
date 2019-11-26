using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinViewModel : ViewModelBase, ICoin, IEditableViewModel {
        public static readonly CoinViewModel Empty = new CoinViewModel(Guid.Empty) {
            _algoId = Guid.Empty,
            _code = string.Empty,
            _enName = string.Empty,
            _cnName = string.Empty,
            _icon = string.Empty,
            _id = Guid.Empty,
            _testWallet = string.Empty,
            _justAsDualCoin = false,
            _walletRegexPattern = string.Empty,
            _notice = string.Empty,
            _tutorialUrl = string.Empty,
            _iconImageSource = string.Empty,
            _minGpuMemoryGb = 0,
            _isHot = false,
            _kernelBrand = string.Empty
        };
        public static readonly CoinViewModel PleaseSelect = new CoinViewModel(Guid.Empty) {
            _code = "不指定"
        };
        public static readonly CoinViewModel DualCoinEnabled = new CoinViewModel(Guid.Empty) {
            _code = "启用了双挖"
        };

        private Guid _id;
        private string _code;
        private Guid _algoId;
        private string _testWallet;
        private string _enName;
        private string _cnName;
        private string _icon;
        private string _walletRegexPattern;
        private bool _justAsDualCoin;
        private string _notice;
        private string _iconImageSource;
        private string _tutorialUrl;
        private bool _isHot;
        private string _kernelBrand;
        private double _minGpuMemoryGb;
        private List<GpuProfileViewModel> _gpuProfileVms;
        private readonly CoinIncomeViewModel _coinIncomeVm;

        public CoinIncomeViewModel CoinIncomeVm {
            get {
                return _coinIncomeVm;
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand AddPool { get; private set; }
        public ICommand AddWallet { get; private set; }
        public ICommand Save { get; private set; }
        public ICommand BrowseIcon { get; private set; }

        public ICommand AddOverClockData { get; private set; }

        public ICommand AddNTMinerWallet { get; private set; }

        public ICommand ApplyTemplateOverClock { get; private set; }

        public ICommand ApplyCustomOverClock { get; private set; }

        public ICommand RestoreOverClock { get; private set; }

        public ICommand FillOverClockForms { get; private set; }

        public ICommand FillOverClockForm { get; private set; }

        public Action CloseWindow { get; set; }

        public CoinViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public CoinViewModel(ICoin data) : this(data.GetId()) {
            _code = data.Code;
            _algoId = data.AlgoId;
            _testWallet = data.TestWallet;
            _enName = data.EnName;
            _cnName = data.CnName;
            _icon = data.Icon;
            _walletRegexPattern = data.WalletRegexPattern;
            _justAsDualCoin = data.JustAsDualCoin;
            _notice = data.Notice;
            _tutorialUrl = data.TutorialUrl;
            _isHot = data.IsHot;
            _kernelBrand = data.KernelBrand;
            _minGpuMemoryGb = data.MinGpuMemoryGb;
            string iconFileFullName = SpecialPath.GetIconFileFullName(data);
            if (!string.IsNullOrEmpty(iconFileFullName) && File.Exists(iconFileFullName)) {
                _iconImageSource = iconFileFullName;
            }
        }

        public CoinViewModel(CoinViewModel vm) : this((ICoin)vm) {
            _iconImageSource = vm.IconImageSource;
        }

        private void ApplyOverClock() {
            VirtualRoot.Execute(new CoinOverClockCommand(this.Id));
        }

        private void FillOverClock(OverClockDataViewModel data) {
            if (IsOverClockGpuAll) {
                GpuAllProfileVm.Update(data);
                VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(GpuAllProfileVm));
            }
            else {
                foreach (var item in GpuProfileVms) {
                    if (item.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    item.Update(data);
                    VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(item));
                }
            }
        }

        public CoinViewModel(Guid id) {
            _id = id;
            _coinIncomeVm = new CoinIncomeViewModel(this);
            this.BrowseIcon = new DelegateCommand(() => {
                if (!DevMode.IsDevMode) {
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "png (*.png)|*.png",
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    try {
                        string iconFileFullName = openFileDialog.FileName;
                        this.IconImageSource = new Uri(iconFileFullName, UriKind.Absolute).ToString();
                        string pngFileName = Path.GetFileName(iconFileFullName);
                        if (AppContext.Instance.CoinVms.AllCoins.Any(a => a.Icon == pngFileName && a.Id != this.Id)) {
                            throw new ValidationException("币种图标不能重名");
                        }
                        this.Icon = pngFileName;
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
            });
            this.ApplyTemplateOverClock = new DelegateCommand<OverClockDataViewModel>((data) => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: data.Tooltip, title: "确定应用该超频设置吗？", onYes: () => {
                    FillOverClock(data);
                    ApplyOverClock();
                }));
            });
            this.ApplyCustomOverClock = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定应用您的自定义超频吗？", title: "确认自定义超频", onYes: () => {
                    ApplyOverClock();
                }));
            });
            this.RestoreOverClock = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定恢复默认吗？", title: "确认", onYes: () => {
                    NTMinerRoot.Instance.GpuSet.OverClock.Restore();
                    this.IsOverClockEnabled = false;
                }));
            });
            this.FillOverClockForm = new DelegateCommand<OverClockDataViewModel>((data) => {
                FillOverClock(data);
            });
            this.FillOverClockForms = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: "确定将统一超频的数据一键填充到每张卡吗？", title: "一键填充表单", onYes: () => {
                    var data = GpuAllProfileVm;
                    foreach (var item in GpuProfileVms) {
                        if (item.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        item.Update((IOverClockInput)data);
                        VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(item));
                    }
                }));
            });
            this.AddOverClockData = new DelegateCommand(() => {
                new OverClockDataViewModel(Guid.NewGuid()) {
                    CoinId = this.Id
                }.Edit.Execute(FormType.Add);
            });
            this.AddNTMinerWallet = new DelegateCommand(() => {
                new NTMinerWalletViewModel(Guid.NewGuid()) {
                    CoinId = this.Id
                }.Edit.Execute(FormType.Add);
            });
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Instance.ServerContext.CoinSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateCoinCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddCoinCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new CoinEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Code}币种吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveCoinCommand(this.Id));
                }));
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
        }

        public void OnOverClockPropertiesChanges() {
            OnPropertyChanged(nameof(IsOverClockEnabled));
            OnPropertyChanged(nameof(IsOverClockGpuAll));
            _gpuAllProfileVm = null;
            _gpuProfileVms = null;
            OnPropertyChanged(nameof(GpuAllProfileVm));
            OnPropertyChanged(nameof(GpuProfileVms));
        }

        public bool IsOverClockEnabled {
            get { return NTMinerRoot.Instance.GpuProfileSet.IsOverClockEnabled(this.Id); }
            set {
                NTMinerRoot.Instance.GpuProfileSet.SetIsOverClockEnabled(this.Id, value);
                OnPropertyChanged(nameof(IsOverClockEnabled));
            }
        }

        public bool IsOverClockGpuAll {
            get { return NTMinerRoot.Instance.GpuProfileSet.IsOverClockGpuAll(this.Id); }
            set {
                NTMinerRoot.Instance.GpuProfileSet.SetIsOverClockGpuAll(this.Id, value);
                OnPropertyChanged(nameof(IsOverClockGpuAll));
            }
        }

        private GpuProfileViewModel _gpuAllProfileVm;
        public GpuProfileViewModel GpuAllProfileVm {
            get {
                if (_gpuAllProfileVm == null) {
                    _gpuAllProfileVm = AppContext.Instance.GpuProfileVms.GpuAllVm(this.Id);
                }
                return _gpuAllProfileVm;
            }
            set {
                _gpuAllProfileVm = value;
                OnPropertyChanged(nameof(GpuAllProfileVm));
            }
        }

        public List<GpuProfileViewModel> GpuProfileVms {
            get {
                if (_gpuProfileVms == null) {
                    _gpuProfileVms = AppContext.Instance.GpuProfileVms.List(this.Id);
                }
                return _gpuProfileVms;
            }
            set {
                _gpuProfileVms = value;
                OnPropertyChanged(nameof(GpuProfileVms));
            }
        }

        public ShareViewModel ShareVm {
            get {
                return AppContext.Instance.ShareVms.GetOrCreate(this.Id);
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

        public bool IsSupported {
            get {
                if (this == PleaseSelect || NTMinerRoot.Instance.GpuSet.GpuType == GpuType.Empty) {
                    return true;
                }
                foreach (var coinKernel in NTMinerRoot.Instance.ServerContext.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == this.Id)) {
                    if (coinKernel.SupportedGpu.IsSupportedGpu(NTMinerRoot.Instance.GpuSet.GpuType)) {
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
                    if (this == Empty || this == PleaseSelect || this == DualCoinEnabled) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                    if (AppContext.Instance.CoinVms.TryGetCoinVm(value, out CoinViewModel coinVm) && coinVm.Id != this.Id) {
                        throw new ValidationException("重复的币种编码");
                    }
                }
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

        public string Icon {
            get { return _icon; }
            set {
                if (_icon != value) {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                    RefreshIcon();
                }
            }
        }

        public void RefreshIcon() {
            string iconFileFullName = SpecialPath.GetIconFileFullName(this);
            // 如果磁盘上存在则不再下载，所以如果要更新币种图标则需重命名Icon文件
            if (string.IsNullOrEmpty(iconFileFullName)) {
                return;
            }
            if (File.Exists(iconFileFullName)) {
                return;
            }
            using (var webClient = VirtualRoot.CreateWebClient(10)) {
                webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                    if (!e.Cancelled && e.Error == null) {
                        VirtualRoot.RaiseEvent(new CoinIconDownloadedEvent(Guid.Empty, this));
                    }
                    else {
                        File.Delete(iconFileFullName);
                    }
                };
                webClient.DownloadFileAsync(new Uri($"{OfficialServer.MinerJsonBucket}coin_icons/{this.Icon}"), iconFileFullName);
            }
        }

        public string IconImageSource {
            get => _iconImageSource;
            set {
                _iconImageSource = value;
                OnPropertyChanged(nameof(IconImageSource));
            }
        }

        public Guid AlgoId {
            get => _algoId;
            set {
                if (_algoId != value) {
                    _algoId = value;
                    OnPropertyChanged(nameof(AlgoId));
                    OnPropertyChanged(nameof(AlgoItem));
                }
            }
        }

        public string Algo {
            get {
                return AlgoItem.Value;
            }
        }

        public SysDicItemViewModel AlgoItem {
            get {
                if (this.AlgoId == Guid.Empty) {
                    return SysDicItemViewModel.PleaseSelect;
                }
                if (AppContext.Instance.SysDicItemVms.TryGetValue(this.AlgoId, out SysDicItemViewModel item)) {
                    return item;
                }
                return SysDicItemViewModel.PleaseSelect;
            }
            set {
                if (value == null) {
                    value = SysDicItemViewModel.PleaseSelect;
                }
                this.AlgoId = value.Id;
            }
        }

        public AppContext.SysDicItemViewModels SysDicItemVms {
            get {
                return AppContext.Instance.SysDicItemVms;
            }
        }

        public string TestWallet {
            get => _testWallet;
            set {
                if (_testWallet != value) {
                    _testWallet = value ?? string.Empty;
                    OnPropertyChanged(nameof(TestWallet));
                    if (!string.IsNullOrEmpty(WalletRegexPattern)) {
                        Regex regex = VirtualRoot.GetRegex(WalletRegexPattern);
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
                if (value != _tutorialUrl) {
                    _tutorialUrl = value;
                    OnPropertyChanged(nameof(TutorialUrl));
                }
            }
        }

        public bool IsHot {
            get { return _isHot; }
            set {
                _isHot = value;
                OnPropertyChanged(nameof(IsHot));
            }
        }

        public string KernelBrand {
            get { return _kernelBrand; }
            set {
                if (_kernelBrand != value) {
                    _kernelBrand = value;
                    OnPropertyChanged(nameof(KernelBrand));
                    if (string.IsNullOrEmpty(value)) {
                        _kernelBrandDic.Clear();
                    }
                }
            }
        }

        public double MinGpuMemoryGb {
            get { return _minGpuMemoryGb; }
            set {
                if (_minGpuMemoryGb != value) {
                    _minGpuMemoryGb = value;
                    OnPropertyChanged(nameof(MinGpuMemoryGb));
                    OnPropertyChanged(nameof(MinGpuMemoryGbText));
                }
            }
        }

        public string MinGpuMemoryGbText {
            get {
                if (MinGpuMemoryGb == 0) {
                    return "-";
                }
                return MinGpuMemoryGb.ToString() + " Gb";
            }
        }

        private string _oldKernelBrand;
        private readonly Dictionary<GpuType, Guid> _kernelBrandDic = new Dictionary<GpuType, Guid>();
        private Dictionary<GpuType, Guid> KernelBrandDic {
            get {
                if (string.IsNullOrEmpty(this.KernelBrand)) {
                    return _kernelBrandDic;
                }
                if (_oldKernelBrand == this.KernelBrand) {
                    return _kernelBrandDic;
                }
                _oldKernelBrand = this.KernelBrand;
                if (_kernelBrandDic.ContainsKey(GpuType.AMD)) {
                    _kernelBrandDic[GpuType.AMD] = this.GetKernelBrandId(GpuType.AMD);
                }
                else {
                    _kernelBrandDic.Add(GpuType.AMD, this.GetKernelBrandId(GpuType.AMD));
                }
                if (_kernelBrandDic.ContainsKey(GpuType.NVIDIA)) {
                    _kernelBrandDic[GpuType.NVIDIA] = this.GetKernelBrandId(GpuType.NVIDIA);
                }
                else {
                    _kernelBrandDic.Add(GpuType.NVIDIA, this.GetKernelBrandId(GpuType.NVIDIA));
                }
                return _kernelBrandDic;
            }
        }

        public SysDicItemViewModel NKernelBrand {
            get {
                if (KernelBrandDic.TryGetValue(GpuType.NVIDIA, out Guid id)) {
                    if (AppContext.SysDicItemViewModels.Instance.TryGetValue(id, out SysDicItemViewModel sysDicItemVm)) {
                        return sysDicItemVm;
                    }
                    return null;
                }
                return null;
            }
            set {
                if (value != NKernelBrand) {
                    if (value == SysDicItemViewModel.PleaseSelect) {
                        value = null;
                    }
                    SetKernelBrand(value, AKernelBrand);
                    OnPropertyChanged(nameof(NKernelBrand));
                }
            }
        }

        public SysDicItemViewModel AKernelBrand {
            get {
                if (KernelBrandDic.TryGetValue(GpuType.AMD, out Guid id)) {
                    if (AppContext.SysDicItemViewModels.Instance.TryGetValue(id, out SysDicItemViewModel sysDicItemVm)) {
                        return sysDicItemVm;
                    }
                    return null;
                }
                return null;
            }
            set {
                if (value != AKernelBrand) {
                    if (value == SysDicItemViewModel.PleaseSelect) {
                        value = null;
                    }
                    SetKernelBrand(NKernelBrand, value);
                    OnPropertyChanged(nameof(AKernelBrand));
                }
            }
        }

        private void SetKernelBrand(SysDicItemViewModel n, SysDicItemViewModel a) {
            if (n == null) {
                if (a == null) {
                    this.KernelBrand = string.Empty;
                }
                else {
                    this.KernelBrand = $"{GpuType.AMD.GetName()}:{a.Id.ToString()}";
                }
            }
            else {
                if (a == null) {
                    this.KernelBrand = $"{GpuType.NVIDIA.GetName()}:{n.Id.ToString()}";
                }
                else {
                    this.KernelBrand = $"{GpuType.NVIDIA.GetName()}:{n.Id.ToString()};{GpuType.AMD.GetName()}:{a.Id.ToString()}";
                }
            }
        }

        public CoinProfileViewModel CoinProfile {
            get {
                if (!NTMinerRoot.Instance.ServerContext.CoinSet.Contains(this.Id)) {
                    return null;
                }
                return AppContext.Instance.CoinProfileVms.GetOrCreateCoinProfile(this.Id);
            }
        }

        public List<PoolViewModel> Pools {
            get {
                return AppContext.Instance.PoolVms.AllPools.Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).ToList();
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
                return AppContext.Instance.OverClockDataVms.Items.Where(a => a.CoinId == this.Id).ToList();
            }
        }

        public List<NTMinerWalletViewModel> NTMinerWallets {
            get {
                return AppContext.Instance.NTMinerWalletVms.Items.Where(a => a.CoinId == this.Id).ToList();
            }
        }

        private IEnumerable<WalletViewModel> GetWallets() {
            // 如果不是开发者模式就不要展示测试钱包了
            if (DevMode.IsDevMode) {
                if (!string.IsNullOrEmpty(TestWallet)) {
                    yield return TestWalletVm;
                }
            }
            foreach (var item in AppContext.Instance.WalletVms.WalletList.Where(a => a.CoinId == this.Id).OrderBy(a => a.SortNumber).ToList()) {
                yield return item;
            }
        }

        public List<WalletViewModel> Wallets {
            get {
                if (WpfUtil.IsInDesignMode) {
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
                return AppContext.Instance.CoinKernelVms.AllCoinKernels
                    .Where(a => a.CoinId == this.Id && a.Kernel != null && a.Kernel.PublishState == PublishStatus.Published)
                    .OrderBy(a => a.Kernel.Code)
                    .ThenByDescending(a => a.Kernel.Version).ToList();
            }
        }

        public CoinKernelViewModel CoinKernel {
            get {
                CoinKernelViewModel coinKernel = CoinKernels.FirstOrDefault(a => a.Id == CoinProfile.CoinKernelId);
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
