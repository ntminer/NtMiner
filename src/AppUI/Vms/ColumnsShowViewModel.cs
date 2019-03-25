using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Reflection;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModel : ViewModelBase, IColumnsShow, IEditableViewModel {
        private Guid _id;
        private string _columnsShowName;
        private bool _work;
        private bool _minerName;
        private bool _clientName;
        private bool _minerIp;
        private bool _minerGroup;
        private bool _mainCoinCode;
        private bool _mainCoinSpeedText;
        private bool _gpuTableVms;
        private bool _mainCoinWallet;
        private bool _mainCoinPool;
        private bool _kernel;
        private bool _dualCoinCode;
        private bool _dualCoinSpeedText;
        private bool _dualCoinWallet;
        private bool _dualCoinPool;
        private bool _lastActivedOnText;
        private bool _version;
        private bool _windowsLoginNameAndPassword;
        private bool _gpuInfo;
        private bool _mainCoinRejectPercentText;
        private bool _dualCoinRejectPercentText;
        private bool _bootTimeSpanText;
        private bool _mineTimeSpanText;
        private bool _incomeMainCoinPerDayText;
        private bool _incomeDualCoinPerDayText;
        private bool _isAutoBoot;
        private bool _isAutoStart;
        private bool _oSName;
        private bool _oSVirtualMemoryGbText;
        private bool _gpuType;
        private bool _gpuDriver;
        private bool _totalPowerText;
        private bool _maxTempText;
        private bool _kernelCommandLine;
        private bool _diskSpace;
        private bool _isAutoRestartKernel;
        private bool _isNoShareRestartKernel;
        private bool _isPeriodicRestartKernel;
        private bool _isPeriodicRestartComputer;

        public ICommand Hide { get; private set; }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public ColumnsShowViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public ColumnsShowViewModel(Guid id) {
            this.Id = id;
            this.Hide = new DelegateCommand<string>((propertyName) => {
                PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
                if (propertyInfo != null) {
                    propertyInfo.SetValue(this, false, null);
                }
            });
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.ColumnsShowSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateColumnsShowCommand(this));
                    NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage($"保存成功");
                }
                else {
                    VirtualRoot.Execute(new AddColumnsShowCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                ColumnsShowEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    DialogWindow.ShowDialog(message: "该项不能删除", title: "警告", icon: "Icon_Error");
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.ColumnsShowName}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveColumnsShowCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
        }

        public ColumnsShowViewModel(IColumnsShow data) : this(data.GetId()) {
            _columnsShowName = data.ColumnsShowName;
            _work = data.Work;
            _minerName = data.MinerName;
            _clientName = data.ClientName;
            _minerIp = data.MinerIp;
            _minerGroup = data.MinerGroup;
            _mainCoinCode = data.MainCoinCode;
            _mainCoinSpeedText = data.MainCoinSpeedText;
            _gpuTableVms = data.GpuTableVm;
            _mainCoinWallet = data.MainCoinWallet;
            _mainCoinPool = data.MainCoinPool;
            _kernel = data.Kernel;
            _dualCoinCode = data.DualCoinCode;
            _dualCoinSpeedText = data.DualCoinSpeedText;
            _dualCoinWallet = data.DualCoinWallet;
            _dualCoinPool = data.DualCoinPool;
            _lastActivedOnText = data.LastActivedOnText;
            _version = data.Version;
            _windowsLoginNameAndPassword = data.WindowsLoginNameAndPassword;
            _gpuInfo = data.GpuInfo;
            _mainCoinRejectPercentText = data.MainCoinRejectPercentText;
            _dualCoinRejectPercentText = data.DualCoinRejectPercentText;
            _bootTimeSpanText = data.BootTimeSpanText;
            _mineTimeSpanText = data.MineTimeSpanText;
            _incomeMainCoinPerDayText = data.IncomeMainCoinPerDayText;
            _incomeDualCoinPerDayText = data.IncomeDualCoinPerDayText;
            _isAutoBoot = data.IsAutoBoot;
            _isAutoStart = data.IsAutoStart;
            _oSName = data.OSName;
            _oSVirtualMemoryGbText = data.OSVirtualMemoryGbText;
            _gpuType = data.GpuType;
            _gpuDriver = data.GpuDriver;
            _totalPowerText = data.TotalPowerText;
            _maxTempText = data.MaxTempText;
            _kernelCommandLine = data.KernelCommandLine;
            _diskSpace = data.DiskSpace;
            _isAutoRestartKernel = data.IsAutoRestartKernel;
            _isNoShareRestartKernel = data.IsNoShareRestartKernel;
            _isPeriodicRestartKernel = data.IsPeriodicRestartKernel;
            _isPeriodicRestartComputer = data.IsPeriodicRestartComputer;
        }

        public bool IsPleaseSelect {
            get {
                return this.Id == ColumnsShowData.PleaseSelect.Id;
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string ColumnsShowName {
            get { return _columnsShowName; }
            set {
                if (_columnsShowName != value) {
                    _columnsShowName = value;
                    OnPropertyChanged(nameof(ColumnsShowName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool BootTimeSpanText {
            get { return _bootTimeSpanText; }
            set {
                if (_bootTimeSpanText != value) {
                    _bootTimeSpanText = value;
                    OnPropertyChanged(nameof(BootTimeSpanText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MineTimeSpanText {
            get { return _mineTimeSpanText; }
            set {
                if (_mineTimeSpanText != value) {
                    _mineTimeSpanText = value;
                    OnPropertyChanged(nameof(MineTimeSpanText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool Work {
            get => _work;
            set {
                if (_work != value) {
                    _work = value;
                    OnPropertyChanged(nameof(Work));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MinerName {
            get { return _minerName; }
            set {
                if (_minerName != value) {
                    _minerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool ClientName {
            get { return _clientName; }
            set {
                if (_clientName != value) {
                    _clientName = value;
                    OnPropertyChanged(nameof(ClientName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MinerIp {
            get { return _minerIp; }
            set {
                if (_minerIp != value) {
                    _minerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MinerGroup {
            get { return _minerGroup; }
            set {
                if (_minerGroup != value) {
                    _minerGroup = value;
                    OnPropertyChanged(nameof(MinerGroup));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MainCoinCode {
            get {
                return _mainCoinCode;
            }
            set {
                if (_mainCoinCode != value) {
                    _mainCoinCode = value;
                    OnPropertyChanged(nameof(MainCoinCode));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MainCoinSpeedText {
            get { return _mainCoinSpeedText; }
            set {
                if (_mainCoinSpeedText != value) {
                    _mainCoinSpeedText = value;
                    OnPropertyChanged(nameof(MainCoinSpeedText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MainCoinRejectPercentText {
            get => _mainCoinRejectPercentText;
            set {
                if (_mainCoinRejectPercentText != value) {
                    _mainCoinRejectPercentText = value;
                    OnPropertyChanged(nameof(MainCoinRejectPercentText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool GpuTableVm {
            get { return _gpuTableVms; }
            set {
                if (_gpuTableVms != value) {
                    _gpuTableVms = value;
                    OnPropertyChanged(nameof(GpuTableVm));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool TotalPowerText {
            get => _totalPowerText;
            set {
                if (_totalPowerText != value) {
                    _totalPowerText = value;
                    OnPropertyChanged(nameof(TotalPowerText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MaxTempText {
            get => _maxTempText;
            set {
                if (_maxTempText != value) {
                    _maxTempText = value;
                    OnPropertyChanged(nameof(MaxTempText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MainCoinWallet {
            get => _mainCoinWallet;
            set {
                if (_mainCoinWallet != value) {
                    _mainCoinWallet = value;
                    OnPropertyChanged(nameof(MainCoinWallet));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool MainCoinPool {
            get => _mainCoinPool;
            set {
                if (_mainCoinPool != value) {
                    _mainCoinPool = value;
                    OnPropertyChanged(nameof(MainCoinPool));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool Kernel {
            get => _kernel;
            set {
                if (_kernel != value) {
                    _kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                    UpdateColumnsShowAsync();
                }
            }
        }
        public bool DualCoinCode {
            get => _dualCoinCode;
            set {
                if (_dualCoinCode != value) {
                    _dualCoinCode = value;
                    OnPropertyChanged(nameof(DualCoinCode));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool DualCoinSpeedText {
            get => _dualCoinSpeedText;
            set {
                if (_dualCoinSpeedText != value) {
                    _dualCoinSpeedText = value;
                    OnPropertyChanged(nameof(DualCoinSpeedText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool DualCoinRejectPercentText {
            get => _dualCoinRejectPercentText;
            set {
                if (_dualCoinRejectPercentText != value) {
                    _dualCoinRejectPercentText = value;
                    OnPropertyChanged(nameof(DualCoinRejectPercentText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IncomeMainCoinPerDayText {
            get => _incomeMainCoinPerDayText;
            set {
                if (_incomeMainCoinPerDayText != value) {
                    _incomeMainCoinPerDayText = value;
                    OnPropertyChanged(nameof(IncomeMainCoinPerDayText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IncomeDualCoinPerDayText {
            get => _incomeDualCoinPerDayText;
            set {
                if (_incomeDualCoinPerDayText != value) {
                    _incomeDualCoinPerDayText = value;
                    OnPropertyChanged(nameof(IncomeDualCoinPerDayText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool DualCoinWallet {
            get => _dualCoinWallet;
            set {
                if (_dualCoinWallet != value) {
                    _dualCoinWallet = value;
                    OnPropertyChanged(nameof(DualCoinWallet));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool DualCoinPool {
            get => _dualCoinPool;
            set {
                if (_dualCoinPool != value) {
                    _dualCoinPool = value;
                    OnPropertyChanged(nameof(DualCoinPool));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool LastActivedOnText {
            get => _lastActivedOnText;
            set {
                if (_lastActivedOnText != value) {
                    _lastActivedOnText = value;
                    OnPropertyChanged(nameof(LastActivedOnText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool Version {
            get => _version;
            set {
                if (_version != value) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsAutoBoot {
            get => _isAutoBoot;
            set {
                if (_isAutoBoot != value) {
                    _isAutoBoot = value;
                    OnPropertyChanged(nameof(IsAutoBoot));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsAutoStart {
            get => _isAutoStart;
            set {
                if (_isAutoStart != value) {
                    _isAutoStart = value;
                    OnPropertyChanged(nameof(IsAutoStart));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool WindowsLoginNameAndPassword {
            get => _windowsLoginNameAndPassword;
            set {
                if (_windowsLoginNameAndPassword != value) {
                    _windowsLoginNameAndPassword = value;
                    OnPropertyChanged(nameof(WindowsLoginNameAndPassword));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool OSName {
            get => _oSName;
            set {
                if (_oSName != value) {
                    _oSName = value;
                    OnPropertyChanged(nameof(OSName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool OSVirtualMemoryGbText {
            get => _oSVirtualMemoryGbText;
            set {
                if (_oSVirtualMemoryGbText != value) {
                    _oSVirtualMemoryGbText = value;
                    OnPropertyChanged(nameof(OSVirtualMemoryGbText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool GpuDriver {
            get => _gpuDriver;
            set {
                if (_gpuDriver != value) {
                    _gpuDriver = value;
                    OnPropertyChanged(nameof(GpuDriver));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool KernelCommandLine {
            get => _kernelCommandLine;
            set {
                if (_kernelCommandLine != value) {
                    _kernelCommandLine = value;
                    OnPropertyChanged(nameof(KernelCommandLine));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool GpuInfo {
            get => _gpuInfo;
            set {
                if (_gpuInfo != value) {
                    _gpuInfo = value;
                    OnPropertyChanged(nameof(GpuInfo));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool DiskSpace {
            get => _diskSpace;
            set {
                if (_diskSpace != value) {
                    _diskSpace = value;
                    OnPropertyChanged(nameof(DiskSpace));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => _isAutoRestartKernel;
            set {
                if (_isAutoRestartKernel != value) {
                    _isAutoRestartKernel = value;
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsNoShareRestartKernel {
            get => _isNoShareRestartKernel;
            set {
                if (_isNoShareRestartKernel != value) {
                    _isNoShareRestartKernel = value;
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => _isPeriodicRestartKernel;
            set {
                if (_isPeriodicRestartKernel != value) {
                    _isPeriodicRestartKernel = value;
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                    UpdateColumnsShowAsync();
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => _isPeriodicRestartComputer;
            set {
                if (_isPeriodicRestartComputer != value) {
                    _isPeriodicRestartComputer = value;
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                    UpdateColumnsShowAsync();
                }
            }
        }

        private void UpdateColumnsShowAsync() {
            Server.ControlCenterService.AddOrUpdateColumnsShowAsync(new ColumnsShowData().Update(this), (response, exception) => {
                if (!response.IsSuccess()) {
                    if (response != null) {
                        Write.UserLine(response.Description, ConsoleColor.Red);
                    }
                }
            });
        }
    }
}
