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
        private ulong _publishOn;
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
            if (Design.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Instance.KernelSet.Contains(this.Id)) {
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
                VirtualRoot.Execute(new KernelEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowDialog(message: $"您确定删除{this.FullName}内核吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.Publish = new DelegateCommand(() => {
                this.ShowDialog(message: $"您确定发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
                    this.PublishState = PublishStatus.Published;
                    this.PublishOn = Timestamp.GetTimestamp();
                    VirtualRoot.Execute(new UpdateKernelCommand(this));
                }, icon: IconConst.IconConfirm);
            });
            this.UnPublish = new DelegateCommand(() => {
                this.ShowDialog(message: $"您确定取消发布{this.Code} (v{this.Version})吗？", title: "确认", onYes: () => {
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
                if (AppContext.Instance.KernelVms.TryGetKernelVm(this.Id, out KernelViewModel kernelVm)) {
                    return kernelVm;
                }
                return null;
            }
        }

        public KernelOutputViewModel KernelOutputVm {
            get {
                if (_kernelOutputVm == null || _kernelOutputVm.Id != this.KernelOutputId) {
                    AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(this.KernelOutputId, out _kernelOutputVm);
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

        public AppContext.KernelOutputViewModels KernelOutputVms {
            get {
                return AppContext.Instance.KernelOutputVms;
            }
        }

        public KernelInputViewModel KernelInputVm {
            get {
                if (_kernelInputVm == null || _kernelInputVm.Id != this.KernelInputId) {
                    AppContext.Instance.KernelInputVms.TryGetKernelInputVm(this.KernelInputId, out _kernelInputVm);
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

        public AppContext.KernelInputViewModels KernelInputVms {
            get {
                return AppContext.Instance.KernelInputVms;
            }
        }

        public AppContext.GroupViewModels GroupVms {
            get {
                return AppContext.Instance.GroupVms;
            }
        }

        public List<KernelViewModel> OtherVersionKernelVms {
            get {
                return AppContext.Instance.KernelVms.AllKernels.Where(a => a.Code == this.Code && a.Id != this.Id).OrderBy(a => a.Code + a.Version).ToList();
            }
        }

        public List<CoinKernelViewModel> CoinKernels {
            get {
                return AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinViewModel> CoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>() {
                    CoinViewModel.PleaseSelect
                };
                var coinKernelVms = AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == this.Id).ToList();
                foreach (var item in AppContext.Instance.CoinVms.AllCoins) {
                    if (coinKernelVms.All(a => a.CoinId != item.Id)) {
                        list.Add(item);
                    }
                }
                return list.OrderBy(a => a.SortNumber).ToList();
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
                    _kernelProfileVm = new KernelProfileViewModel(this, NTMinerRoot.Instance.KernelProfileSet.GetKernelProfile(this.Id));
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
                SysDicItemViewModel item;
                if (AppContext.Instance.SysDicItemVms.TryGetValue(this.BrandId, out item)) {
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

        public AppContext.SysDicItemViewModels SysDicItemVms {
            get {
                return AppContext.Instance.SysDicItemVms;
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
                PackageViewModel packageVm = AppContext.Instance.PackageVms.AllPackages.FirstOrDefault(a => a.Name == this.Package);
                return packageVm != null;
            }
        }

        private PackageViewModel _packageVm;
        public PackageViewModel PackageVm {
            get {
                if (_packageVm == null || this.Package != _packageVm.Name) {
                    _packageVm = AppContext.Instance.PackageVms.AllPackages.FirstOrDefault(a => a.Name == this.Package);
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
                foreach (var item in NTMinerRoot.Instance.CoinKernelSet.Where(a => a.KernelId == this.Id)) {
                    if (AppContext.Instance.CoinVms.TryGetCoinVm(item.CoinId, out CoinViewModel coin)) {
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
                foreach (var coinVm in SupportedCoinVms.OrderBy(a => a.SortNumber)) {
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
                return EnumSet.PublishStatusEnumItems.FirstOrDefault(a => a.Value == PublishState);
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
