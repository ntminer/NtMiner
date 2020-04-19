using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelViewModel : ViewModelBase, IEditableViewModel, IKernel {
        public static readonly KernelViewModel Empty = new KernelViewModel(Guid.Empty) {
            _kernelProfileVm = KernelProfileViewModel.Empty,
            _code = string.Empty,
            _brandId = Guid.Empty,
            _notice = string.Empty,
            _id = Guid.Empty,
            _publishState = PublishStatus.UnPublished,
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
        private long _publishOn;
        private string _package;
        private long _size;
        private PublishStatus _publishState = PublishStatus.UnPublished;
        private string _notice;
        private Guid _kernelInputId;
        private Guid _kernelOutputId;
        private KernelInputViewModel _kernelInputVm;
        private KernelOutputViewModel _kernelOutputVm;
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

        public ICommand Save { get; private set; }
        #endregion

        #region ctor
        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public KernelViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public KernelViewModel(IKernel data) : this(data.GetId()) {
            _code = data.Code;
            _brandId = data.BrandId;
            _notice = data.Notice;
            _publishState = data.PublishState;
            _size = data.Size;
            _publishOn = data.PublishOn;
            _version = data.Version;
            _package = data.Package;
            _kernelOutputId = data.KernelOutputId;
            _kernelInputId = data.KernelInputId;
        }

        public KernelViewModel(Guid id) {
            _id = id;
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                if (NTMinerContext.Instance.ServerContext.KernelSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this == Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditKernelCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.FullName}内核吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelCommand(this.Id));
                }));
            });
            this.Publish = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.Published;
                    this.PublishOn = Timestamp.GetTimestamp();
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }));
            });
            this.UnPublish = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定取消发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.UnPublished;
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }));
            });
            this.BrowsePackage = new DelegateCommand(() => {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "zip (*.zip)|*.zip",
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string package = Path.GetFileName(openFileDialog.FileName);
                    this.Package = package;
                    this.Size = new FileInfo(openFileDialog.FileName).Length;
                    // 当内核文件改变时同时更新发布时间
                    this.PublishOn = Timestamp.GetTimestamp();
                    this.KernelProfileVm.Refresh();
                }
            });
        }
        #endregion

        public KernelViewModel KernelVmSingleInstance {
            get {
                if (AppRoot.KernelVms.TryGetKernelVm(this.Id, out KernelViewModel kernelVm)) {
                    return kernelVm;
                }
                return null;
            }
        }

        public KernelOutputViewModel KernelOutputVm {
            get {
                if (_kernelOutputVm == null || _kernelOutputVm.Id != this.KernelOutputId) {
                    AppRoot.KernelOutputVms.TryGetKernelOutputVm(this.KernelOutputId, out _kernelOutputVm);
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

        public AppRoot.KernelOutputViewModels KernelOutputVms {
            get {
                return AppRoot.KernelOutputVms;
            }
        }

        public KernelInputViewModel KernelInputVm {
            get {
                if (_kernelInputVm == null || _kernelInputVm.Id != this.KernelInputId) {
                    AppRoot.KernelInputVms.TryGetKernelInputVm(this.KernelInputId, out _kernelInputVm);
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

        public AppRoot.KernelInputViewModels KernelInputVms {
            get {
                return AppRoot.KernelInputVms;
            }
        }

        public AppRoot.GroupViewModels GroupVms {
            get {
                return AppRoot.GroupVms;
            }
        }

        public List<CoinKernelViewModel> CoinKernels {
            get {
                return AppRoot.CoinKernelVms.AllCoinKernels
                    .Where(a => a.KernelId == this.Id)
                    .OrderBy(a => a.CoinCode).ToList();
            }
        }

        public List<CoinViewModel> CoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>() {
                    CoinViewModel.PleaseSelect
                };
                var coinKernelVms = AppRoot.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == this.Id).ToList();
                foreach (var item in AppRoot.CoinVms.AllCoins) {
                    if (coinKernelVms.All(a => a.CoinId != item.Id)) {
                        list.Add(item);
                    }
                }
                return list.OrderBy(a => a.Code).ToList();
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
                    _kernelProfileVm = new KernelProfileViewModel(this, NTMinerContext.Instance.KernelProfileSet.GetKernelProfile(this.Id));
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

        public Guid BrandId {
            get { return _brandId; }
            set {
                _brandId = value;
                OnPropertyChanged(nameof(BrandId));
                OnPropertyChanged(nameof(BrandItem));
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
                }
            }
        }

        public string FullName {
            get {
                return this.GetFullName();
            }
        }

        public string Package {
            get { return _package; }
            set {
                if (_package != value) {
                    _package = value;
                    OnPropertyChanged(nameof(Package));
                    OnPropertyChanged(nameof(PackageVm));
                    OnPropertyChanged(nameof(IsPackageValid));
                }
            }
        }

        public bool IsPackageValid {
            get {
                if (string.IsNullOrEmpty(this.Package)) {
                    return false;
                }
                PackageViewModel packageVm = AppRoot.PackageVms.AllPackages.FirstOrDefault(a => a.Name == this.Package);
                return packageVm != null;
            }
        }

        private PackageViewModel _packageVm;
        public PackageViewModel PackageVm {
            get {
                if (_packageVm == null || this.Package != _packageVm.Name) {
                    _packageVm = AppRoot.PackageVms.AllPackages.FirstOrDefault(a => a.Name == this.Package);
                }
                return _packageVm;
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
                foreach (var item in NTMinerContext.Instance.ServerContext.CoinKernelSet.AsEnumerable().Where(a => a.KernelId == this.Id)) {
                    if (AppRoot.CoinVms.TryGetCoinVm(item.CoinId, out CoinViewModel coin)) {
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
                foreach (var coinVm in SupportedCoinVms.OrderBy(a => a.Code)) {
                    if (len != sb.Length) {
                        sb.Append(",");
                    }
                    sb.Append(coinVm.Code);
                }
                return sb.ToString();
            }
        }

        public long PublishOn {
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
                return Timestamp.UnixBaseTime.AddSeconds(this.PublishOn).ToString("yyyy-MM-dd");
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
                return NTMinerContext.PublishStatusEnumItems.FirstOrDefault(a => a.Value == PublishState);
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

        public string Notice {
            get => _notice;
            set {
                if (_notice != value) {
                    _notice = value;
                    OnPropertyChanged(nameof(Notice));
                }
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
