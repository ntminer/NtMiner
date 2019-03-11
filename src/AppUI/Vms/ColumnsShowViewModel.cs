using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Notifications;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Reflection;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModel : ViewModelBase, IColumnsShow, IEditableViewModel {
        private Guid _id;
        private string _columnsShowName;
        private bool _work = true;
        private bool _minerName = true;
        private bool _minerIp = true;
        private bool _minerGroup = true;
        private bool _mainCoinCode = true;
        private bool _mainCoinSpeedText = true;
        private bool _gpuTableTrs = true;
        private bool _mainCoinWallet = true;
        private bool _mainCoinPool = true;
        private bool _kernel = true;
        private bool _dualCoinCode = true;
        private bool _dualCoinSpeedText = true;
        private bool _dualCoinWallet = true;
        private bool _dualCoinPool = true;
        private bool _lastActivedOnText = true;
        private bool _version = true;
        private bool _windowsLoginNameAndPassword = true;
        private bool _gpuInfo = true;
        private bool _mainCoinRejectPercentText = true;
        private bool _dualCoinRejectPercentText = true;
        private bool _bootTimeSpanText = true;
        private bool _mineTimeSpanText = true;
        private bool _incomeMainCoinPerDayText = true;
        private bool _incomeDualCoinPerDayText = true;
        private bool _isAutoBoot = true;
        private bool _isAutoStart = true;
        private bool _oSName = true;
        private bool _oSVirtualMemoryGbText = true;
        private bool _gpuType = true;
        private bool _gpuDriver = true;
        private bool _totalPowerText = true;
        private bool _maxTempText = true;
        private bool _kernelCommandLine = true;

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
                    MinerClientsWindowViewModel.Current.Manager.ShowSuccessMessage($"保存成功");
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
                }, icon: "Icon_Confirm");
            });
        }

        public ColumnsShowViewModel(IColumnsShow data) : this(data.GetId()) {
            _columnsShowName = data.ColumnsShowName;
            _work = data.Work;
            _minerName = data.MinerName;
            _minerIp = data.MinerIp;
            _minerGroup = data.MinerGroup;
            _mainCoinCode = data.MainCoinCode;
            _mainCoinSpeedText = data.MainCoinSpeedText;
            _gpuTableTrs = data.GpuTableTrs;
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
        }

        public bool IsPleaseSelect {
            get {
                return this.Id == ColumnsShowData.PleaseSelectId;
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

        public bool GpuTableTrs {
            get { return _gpuTableTrs; }
            set {
                if (_gpuTableTrs != value) {
                    _gpuTableTrs = value;
                    OnPropertyChanged(nameof(GpuTableTrs));
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
