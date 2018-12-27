using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelViewModel : ViewModelBase, IKernel {
        public static readonly KernelViewModel Empty = new KernelViewModel(Guid.Empty) {
            _translaterKeyword = string.Empty,
            _kernelProfileVm = KernelProfileViewModel.Empty,
            _args = string.Empty,
            _isSupportDualMine = false,
            _dualFullArgs = string.Empty,
            _helpArg = string.Empty,
            _code = string.Empty,
            _notice = string.Empty,
            _id = Guid.Empty,
            _publishState = PublishStatus.UnPublished,
            _sha1 = string.Empty,
            _size = 0,
            _sortNumber = 0,
            _dualCoinGroupId = Guid.Empty,
            _publishOn = 0,
            _version = string.Empty,
            _package = string.Empty,
            _packageHistory = string.Empty,
            _gpuSpeedPattern = string.Empty,
            _rejectSharePattern = string.Empty,
            _rejectPercentPattern = string.Empty,
            _totalSharePattern = string.Empty,
            _acceptSharePattern = string.Empty,
            _totalSpeedPattern = string.Empty,
            _dualGpuSpeedPattern = string.Empty,
            _dualRejectSharePattern = string.Empty,
            _dualRejectPercentPattern = string.Empty,
            _dualTotalSharePattern = string.Empty,
            _dualAcceptSharePattern = string.Empty,
            _dualTotalSpeedPattern = string.Empty
        };

        private Guid _id;
        private string _code;
        private string _version;
        private int _sortNumber;
        private string _args;
        private bool _isSupportDualMine;
        private string _dualFullArgs;
        private string _helpArg;
        private ulong _publishOn;
        private string _package;
        private string _packageHistory;
        private string _sha1;
        private long _size;
        private PublishStatus _publishState = PublishStatus.UnPublished;
        private string _notice;
        private string _totalSpeedPattern;
        private string _totalSharePattern;
        private string _acceptSharePattern;
        private string _gpuSpeedPattern;
        private string _rejectSharePattern;
        private string _rejectPercentPattern;

        private string _dualTotalSpeedPattern;
        private string _dualTotalSharePattern;
        private string _dualAcceptSharePattern;
        private string _dualGpuSpeedPattern;
        private string _dualRejectSharePattern;
        private string _dualRejectPercentPattern;

        private KernelProfileViewModel _kernelProfileVm;
        private string _translaterKeyword;
        private Guid _dualCoinGroupId;

        public Guid GetId() {
            return this.Id;
        }

        #region Commands
        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public ICommand Publish { get; private set; }
        public ICommand UnPublish { get; private set; }

        public ICommand BrowsePackage { get; private set; }

        public ICommand AddKernelOutputFilter { get; private set; }

        public ICommand AddKernelOutputTranslater { get; private set; }

        public ICommand ShowKernelHelp { get; private set; }

        public ICommand ClearTranslaterKeyword { get; private set; }
        public ICommand SelectCopySourceKernel { get; private set; }
        public ICommand AddCoinKernel { get; private set; }
        #endregion

        #region ctor
        // 供设计视图使用
        public KernelViewModel() {
            if (!NTMinerRoot.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelViewModel(IKernel data) : this(data.GetId()) {
            _args = data.Args;
            _isSupportDualMine = data.IsSupportDualMine;
            _dualFullArgs = data.DualFullArgs;
            _helpArg = data.HelpArg;
            _code = data.Code;
            _notice = data.Notice;
            _publishState = data.PublishState;
            _sha1 = data.Sha1;
            _size = data.Size;
            _sortNumber = data.SortNumber;
            _dualCoinGroupId = data.DualCoinGroupId;
            _publishOn = data.PublishOn;
            _version = data.Version;
            _package = data.Package;
            _packageHistory = data.PackageHistory;
            _gpuSpeedPattern = data.GpuSpeedPattern;
            _rejectSharePattern = data.RejectSharePattern;
            _totalSharePattern = data.TotalSharePattern;
            _acceptSharePattern = data.AcceptSharePattern;
            _rejectPercentPattern = data.RejectPercentPattern;
            _totalSpeedPattern = data.TotalSpeedPattern;
            _dualGpuSpeedPattern = data.DualGpuSpeedPattern;
            _dualRejectSharePattern = data.DualRejectSharePattern;
            _dualAcceptSharePattern = data.DualAcceptSharePattern;
            _dualRejectPercentPattern = data.DualRejectPercentPattern;
            _dualTotalSharePattern = data.DualTotalSharePattern;
            _dualTotalSpeedPattern = data.DualTotalSpeedPattern;
        }

        public KernelViewModel(Guid id) {
            _id = id;
            this.Edit = new DelegateCommand(() => {
                if (this == Empty) {
                    return;
                }
                KernelEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.FullName}内核吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveKernelCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.SortUp = new DelegateCommand(() => {
                KernelViewModel upOne = KernelViewModels.Current.AllKernels.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateKernelCommand(upOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateKernelCommand(this));
                    KernelViewModels.Current.OnPropertyChanged(nameof(KernelViewModels.AllKernels));
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.QueryResults));
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.DownloadingVms));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                KernelViewModel nextOne = KernelViewModels.Current.AllKernels.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateKernelCommand(nextOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateKernelCommand(this));
                    KernelViewModels.Current.OnPropertyChanged(nameof(KernelViewModels.AllKernels));
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.QueryResults));
                    KernelPageViewModel.Current.OnPropertyChanged(nameof(KernelPageViewModel.DownloadingVms));
                }
            });
            this.Publish = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.Published;
                    this.PublishOn = Global.GetTimestamp();
                    Global.Execute(new UpdateKernelCommand(this));
                }, icon: "Icon_Confirm");
            });
            this.UnPublish = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定取消发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.UnPublished;
                    Global.Execute(new UpdateKernelCommand(this));
                }, icon: "Icon_Confirm");
            });
            this.BrowsePackage = new DelegateCommand(() => {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "zip (*.zip)|*.zip",
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string package = Path.GetFileName(openFileDialog.FileName);
                    this.Sha1 = HashUtil.Sha1(File.ReadAllBytes(openFileDialog.FileName));
                    this.Package = package;
                    this.Size = new FileInfo(openFileDialog.FileName).Length;
                    this.KernelProfileVm.Refresh();
                }
            });
            this.AddCoinKernel = new DelegateCommand<CoinViewModel>((coinVm) => {
                int sortNumber = coinVm.CoinKernels.Count == 0 ? 1 : coinVm.CoinKernels.Max(a => a.SortNumber) + 1;
                Global.Execute(new AddCoinKernelCommand(new CoinKernelViewModel(Guid.NewGuid()) {
                    Args = string.Empty,
                    CoinId = coinVm.Id,
                    Description = string.Empty,
                    KernelId = this.Id,
                    SortNumber = sortNumber
                }));
            });
            this.AddKernelOutputFilter = new DelegateCommand(() => {
                new KernelOutputFilterViewModel(Guid.NewGuid()) {
                    KernelId = this.Id
                }.Edit.Execute(null);
            });
            this.AddKernelOutputTranslater = new DelegateCommand(() => {
                int sortNumber = this.KernelOutputTranslaters.Count == 0 ? 1 : this.KernelOutputTranslaters.Count + 1;
                new KernelOutputTranslaterViewModel(Guid.NewGuid()) {
                    KernelId = this.Id,
                    SortNumber = sortNumber
                }.Edit.Execute(null);
            });
            this.ShowKernelHelp = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(HelpArg)) {
                    return;
                }
                string helpArg = this.HelpArg.Trim();
                string asFileFullName = Path.Combine(this.GetKernelDirFullName(), helpArg);
                // 如果当前内核不处在挖矿中则可以解压缩，否则不能解压缩因为内核文件处在使用中无法覆盖
                if (!NTMinerRoot.Current.IsMining || NTMinerRoot.Current.CurrentMineContext.Kernel.GetId() != this.GetId()) {
                    this.ExtractPackage();
                }
                string helpText;
                if (File.Exists(asFileFullName)) {
                    helpText = File.ReadAllText(asFileFullName, Encoding.UTF8);
                    KernelHelpPage.ShowWindow("内核帮助 - " + this.FullName, helpText);
                }
                else {
                    string commandName = this.GetCommandName(fromHelpArg: true);
                    string kernelExeFileFullName = Path.Combine(this.GetKernelDirFullName(), commandName);
                    int exitCode = -1;
                    Windows.Cmd.RunClose(kernelExeFileFullName, helpArg.Substring(commandName.Length), ref exitCode, out helpText);
                }
                KernelHelpPage.ShowWindow("内核帮助 - " + this.FullName, helpText);
            });
            this.ClearTranslaterKeyword = new DelegateCommand(() => {
                this.TranslaterKeyword = string.Empty;
            });
            this.SelectCopySourceKernel = new DelegateCommand<string>((tag) => {
                KernelCopySourceSelect.ShowWindow(this, tag);
            });
        }
        #endregion

        public KernelViewModel KernelVmSingleInstance {
            get {
                if (KernelViewModels.Current.TryGetKernelVm(this.Id, out KernelViewModel kernelVm)) {
                    return kernelVm;
                }
                return null;
            }
        }

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Current;
            }
        }

        public List<KernelOutputFilterViewModel> KernelOutputFilters {
            get {
                return KernelOutputFilterViewModels.Current.GetListByKernelId(this.Id).ToList();
            }
        }

        public List<KernelViewModel> OtherVersionKernelVms {
            get {
                return KernelViewModels.Current.AllKernels.Where(a => a.Code == this.Code && a.Id != this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinKernelViewModel> CoinKernels {
            get {
                return CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinViewModel> CoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>();
                var coinKernelVms = CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == this.Id).ToList();
                foreach (var item in CoinViewModels.Current.AllCoins) {
                    if (coinKernelVms.All(a => a.CoinId != item.Id)) {
                        list.Add(item);
                    }
                }
                return list.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public string TranslaterKeyword {
            get { return _translaterKeyword; }
            set {
                _translaterKeyword = value;
                OnPropertyChanged(nameof(TranslaterKeyword));
                OnPropertyChanged(nameof(KernelOutputTranslaters));
            }
        }

        public List<KernelOutputTranslaterViewModel> KernelOutputTranslaters {
            get {
                var query = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.Id).AsQueryable();
                if (!string.IsNullOrEmpty(TranslaterKeyword)) {
                    query = query.Where(a => (a.RegexPattern != null && a.RegexPattern.Contains(TranslaterKeyword))
                        || (a.Replacement != null && a.Replacement.Contains(TranslaterKeyword)));
                }
                return query.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public Visibility IsTransFilterVisible {
            get {
                if (!DevMode.IsDevMode) {
                    return Visibility.Collapsed;
                }
                if (NTMinerRoot.Current.KernelSet.Contains(this.Id)) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (item.SupportedGpu == Core.Gpus.SupportedGpu.Both) {
                        return Visibility.Visible;
                    }
                    if (item.SupportedGpu == Core.Gpus.SupportedGpu.NVIDIA && NTMinerRoot.Current.GpuSet.GpuType == Core.Gpus.GpuType.NVIDIA) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAMDIconVisible {
            get {
                foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (item.SupportedGpu == Core.Gpus.SupportedGpu.Both) {
                        return Visibility.Visible;
                    }
                    if (item.SupportedGpu == Core.Gpus.SupportedGpu.AMD && NTMinerRoot.Current.GpuSet.GpuType == Core.Gpus.GpuType.AMD) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public KernelProfileViewModel KernelProfileVm {
            get {
                if (_kernelProfileVm == null) {
                    _kernelProfileVm = new KernelProfileViewModel(this, NTMinerRoot.Current.KernelProfileSet.GetKernelProfile(this.Id));
                }
                return _kernelProfileVm;
            }
        }

        public string Code {
            get { return _code; }
            set {
                if (_code != value) {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                    OnPropertyChanged(nameof(FullName));
                    if (this == Empty) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                }
            }
        }

        public string Version {
            get { return _version; }
            set {
                if (value != _version) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    OnPropertyChanged(nameof(FullName));
                    if (this == Empty) {
                        return;
                    }
                    if (!string.IsNullOrEmpty(value)) {
                        if (!System.Version.TryParse(value, out Version version)) {
                            throw new ValidationException("版本号格式不正确");
                        }
                    }
                }
            }
        }

        public string FullName {
            get {
                if (this == Empty) {
                    return "空";
                }
                return $"{this.Code}(v{this.Version})";
            }
        }

        public string Package {
            get { return _package; }
            set {
                _package = value;
                OnPropertyChanged(nameof(Package));
            }
        }

        public string PackageHistory {
            get { return _packageHistory; }
            set {
                _packageHistory = value;
                OnPropertyChanged(nameof(PackageHistory));
                KernelProfileVm.Refresh();
            }
        }

        public string Sha1 {
            get => _sha1;
            set {
                _sha1 = value;
                OnPropertyChanged(nameof(Sha1));
            }
        }

        public long Size {
            get => _size;
            set {
                _size = value;
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(SizeMbText));
            }
        }

        public string SizeMbText {
            get {
                return (Size / (1024.0 * 1024)).ToString("f1") + " Mb";
            }
        }

        public List<CoinViewModel> SupportedCoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>();
                foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (CoinViewModels.Current.TryGetCoinVm(item.CoinId, out CoinViewModel coin)) {
                        list.Add(coin);
                    }
                }
                return list;
            }
        }

        public string SupportedCoins {
            get {
                StringBuilder sb = new StringBuilder();
                int len = sb.Length;
                foreach (var coinVm in SupportedCoinVms) {
                    if (len != sb.Length) {
                        sb.Append(",");
                    }
                    sb.Append(coinVm.Code);
                }
                return sb.ToString();
            }
        }

        public ulong PublishOn {
            get => _publishOn;
            set {
                _publishOn = value;
                OnPropertyChanged(nameof(PublishOn));
                OnPropertyChanged(nameof(PublishOnText));
            }
        }

        public string PublishOnText {
            get {
                if (this.PublishState == PublishStatus.UnPublished) {
                    return "未发布";
                }
                return Global.UnixBaseTime.AddSeconds(this.PublishOn).ToString("yyyy-MM-dd HH:mm") + "发布";
            }
        }

        public Visibility IsBtnPublishVisible {
            get {
                if (PublishState == PublishStatus.Published) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public PublishStatus PublishState {
            get => _publishState;
            set {
                _publishState = value;
                OnPropertyChanged(nameof(PublishState));
                OnPropertyChanged(nameof(PublishStateDescription));
                OnPropertyChanged(nameof(IsBtnPublishVisible));
                OnPropertyChanged(nameof(PublishOnText));
            }
        }

        public IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return PublishStatus.Published.GetEnumItems();
            }
        }

        public EnumItem<PublishStatus> PublishStateEnumItem {
            get {
                return PublishStatusEnumItems.FirstOrDefault(a => a.Value == PublishState);
            }
            set {
                PublishState = value.Value;
                OnPropertyChanged(nameof(PublishStateEnumItem));
            }
        }

        public string PublishStateDescription {
            get {
                return PublishState.GetDescription();
            }
        }

        public bool IsSupported {
            get {
                return this.IsSupported();
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                _sortNumber = value;
                OnPropertyChanged(nameof(SortNumber));
            }
        }

        public Guid DualCoinGroupId {
            get => _dualCoinGroupId;
            set {
                _dualCoinGroupId = value;
                OnPropertyChanged(nameof(DualCoinGroupId));
            }
        }

        private GroupViewModel _dualCoinGroup;
        public GroupViewModel DualCoinGroup {
            get {
                if (this.DualCoinGroupId == Guid.Empty) {
                    return GroupViewModel.PleaseSelect;
                }
                if (_dualCoinGroup == null || _dualCoinGroup.Id != this.DualCoinGroupId) {
                    GroupViewModels.Current.TryGetGroupVm(DualCoinGroupId, out _dualCoinGroup);
                    if (_dualCoinGroup == null) {
                        _dualCoinGroup = GroupViewModel.PleaseSelect;
                    }
                }
                return _dualCoinGroup;
            }
            set {
                if (DualCoinGroupId != value.Id) {
                    DualCoinGroupId = value.Id;
                    OnPropertyChanged(nameof(DualCoinGroup));
                }
            }
        }

        public string Args {
            get { return _args; }
            set {
                _args = value;
                OnPropertyChanged(nameof(Args));
            }
        }

        public bool IsSupportDualMine {
            get => _isSupportDualMine;
            set {
                _isSupportDualMine = value;
                OnPropertyChanged(nameof(IsSupportDualMine));
            }
        }

        public string DualFullArgs {
            get { return _dualFullArgs; }
            set {
                _dualFullArgs = value;
                OnPropertyChanged(nameof(DualFullArgs));
            }
        }

        public string HelpArg {
            get { return _helpArg; }
            set {
                _helpArg = value;
                OnPropertyChanged(nameof(HelpArg));
            }
        }

        public string Notice {
            get => _notice;
            set {
                _notice = value;
                OnPropertyChanged(nameof(Notice));
            }
        }

        public string TotalSpeedPattern {
            get => _totalSpeedPattern;
            set {
                _totalSpeedPattern = value;
                OnPropertyChanged(nameof(TotalSpeedPattern));
            }
        }

        public string TotalSharePattern {
            get { return _totalSharePattern; }
            set {
                _totalSharePattern = value;
                OnPropertyChanged(nameof(TotalSharePattern));
            }
        }

        public string AcceptSharePattern {
            get { return _acceptSharePattern; }
            set {
                _acceptSharePattern = value;
                OnPropertyChanged(nameof(AcceptSharePattern));
            }
        }

        public string RejectSharePattern {
            get { return _rejectSharePattern; }
            set {
                _rejectSharePattern = value;
                OnPropertyChanged(nameof(RejectSharePattern));
            }
        }

        public string RejectPercentPattern {
            get { return _rejectPercentPattern; }
            set {
                _rejectPercentPattern = value;
                OnPropertyChanged(nameof(RejectPercentPattern));
            }
        }

        public string GpuSpeedPattern {
            get => _gpuSpeedPattern;
            set {
                _gpuSpeedPattern = value;
                OnPropertyChanged(nameof(GpuSpeedPattern));
            }
        }

        public string DualTotalSpeedPattern {
            get => _dualTotalSpeedPattern;
            set {
                _dualTotalSpeedPattern = value;
                OnPropertyChanged(nameof(DualTotalSpeedPattern));
            }
        }

        public string DualTotalSharePattern {
            get { return _dualTotalSharePattern; }
            set {
                _dualTotalSharePattern = value;
                OnPropertyChanged(nameof(DualTotalSharePattern));
            }
        }

        public string DualAcceptSharePattern {
            get { return _dualAcceptSharePattern; }
            set {
                _dualAcceptSharePattern = value;
                OnPropertyChanged(nameof(DualAcceptSharePattern));
            }
        }

        public string DualRejectSharePattern {
            get { return _dualRejectSharePattern; }
            set {
                _dualRejectSharePattern = value;
                OnPropertyChanged(nameof(DualRejectSharePattern));
            }
        }

        public string DualRejectPercentPattern {
            get { return _dualRejectPercentPattern; }
            set {
                _dualRejectPercentPattern = value;
                OnPropertyChanged(nameof(DualRejectPercentPattern));
            }
        }

        public string DualGpuSpeedPattern {
            get => _dualGpuSpeedPattern;
            set {
                _dualGpuSpeedPattern = value;
                OnPropertyChanged(nameof(DualGpuSpeedPattern));
            }
        }
    }
}
