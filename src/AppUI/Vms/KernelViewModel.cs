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
            _kernelProfileVm = KernelProfileViewModel.Empty,
            _helpArg = string.Empty,
            _code = string.Empty,
            _brandId = Guid.Empty,
            _notice = string.Empty,
            _id = Guid.Empty,
            _publishState = PublishStatus.UnPublished,
            _sha1 = string.Empty,
            _size = 0,
            _publishOn = 0,
            _version = string.Empty,
            _package = string.Empty,
            _kernelOutputId = Guid.Empty,
            _kernelInputId = Guid.Empty
        };

        private Guid _id;
        private string _code;
        private Guid _brandId;
        private string _version;
        private string _helpArg;
        private ulong _publishOn;
        private string _package;
        private string _sha1;
        private long _size;
        private PublishStatus _publishState = PublishStatus.UnPublished;
        private string _notice;
        private Guid _kernelInputId;
        private Guid _kernelOutputId;

        private KernelProfileViewModel _kernelProfileVm;

        public Guid GetId() {
            return this.Id;
        }

        #region Commands
        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }

        public ICommand Publish { get; private set; }
        public ICommand UnPublish { get; private set; }

        public ICommand BrowsePackage { get; private set; }

        public ICommand ShowKernelHelp { get; private set; }

        public ICommand AddCoinKernel { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }
        #endregion

        #region ctor
        // 供设计视图使用
        public KernelViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelViewModel(IKernel data) : this(data.GetId()) {
            _helpArg = data.HelpArg;
            _code = data.Code;
            _brandId = data.BrandId;
            _notice = data.Notice;
            _publishState = data.PublishState;
            _sha1 = data.Sha1;
            _size = data.Size;
            _publishOn = data.PublishOn;
            _version = data.Version;
            _package = data.Package;
            _kernelOutputId = data.KernelOutputId;
            _kernelInputId = data.KernelInputId;
        }

        public KernelViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.KernelSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this == Empty) {
                    return;
                }
                KernelEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.FullName}内核吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.Publish = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.Published;
                    this.PublishOn = Timestamp.GetTimestamp();
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }, icon: IconConst.IconConfirm);
            });
            this.UnPublish = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定取消发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.UnPublished;
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }, icon: IconConst.IconConfirm);
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
                if (coinVm == null || coinVm.Id == Guid.Empty) {
                    return;
                }
                int sortNumber = coinVm.CoinKernels.Count == 0 ? 1 : coinVm.CoinKernels.Max(a => a.SortNumber) + 1;
                VirtualRoot.Execute(new AddCoinKernelCommand(new CoinKernelViewModel(Guid.NewGuid()) {
                    Args = string.Empty,
                    CoinId = coinVm.Id,
                    Description = string.Empty,
                    KernelId = this.Id,
                    SortNumber = sortNumber
                }));
            });
            this.ShowKernelHelp = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(HelpArg)) {
                    return;
                }
                string kernelDirFullName = this.GetKernelDirFullName();
                if (string.IsNullOrEmpty(kernelDirFullName)) {
                    return;
                }
                string helpArg = this.HelpArg.Trim();
                string asFileFullName = Path.Combine(kernelDirFullName, helpArg);
                // 如果当前内核不处在挖矿中则可以解压缩，否则不能解压缩因为内核文件处在使用中无法覆盖
                if (!NTMinerRoot.Current.IsMining || NTMinerRoot.Current.CurrentMineContext.Kernel.GetId() != this.GetId()) {
                    if (!this.IsPackageFileExist()) {
                        DialogWindow.ShowDialog(icon: "Icon_Info", title: "提示", message: "内核未安装");
                        return;
                    }
                    this.ExtractPackage();
                }
                string helpText;
                if (File.Exists(asFileFullName)) {
                    helpText = File.ReadAllText(asFileFullName);
                    KernelHelpPage.ShowWindow("内核帮助 - " + this.FullName, helpText);
                }
                else {
                    string commandName = this.GetCommandName(fromHelpArg: true);
                    string kernelExeFileFullName = Path.Combine(kernelDirFullName, commandName);
                    int exitCode = -1;
                    Windows.Cmd.RunClose(kernelExeFileFullName, helpArg.Substring(commandName.Length), ref exitCode, out helpText);
                }
                KernelHelpPage.ShowWindow("内核帮助 - " + this.FullName, helpText);
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

        private KernelOutputViewModel _kernelOutputVm;
        public KernelOutputViewModel KernelOutputVm {
            get {
                if (_kernelOutputVm == null || _kernelOutputVm.Id != this.KernelOutputId) {
                    KernelOutputViewModels.Current.TryGetKernelOutputVm(this.KernelOutputId, out _kernelOutputVm);
                    if (_kernelOutputVm == null) {
                        _kernelOutputVm = KernelOutputViewModel.PleaseSelect;
                    }
                }
                return _kernelOutputVm;
            }
            set {
                if (_kernelOutputVm != value) {
                    _kernelOutputVm = value;
                    this.KernelOutputId = value.Id;
                    OnPropertyChanged(nameof(KernelOutputVm));
                }
            }
        }

        public KernelOutputViewModels KernelOutputVms {
            get {
                return KernelOutputViewModels.Current;
            }
        }

        private KernelInputViewModel _kernelInputVm;
        public KernelInputViewModel KernelInputVm {
            get {
                if (_kernelInputVm == null || _kernelInputVm.Id != this.KernelInputId) {
                    KernelInputViewModels.Current.TryGetKernelInputVm(this.KernelInputId, out _kernelInputVm);
                    if (_kernelInputVm == null) {
                        _kernelInputVm = KernelInputViewModel.PleaseSelect;
                    }
                }
                return _kernelInputVm;
            }
            set {
                if (_kernelInputVm != value) {
                    _kernelInputVm = value;
                    this.KernelInputId = value.Id;
                    OnPropertyChanged(nameof(KernelInputVm));
                }
            }
        }

        public KernelInputViewModels KernelInputVms {
            get {
                return KernelInputViewModels.Current;
            }
        }

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Current;
            }
        }

        public List<KernelViewModel> OtherVersionKernelVms {
            get {
                return KernelViewModels.Current.AllKernels.Where(a => a.Code == this.Code && a.Id != this.Id).OrderBy(a => a.Code + a.Version).ToList();
            }
        }

        public List<CoinKernelViewModel> CoinKernels {
            get {
                return CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinViewModel> CoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>() {
                    CoinViewModel.PleaseSelect
                };
                var coinKernelVms = CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == this.Id).ToList();
                foreach (var item in CoinViewModels.Current.AllCoins) {
                    if (coinKernelVms.All(a => a.CoinId != item.Id)) {
                        list.Add(item);
                    }
                }
                return list.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (item.SupportedGpu == SupportedGpu.Both) {
                        return Visibility.Visible;
                    }
                    if (item.SupportedGpu == SupportedGpu.NVIDIA) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAMDIconVisible {
            get {
                foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (item.SupportedGpu == SupportedGpu.Both) {
                        return Visibility.Visible;
                    }
                    if (item.SupportedGpu == SupportedGpu.AMD) {
                        return Visibility.Visible;
                    }
                }
                return Visibility.Collapsed;
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
                    OnPropertyChanged(nameof(KernelNotice));
                    if (this == Empty) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                }
            }
        }

        public Guid BrandId {
            get { return _brandId; }
            set {
                _brandId = value;
                OnPropertyChanged(nameof(BrandId));
            }
        }

        public SysDicItemViewModel BrandItem {
            get {
                if (this.BrandId == Guid.Empty) {
                    return SysDicItemViewModel.PleaseSelect;
                }
                SysDicItemViewModel item;
                if (SysDicItemViewModels.Current.TryGetValue(this.BrandId, out item)) {
                    return item;
                }
                return SysDicItemViewModel.PleaseSelect;
            }
            set {
                if (value == null) {
                    value = SysDicItemViewModel.PleaseSelect;
                }
                this.BrandId = value.Id;
                OnPropertyChanged(nameof(BrandItem));
            }
        }

        public SysDicItemViewModels SysDicItemVms {
            get {
                return SysDicItemViewModels.Current;
            }
        }

        public string Version {
            get { return _version; }
            set {
                if (value != _version) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(KernelNotice));
                    if (this == Empty) {
                        return;
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
                if (_package != value) {
                    _package = value;
                    OnPropertyChanged(nameof(Package));
                }
            }
        }

        public string Sha1 {
            get => _sha1;
            set {
                if (_sha1 != value) {
                    _sha1 = value;
                    OnPropertyChanged(nameof(Sha1));
                }
            }
        }

        public long Size {
            get => _size;
            set {
                if (_size != value) {
                    _size = value;
                    OnPropertyChanged(nameof(Size));
                    OnPropertyChanged(nameof(SizeMbText));
                }
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
                if (_publishOn != value) {
                    _publishOn = value;
                    OnPropertyChanged(nameof(PublishOn));
                    OnPropertyChanged(nameof(PublishOnText));
                    OnPropertyChanged(nameof(IsPublished));
                }
            }
        }

        public bool IsPublished {
            get {
                if (this.PublishState == PublishStatus.Published) {
                    return true;
                }
                return false;
            }
        }

        public string PublishOnText {
            get {
                return Timestamp.UnixBaseTime.AddSeconds(this.PublishOn).ToString("yyyy-MM-dd HH:mm");
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
                if (_publishState != value) {
                    _publishState = value;
                    OnPropertyChanged(nameof(PublishState));
                    OnPropertyChanged(nameof(PublishStateDescription));
                    OnPropertyChanged(nameof(IsBtnPublishVisible));
                    OnPropertyChanged(nameof(PublishOnText));
                }
            }
        }

        public EnumItem<PublishStatus> PublishStateEnumItem {
            get {
                return AppStatic.PublishStatusEnumItems.FirstOrDefault(a => a.Value == PublishState);
            }
            set {
                if (PublishState != value.Value) {
                    PublishState = value.Value;
                    OnPropertyChanged(nameof(PublishStateEnumItem));
                }
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

        public string HelpArg {
            get { return _helpArg; }
            set {
                if (_helpArg != value) {
                    _helpArg = value;
                    OnPropertyChanged(nameof(HelpArg));
                }
            }
        }

        public string Notice {
            get => _notice;
            set {
                if (_notice != value) {
                    _notice = value;
                    OnPropertyChanged(nameof(Notice));
                    OnPropertyChanged(nameof(KernelNotice));
                }
            }
        }

        public string KernelNotice {
            get {
                if (string.IsNullOrEmpty(this.Notice)) {
                    return this.FullName;
                }
                return $"{this.FullName}：{this.Notice}";
            }
        }

        public Guid KernelInputId {
            get { return _kernelInputId; }
            set {
                if (_kernelInputId != value) {
                    _kernelInputId = value;
                    OnPropertyChanged(nameof(KernelInputId));
                    OnPropertyChanged(nameof(KernelInputVm));
                }
            }
        }

        public Guid KernelOutputId {
            get { return _kernelOutputId; }
            set {
                if (_kernelOutputId != value) {
                    _kernelOutputId = value;
                    OnPropertyChanged(nameof(KernelOutputId));
                    OnPropertyChanged(nameof(KernelOutputVm));
                }
            }
        }
    }
}
