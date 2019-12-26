using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputViewModel : ViewModelBase, IKernelOutput, IEditableViewModel {
        public static readonly KernelOutputViewModel PleaseSelect = new KernelOutputViewModel(Guid.Empty) {
            _name = "请选择"
        };

        private Guid _id;
        private string _name;
        private string _kernelRestartKeyword;
        private bool _isDualInSameLine;
        private string _totalSpeedPattern;
        private string _totalSharePattern;
        private string _acceptSharePattern;
        private string _acceptOneShare;
        private string _foundOneShare;
        private string _gpuSpeedPattern;
        private string _gpuGotOneIncorrectShare;
        private string _rejectOneShare;
        private string _rejectSharePattern;
        private string _rejectPercentPattern;

        private string _dualTotalSpeedPattern;
        private string _dualTotalSharePattern;
        private string _dualAcceptSharePattern;
        private string _dualAcceptOneShare;
        private string _dualGpuSpeedPattern;
        private string _dualRejectSharePattern;
        private string _dualRejectOneShare;
        private string _dualRejectPercentPattern;

        private string _translaterKeyword;
        private string _poolDelayPattern;
        private string _dualPoolDelayPattern;
        private string _speedUnit;
        private string _dualSpeedUnit;
        private int _gpuBaseIndex;
        private bool _isMapGpuIndex;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public ICommand AddKernelOutputTranslater { get; private set; }

        public ICommand ClearTranslaterKeyword { get; private set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public KernelOutputViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelOutputViewModel(IKernelOutput data) : this(data.GetId()) {
            _name = data.Name;
            _kernelRestartKeyword = data.KernelRestartKeyword;
            _isDualInSameLine = data.IsDualInSameLine;
            _totalSpeedPattern = data.TotalSpeedPattern;
            _gpuSpeedPattern = data.GpuSpeedPattern;
            _gpuGotOneIncorrectShare = data.GpuGotOneIncorrectShare;
            _totalSharePattern = data.TotalSharePattern;
            _acceptSharePattern = data.AcceptSharePattern;
            _foundOneShare = data.FoundOneShare;
            _acceptOneShare = data.AcceptOneShare;
            _rejectSharePattern = data.RejectSharePattern;
            _rejectOneShare = data.RejectOneShare;
            _rejectPercentPattern = data.RejectPercentPattern;
            _dualGpuSpeedPattern = data.DualGpuSpeedPattern;
            _dualAcceptSharePattern = data.DualAcceptSharePattern;
            _dualAcceptOneShare = data.DualAcceptOneShare;
            _dualRejectSharePattern = data.DualRejectSharePattern;
            _dualRejectOneShare = data.DualRejectOneShare;
            _dualRejectPercentPattern = data.DualRejectPercentPattern;
            _dualTotalSharePattern = data.DualTotalSharePattern;
            _dualTotalSpeedPattern = data.DualTotalSpeedPattern;
            _poolDelayPattern = data.PoolDelayPattern;
            _dualPoolDelayPattern = data.DualPoolDelayPattern;
            _speedUnit = data.SpeedUnit;
            _dualSpeedUnit = data.DualSpeedUnit;
            _gpuBaseIndex = data.GpuBaseIndex;
            _isMapGpuIndex = data.IsMapGpuIndex;
        }

        public KernelOutputViewModel(Guid id) {
            this._id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Instance.ServerContext.KernelOutputSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelOutputCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelOutputCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new KernelOutputEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}内核输出吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelOutputCommand(this.Id));
                }));
            });
            this.AddKernelOutputTranslater = new DelegateCommand(() => {
                int sortNumber = this.KernelOutputTranslaters.Count == 0 ? 1 : this.KernelOutputTranslaters.Count + 1;
                new KernelOutputTranslaterViewModel(Guid.NewGuid()) {
                    KernelOutputId = this.Id,
                    SortNumber = sortNumber
                }.Edit.Execute(FormType.Add);
            });
            this.ClearTranslaterKeyword = new DelegateCommand(() => {
                this.TranslaterKeyword = string.Empty;
            });
        }

        public List<KernelOutputKeywordViewModel> KernelOutputKeywords {
            get {
                return new List<KernelOutputKeywordViewModel>(AppContext.Instance.KernelOutputKeywordVms.GetListByKernelId(this.Id).OrderBy(a => a.Keyword));
            }
        }

        public string TranslaterKeyword {
            get { return _translaterKeyword; }
            set {
                if (_translaterKeyword != value) {
                    _translaterKeyword = value;
                    OnPropertyChanged(nameof(TranslaterKeyword));
                    OnPropertyChanged(nameof(KernelOutputTranslaters));
                }
            }
        }

        public List<KernelOutputTranslaterViewModel> KernelOutputTranslaters {
            get {
                var query = AppContext.Instance.KernelOutputTranslaterVms.GetListByKernelId(this.Id).AsQueryable();
                if (!string.IsNullOrEmpty(TranslaterKeyword)) {
                    query = query.Where(a => (a.RegexPattern != null && a.RegexPattern.Contains(TranslaterKeyword))
                        || (a.Replacement != null && a.Replacement.Contains(TranslaterKeyword)));
                }
                return query.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public string GroupNames {
            get {
                return string.Join("、", new string[] {
                    NTKeyword.TotalSpeedGroupName, NTKeyword.TotalSpeedUnitGroupName,
                    NTKeyword.TotalShareGroupName, NTKeyword.AcceptShareGroupName,
                    NTKeyword.RejectShareGroupName, NTKeyword.RejectPercentGroupName,
                    NTKeyword.GpuIndexGroupName, NTKeyword.GpuSpeedGroupName,
                    NTKeyword.GpuSpeedUnitGroupName, NTKeyword.PoolDelayGroupName
                });
            }
        }

        public Guid GetId() {
            return this.Id;
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

        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                }
            }
        }

        public string KernelRestartKeyword {
            get { return _kernelRestartKeyword; }
            set {
                _kernelRestartKeyword = value;
                OnPropertyChanged(nameof(KernelRestartKeyword));
            }
        }

        public bool IsDualInSameLine {
            get { return _isDualInSameLine; }
            set {
                _isDualInSameLine = value;
                OnPropertyChanged(nameof(IsDualInSameLine));
            }
        }

        public string TotalSpeedPattern {
            get => _totalSpeedPattern;
            set {
                if (_totalSpeedPattern != value) {
                    _totalSpeedPattern = value;
                    OnPropertyChanged(nameof(TotalSpeedPattern));
                }
            }
        }

        public string TotalSharePattern {
            get { return _totalSharePattern; }
            set {
                if (_totalSharePattern != value) {
                    _totalSharePattern = value;
                    OnPropertyChanged(nameof(TotalSharePattern));
                }
            }
        }

        public string AcceptSharePattern {
            get { return _acceptSharePattern; }
            set {
                if (_acceptSharePattern != value) {
                    _acceptSharePattern = value;
                    OnPropertyChanged(nameof(AcceptSharePattern));
                }
            }
        }

        public string FoundOneShare {
            get { return _foundOneShare; }
            set {
                if (_foundOneShare != value) {
                    _foundOneShare = value;
                    OnPropertyChanged(nameof(FoundOneShare));
                    OnPropertyChanged(nameof(IsFoundOneGpuShare));
                }
            }
        }

        public bool IsFoundOneGpuShare {
            get {
                if (!string.IsNullOrEmpty(FoundOneShare) && FoundOneShare.Contains("?<gpu>")) {
                    return true;
                }
                if (!string.IsNullOrEmpty(AcceptOneShare) && AcceptOneShare.Contains("?<gpu>")) {
                    return true;
                }
                return false;
            }
        }

        public string AcceptOneShare {
            get { return _acceptOneShare; }
            set {
                if (_acceptOneShare != value) {
                    _acceptOneShare = value;
                    OnPropertyChanged(nameof(AcceptOneShare));
                    OnPropertyChanged(nameof(IsFoundOneGpuShare));
                }
            }
        }

        public string RejectSharePattern {
            get { return _rejectSharePattern; }
            set {
                if (_rejectSharePattern != value) {
                    _rejectSharePattern = value;
                    OnPropertyChanged(nameof(RejectSharePattern));
                }
            }
        }

        public string RejectOneShare {
            get { return _rejectOneShare; }
            set {
                if (_rejectOneShare != value) {
                    _rejectOneShare = value;
                    OnPropertyChanged(nameof(RejectOneShare));
                    OnPropertyChanged(nameof(IsRejectOneGpuShare));
                }
            }
        }

        public bool IsRejectOneGpuShare {
            get {
                if (string.IsNullOrEmpty(RejectOneShare)) {
                    return false;
                }
                return RejectOneShare.Contains("?<gpu>");
            }
        }

        public bool IsGotOneIncorrectGpuShare {
            get {
                if (string.IsNullOrEmpty(GpuGotOneIncorrectShare)) {
                    return false;
                }
                return GpuGotOneIncorrectShare.Contains("?<gpu>");
            }
        }

        public string RejectPercentPattern {
            get { return _rejectPercentPattern; }
            set {
                if (_rejectPercentPattern != value) {
                    _rejectPercentPattern = value;
                    OnPropertyChanged(nameof(RejectPercentPattern));
                }
            }
        }

        public string GpuSpeedPattern {
            get => _gpuSpeedPattern;
            set {
                if (_gpuSpeedPattern != value) {
                    _gpuSpeedPattern = value;
                    OnPropertyChanged(nameof(GpuSpeedPattern));
                }
            }
        }

        public string GpuGotOneIncorrectShare {
            get { return _gpuGotOneIncorrectShare; }
            set {
                if (_gpuGotOneIncorrectShare != value) {
                    _gpuGotOneIncorrectShare = value;
                    OnPropertyChanged(nameof(GpuGotOneIncorrectShare));
                    OnPropertyChanged(nameof(IsGotOneIncorrectGpuShare));
                }
            }
        }

        public string DualTotalSpeedPattern {
            get => _dualTotalSpeedPattern;
            set {
                if (_dualTotalSpeedPattern != value) {
                    _dualTotalSpeedPattern = value;
                    OnPropertyChanged(nameof(DualTotalSpeedPattern));
                }
            }
        }

        public string DualTotalSharePattern {
            get { return _dualTotalSharePattern; }
            set {
                if (_dualTotalSharePattern != value) {
                    _dualTotalSharePattern = value;
                    OnPropertyChanged(nameof(DualTotalSharePattern));
                }
            }
        }

        public string DualAcceptOneShare {
            get { return _dualAcceptOneShare; }
            set {
                if (_dualAcceptOneShare != value) {
                    _dualAcceptOneShare = value;
                    OnPropertyChanged(nameof(DualAcceptOneShare));
                }
            }
        }

        public string DualAcceptSharePattern {
            get { return _dualAcceptSharePattern; }
            set {
                if (_dualAcceptSharePattern != value) {
                    _dualAcceptSharePattern = value;
                    OnPropertyChanged(nameof(DualAcceptSharePattern));
                }
            }
        }

        public string DualRejectSharePattern {
            get { return _dualRejectSharePattern; }
            set {
                if (_dualRejectSharePattern != value) {
                    _dualRejectSharePattern = value;
                    OnPropertyChanged(nameof(DualRejectSharePattern));
                }
            }
        }

        public string DualRejectOneShare {
            get { return _dualRejectOneShare; }
            set {
                if (_dualRejectOneShare != value) {
                    _dualRejectOneShare = value;
                    OnPropertyChanged(nameof(DualRejectOneShare));
                }
            }
        }

        public bool IsDualRejectOneGpuShare {
            get {
                if (string.IsNullOrEmpty(DualRejectOneShare)) {
                    return false;
                }
                return DualRejectOneShare.Contains("?<gpu>");
            }
        }

        public string DualRejectPercentPattern {
            get { return _dualRejectPercentPattern; }
            set {
                if (_dualRejectPercentPattern != value) {
                    _dualRejectPercentPattern = value;
                    OnPropertyChanged(nameof(DualRejectPercentPattern));
                }
            }
        }

        public string DualGpuSpeedPattern {
            get => _dualGpuSpeedPattern;
            set {
                if (_dualGpuSpeedPattern != value) {
                    _dualGpuSpeedPattern = value;
                    OnPropertyChanged(nameof(DualGpuSpeedPattern));
                }
            }
        }

        public string PoolDelayPattern {
            get => _poolDelayPattern;
            set {
                if (_poolDelayPattern != value) {
                    _poolDelayPattern = value;
                    OnPropertyChanged(nameof(PoolDelayPattern));
                }
            }
        }

        public string DualPoolDelayPattern {
            get => _dualPoolDelayPattern;
            set {
                if (_dualPoolDelayPattern != value) {
                    _dualPoolDelayPattern = value;
                    OnPropertyChanged(nameof(DualPoolDelayPattern));
                }
            }
        }

        public string SpeedUnit {
            get => _speedUnit;
            set {
                _speedUnit = value;
                OnPropertyChanged(nameof(SpeedUnit));
            }
        }

        public string DualSpeedUnit {
            get => _dualSpeedUnit;
            set {
                _dualSpeedUnit = value;
                OnPropertyChanged(nameof(DualSpeedUnit));
            }
        }

        public int GpuBaseIndex {
            get => _gpuBaseIndex;
            set {
                _gpuBaseIndex = value;
                OnPropertyChanged(nameof(GpuBaseIndex));
            }
        }

        public bool IsMapGpuIndex {
            get { return _isMapGpuIndex; }
            set {
                _isMapGpuIndex = value;
                OnPropertyChanged(nameof(IsMapGpuIndex));
            }
        }


        public string KernelFullNames {
            get {
                string names = string.Join(";", AppContext.Instance.KernelVms.AllKernels
                    .Where(a => a.KernelOutputId == this.Id)
                    .OrderBy(a => a.Code)
                    .ThenByDescending(a => a.Version)
                    .Select(a => a.FullName));
                if (string.IsNullOrEmpty(names)) {
                    return "无";
                }
                return names;
            }
        }
    }
}
