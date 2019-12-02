using NTMiner.Core;
using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinKernelViewModel : ViewModelBase, ICoinKernel, IEditableViewModel {
        private Guid _id;
        private Guid _coinId;
        private Guid _kernelId;
        private Guid _dualCoinGroupId;
        private bool _isSupportPool1;
        private string _args;
        private string _dualFullArgs;
        private string _notice;
        private bool _isHot;
        private bool _isRecommend;
        private SupportedGpu _supportedGpu;
        private GroupViewModel _selectedDualCoinGroup;
        private List<EnvironmentVariable> _environmentVariables = new List<EnvironmentVariable>();
        private List<InputSegment> _inputSegments = new List<InputSegment>();
        private List<InputSegmentViewModel> _inputSegmentVms = new List<InputSegmentViewModel>();
        private List<InputSegmentViewModel> _gpuInputSegmentVms = new List<InputSegmentViewModel>();
        private List<Guid> _fileWriterIds = new List<Guid>();
        private List<Guid> _fragmentWriterIds = new List<Guid>();
        private List<FileWriterViewModel> _fileWriterVms = new List<FileWriterViewModel>();
        private List<FragmentWriterViewModel> _fragmentWriterVms = new List<FragmentWriterViewModel>();
        private CoinViewModel _coinVm;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public ICommand AddEnvironmentVariable { get; private set; }
        public ICommand EditEnvironmentVariable { get; private set; }
        public ICommand RemoveEnvironmentVariable { get; private set; }
        public ICommand AddSegment { get; private set; }
        public ICommand EditSegment { get; private set; }
        public ICommand RemoveSegment { get; private set; }
        public ICommand RemoveFileWriter { get; private set; }
        public ICommand RemoveFragmentWriter { get; private set; }

        public CoinKernelViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public CoinKernelViewModel(ICoinKernel data) : this(data.GetId()) {
            _coinId = data.CoinId;
            _kernelId = data.KernelId;
            _dualCoinGroupId = data.DualCoinGroupId;
            _args = data.Args;
            _dualFullArgs = data.DualFullArgs;
            _notice = data.Notice;
            _supportedGpu = data.SupportedGpu;
            _isSupportPool1 = data.IsSupportPool1;
            // 复制，视为值对象，防止直接修改引用
            _environmentVariables.AddRange(data.EnvironmentVariables.Select(a => new EnvironmentVariable(a)));
            // 复制，视为值对象，防止直接修改引用
            _inputSegments.AddRange(data.InputSegments.Select(a => new InputSegment(a)));
            _inputSegmentVms.AddRange(_inputSegments.Select(a => new InputSegmentViewModel(a)));
            _gpuInputSegmentVms.AddRange(_inputSegmentVms.Where(a => a.TargetGpu.IsSupportedGpu(NTMinerRoot.Instance.GpuSet.GpuType)));
            _fileWriterIds = data.FileWriterIds;
            _fragmentWriterIds = data.FragmentWriterIds;
            _isHot = data.IsHot;
            _isRecommend = data.IsRecommend;
            foreach (var writerId in _fileWriterIds) {
                if (AppContext.Instance.FileWriterVms.TryGetFileWriterVm(writerId, out FileWriterViewModel writerVm)) {
                    _fileWriterVms.Add(writerVm);
                }
            }
            foreach (var writerId in _fragmentWriterIds) {
                if (AppContext.Instance.FragmentWriterVms.TryGetFragmentWriterVm(writerId, out FragmentWriterViewModel writerVm)) {
                    _fragmentWriterVms.Add(writerVm);
                }
            }
        }

        public CoinKernelViewModel(Guid id) {
            _id = id;
            this.AddEnvironmentVariable = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnvironmentVariableEditCommand(this, new EnvironmentVariable()));
            });
            this.EditEnvironmentVariable = new DelegateCommand<EnvironmentVariable>(environmentVariable => {
                VirtualRoot.Execute(new EnvironmentVariableEditCommand(this, environmentVariable));
            });
            this.RemoveEnvironmentVariable = new DelegateCommand<EnvironmentVariable>(environmentVariable => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除环境变量{environmentVariable.Key}吗？", title: "确认", onYes: () => {
                    this.EnvironmentVariables.Remove(environmentVariable);
                    EnvironmentVariables = EnvironmentVariables.ToList();
                }));
            });
            this.AddSegment = new DelegateCommand(() => {
                VirtualRoot.Execute(new InputSegmentEditCommand(this, new InputSegmentViewModel()));
            });
            this.EditSegment = new DelegateCommand<InputSegmentViewModel>((segment) => {
                VirtualRoot.Execute(new InputSegmentEditCommand(this, segment));
            });
            this.RemoveSegment = new DelegateCommand<InputSegmentViewModel>((segment) => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除片段{segment.Name}吗？", title: "确认", onYes: () => {
                    this.InputSegments.Remove(this.InputSegments.FirstOrDefault(a => a.Name == segment.Name && a.Segment == segment.Segment && a.TargetGpu == segment.TargetGpu));
                    InputSegments = InputSegments.ToList();
                }));
            });
            this.RemoveFileWriter = new DelegateCommand<FileWriterViewModel>((writer) => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除文件书写器{writer.Name}吗？", title: "确认", onYes: () => {
                    this.FileWriterVms.Remove(writer);
                    List<Guid> writerIds = new List<Guid>(this.FileWriterIds);
                    writerIds.Remove(writer.Id);
                    this.FileWriterIds = writerIds;
                }));
            });
            this.RemoveFragmentWriter = new DelegateCommand<FragmentWriterViewModel>((writer) => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除文件书写器{writer.Name}吗？", title: "确认", onYes: () => {
                    this.FragmentWriterVms.Remove(writer);
                    List<Guid> writerIds = new List<Guid>(this.FragmentWriterIds);
                    writerIds.Remove(writer.Id);
                    this.FragmentWriterIds = writerIds;
                }));
            });
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Instance.ServerContext.CoinKernelSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateCoinKernelCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new CoinKernelEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{Kernel.Code}币种内核吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveCoinKernelCommand(this.Id));
                    Kernel.OnPropertyChanged(nameof(Kernel.SupportedCoins));
                }));
            });
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

        public Guid CoinId {
            get {
                return _coinId;
            }
            set {
                if (_coinId != value) {
                    _coinId = value;
                    OnPropertyChanged(nameof(CoinId));
                    OnPropertyChanged(nameof(CoinCode));
                }
            }
        }

        public string CoinCode {
            get {
                return CoinVm.Code;
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (_coinVm == null || this.CoinId != _coinVm.Id) {
                    AppContext.Instance.CoinVms.TryGetCoinVm(this.CoinId, out _coinVm);
                    if (_coinVm == null) {
                        _coinVm = CoinViewModel.Empty;
                    }
                }
                return _coinVm;
            }
        }

        public Guid KernelId {
            get => _kernelId;
            set {
                if (_kernelId != value) {
                    _kernelId = value;
                    OnPropertyChanged(nameof(KernelId));
                }
            }
        }

        public string DisplayName {
            get {
                return $"{Kernel.Code}{Kernel.Version}";
            }
        }

        public KernelViewModel Kernel {
            get {
                if (AppContext.Instance.KernelVms.TryGetKernelVm(this.KernelId, out KernelViewModel kernel)) {
                    return kernel;
                }
                return KernelViewModel.Empty;
            }
        }

        public Guid DualCoinGroupId {
            get => _dualCoinGroupId;
            set {
                if (_dualCoinGroupId != value) {
                    _dualCoinGroupId = value;
                    OnPropertyChanged(nameof(DualCoinGroupId));
                    OnPropertyChanged(nameof(SelectedDualCoinGroup));
                    OnPropertyChanged(nameof(IsSupportDualMine));
                }
            }
        }

        public GroupViewModel SelectedDualCoinGroup {
            get {
                if (this.DualCoinGroupId == Guid.Empty) {
                    return GroupViewModel.PleaseSelect;
                }
                if (_selectedDualCoinGroup == null || _selectedDualCoinGroup.Id != this.DualCoinGroupId) {
                    AppContext.Instance.GroupVms.TryGetGroupVm(DualCoinGroupId, out _selectedDualCoinGroup);
                    if (_selectedDualCoinGroup == null) {
                        _selectedDualCoinGroup = GroupViewModel.PleaseSelect;
                    }
                }
                return _selectedDualCoinGroup;
            }
            set {
                if (DualCoinGroupId != value.Id) {
                    DualCoinGroupId = value.Id;
                }
            }
        }

        public AppContext.GroupViewModels GroupVms {
            get {
                return AppContext.Instance.GroupVms;
            }
        }

        public string Args {
            get { return _args; }
            set {
                if (_args != value) {
                    _args = value;
                    OnPropertyChanged(nameof(Args));
                }
            }
        }

        public string DualFullArgs {
            get { return _dualFullArgs; }
            set {
                _dualFullArgs = value;
                OnPropertyChanged(nameof(DualFullArgs));
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

        public bool IsHot {
            get { return _isHot; }
            set {
                _isHot = value;
                OnPropertyChanged(nameof(IsHot));
            }
        }

        public bool IsRecommend {
            get { return _isRecommend; }
            set {
                _isRecommend = value;
                OnPropertyChanged(nameof(IsRecommend));
            }
        }

        public List<EnvironmentVariable> EnvironmentVariables {
            get => _environmentVariables;
            set {
                _environmentVariables = value;
                OnPropertyChanged(nameof(EnvironmentVariables));
            }
        }

        public List<InputSegment> InputSegments {
            get => _inputSegments;
            set {
                _inputSegments = value;
                OnPropertyChanged(nameof(InputSegments));
                if (value != null) {
                    this.InputSegmentVms = value.Select(a => new InputSegmentViewModel(a)).ToList();
                }
                else {
                    this.InputSegmentVms = new List<InputSegmentViewModel>();
                }
            }
        }

        public List<InputSegmentViewModel> InputSegmentVms {
            get {
                return _inputSegmentVms;
            }
            set {
                _inputSegmentVms = value;
                _gpuInputSegmentVms = _inputSegmentVms.Where(a => a.TargetGpu.IsSupportedGpu(NTMinerRoot.Instance.GpuSet.GpuType)).ToList();
                OnPropertyChanged(nameof(InputSegmentVms));
                OnPropertyChanged(nameof(GpuInputSegmentVms));
            }
        }

        public List<InputSegmentViewModel> GpuInputSegmentVms {
            get {
                return _gpuInputSegmentVms;
            }
        }

        public List<Guid> FileWriterIds {
            get => _fileWriterIds;
            set {
                _fileWriterIds = value;
                OnPropertyChanged(nameof(FileWriterIds));
                if (value != null) {
                    this.FileWriterVms = AppContext.Instance.FileWriterVms.List.Where(a => value.Contains(a.Id)).ToList();
                }
                else {
                    this.FileWriterVms = new List<FileWriterViewModel>();
                }
            }
        }

        public List<FileWriterViewModel> FileWriterVms {
            get {
                return _fileWriterVms;
            }
            set {
                _fileWriterVms = value;
                OnPropertyChanged(nameof(FileWriterVms));
            }
        }

        public List<Guid> FragmentWriterIds {
            get {
                return _fragmentWriterIds;
            }
            set {
                _fragmentWriterIds = value;
                OnPropertyChanged(nameof(FragmentWriterIds));
                if (value != null) {
                    this.FragmentWriterVms = AppContext.Instance.FragmentWriterVms.List.Where(a => value.Contains(a.Id)).ToList();
                }
                else {
                    this.FragmentWriterVms = new List<FragmentWriterViewModel>();
                }
            }
        }

        public List<FragmentWriterViewModel> FragmentWriterVms {
            get {
                return _fragmentWriterVms;
            }
            set {
                _fragmentWriterVms = value;
                OnPropertyChanged(nameof(FragmentWriterVms));
            }
        }

        public bool IsSupportDualMine {
            get {
                return this.GetIsSupportDualMine();
            }
        }

        public bool IsSupportPool1 {
            get { return _isSupportPool1; }
            set {
                if (_isSupportPool1 != value) {
                    _isSupportPool1 = value;
                    OnPropertyChanged(nameof(IsSupportPool1));
                    AppContext.Instance.MinerProfileVm.OnPropertyChanged(nameof(AppContext.Instance.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
                }
            }
        }

        public SupportedGpu SupportedGpu {
            get => _supportedGpu;
            set {
                if (_supportedGpu != value) {
                    _supportedGpu = value;
                    OnPropertyChanged(nameof(SupportedGpu));
                    OnPropertyChanged(nameof(IsNvidiaIconVisible));
                    OnPropertyChanged(nameof(IsAMDIconVisible));
                    OnPropertyChanged(nameof(IsSupported));
                    OnPropertyChanged(nameof(SupportedGpuEnumItem));
                }
            }
        }

        public bool IsSupported {
            get {
                return this.SupportedGpu.IsSupportedGpu(NTMinerRoot.Instance.GpuSet.GpuType);
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                if (SupportedGpu.IsSupportedGpu(GpuType.NVIDIA)) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAMDIconVisible {
            get {
                if (SupportedGpu.IsSupportedGpu(GpuType.AMD)) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public EnumItem<SupportedGpu> SupportedGpuEnumItem {
            get {
                return NTMinerRoot.SupportedGpuEnumItems.FirstOrDefault(a => a.Value == SupportedGpu);
            }
            set {
                if (SupportedGpu != value.Value) {
                    SupportedGpu = value.Value;
                }
            }
        }

        public CoinKernelProfileViewModel CoinKernelProfile {
            get {
                return AppContext.Instance.CoinProfileVms.GetOrCreateCoinKernelProfileVm(this.Id);
            }
        }
    }
}
