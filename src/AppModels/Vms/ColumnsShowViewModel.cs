using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModel : ViewModelBase, IColumnsShow, IEditableViewModel {
        public class ColumnItem : ViewModelBase {
            private readonly ColumnsShowViewModel _vm;

            public ColumnItem(PropertyInfo propertyInfo, ColumnsShowViewModel vm) {
                this.PropertyInfo = propertyInfo;
                this._vm = vm;
            }

            public PropertyInfo PropertyInfo {
                get; private set;
            }

            public string Name {
                get {
                    return ((DescriptionAttribute)(this.PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0])).Description;
                }
            }

            public bool IsChecked {
                get {
                    return (bool)this.PropertyInfo.GetValue(_vm, null);
                }
                set {
                    this.PropertyInfo.SetValue(_vm, value, null);
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

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
        private bool _isNoShareRestartComputer;
        private bool _isPeriodicRestartKernel;
        private bool _isPeriodicRestartComputer;

        private List<ColumnItem> _columnItems = null;
        public List<ColumnItem> ColumnItems {
            get {
                if (_columnItems == null) {
                    Type boolType = typeof(bool);
                    var properties = new List<PropertyInfo>(typeof(ColumnsShowViewModel).GetProperties().Where(a => a.PropertyType == boolType && a.CanRead && a.CanWrite && a.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false).Length != 0));
                    _columnItems = new List<ColumnItem>(properties.Select(a => new ColumnItem(a, this) {
                        IsChecked = (bool)a.GetValue(this, null)
                    }));
                }
                return _columnItems;
            }
        }

        private void OnColumnItemChanged(string propertyName) {
            ColumnItem item = this.ColumnItems.FirstOrDefault(a => a.PropertyInfo.Name == propertyName);
            if (item != null) {
                item.OnPropertyChanged(nameof(item.IsChecked));
            }
        }

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
                if (NTMinerRoot.Instance.ColumnsShowSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateColumnsShowCommand(this));
                    NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"保存成功");
                }
                else {
                    VirtualRoot.Execute(new AddColumnsShowCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new ColumnsShowEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    this.ShowDialog(message: "该项不能删除", title: "警告", icon: "Icon_Error");
                    return;
                }
                this.ShowDialog(message: $"您确定删除{this.ColumnsShowName}吗？", title: "确认", onYes: () => {
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
            _isNoShareRestartComputer = data.IsNoShareRestartComputer;
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

        [Description("最后更新")]
        public bool LastActivedOnText {
            get => _lastActivedOnText;
            set {
                if (_lastActivedOnText != value) {
                    _lastActivedOnText = value;
                    OnPropertyChanged(nameof(LastActivedOnText));
                    OnColumnItemChanged(nameof(LastActivedOnText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("运行时长")]
        public bool BootTimeSpanText {
            get { return _bootTimeSpanText; }
            set {
                if (_bootTimeSpanText != value) {
                    _bootTimeSpanText = value;
                    OnPropertyChanged(nameof(BootTimeSpanText));
                    OnColumnItemChanged(nameof(BootTimeSpanText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("挖矿时长")]
        public bool MineTimeSpanText {
            get { return _mineTimeSpanText; }
            set {
                if (_mineTimeSpanText != value) {
                    _mineTimeSpanText = value;
                    OnPropertyChanged(nameof(MineTimeSpanText));
                    OnColumnItemChanged(nameof(MineTimeSpanText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("作业")]
        public bool Work {
            get => _work;
            set {
                if (_work != value) {
                    _work = value;
                    OnPropertyChanged(nameof(Work));
                    OnColumnItemChanged(nameof(Work));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("分组")]
        public bool MinerGroup {
            get { return _minerGroup; }
            set {
                if (_minerGroup != value) {
                    _minerGroup = value;
                    OnPropertyChanged(nameof(MinerGroup));
                    OnColumnItemChanged(nameof(MinerGroup));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("矿机名")]
        public bool ClientName {
            get { return _clientName; }
            set {
                if (_clientName != value) {
                    _clientName = value;
                    OnPropertyChanged(nameof(ClientName));
                    OnColumnItemChanged(nameof(ClientName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("群控矿机名")]
        public bool MinerName {
            get { return _minerName; }
            set {
                if (_minerName != value) {
                    _minerName = value;
                    OnPropertyChanged(nameof(MinerName));
                    OnColumnItemChanged(nameof(MinerName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("IP")]
        public bool MinerIp {
            get { return _minerIp; }
            set {
                if (_minerIp != value) {
                    _minerIp = value;
                    OnPropertyChanged(nameof(MinerIp));
                    OnColumnItemChanged(nameof(MinerIp));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("远程桌面用户名密码")]
        public bool WindowsLoginNameAndPassword {
            get => _windowsLoginNameAndPassword;
            set {
                if (_windowsLoginNameAndPassword != value) {
                    _windowsLoginNameAndPassword = value;
                    OnPropertyChanged(nameof(WindowsLoginNameAndPassword));
                    OnColumnItemChanged(nameof(WindowsLoginNameAndPassword));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("显卡类型")]
        public bool GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    OnColumnItemChanged(nameof(GpuType));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("显卡")]
        public bool GpuInfo {
            get => _gpuInfo;
            set {
                if (_gpuInfo != value) {
                    _gpuInfo = value;
                    OnPropertyChanged(nameof(GpuInfo));
                    OnColumnItemChanged(nameof(GpuInfo));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("显卡驱动")]
        public bool GpuDriver {
            get => _gpuDriver;
            set {
                if (_gpuDriver != value) {
                    _gpuDriver = value;
                    OnPropertyChanged(nameof(GpuDriver));
                    OnColumnItemChanged(nameof(GpuDriver));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主挖币种")]
        public bool MainCoinCode {
            get {
                return _mainCoinCode;
            }
            set {
                if (_mainCoinCode != value) {
                    _mainCoinCode = value;
                    OnPropertyChanged(nameof(MainCoinCode));
                    OnColumnItemChanged(nameof(MainCoinCode));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主币算力")]
        public bool MainCoinSpeedText {
            get { return _mainCoinSpeedText; }
            set {
                if (_mainCoinSpeedText != value) {
                    _mainCoinSpeedText = value;
                    OnPropertyChanged(nameof(MainCoinSpeedText));
                    OnColumnItemChanged(nameof(MainCoinSpeedText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主币拒绝")]
        public bool MainCoinRejectPercentText {
            get => _mainCoinRejectPercentText;
            set {
                if (_mainCoinRejectPercentText != value) {
                    _mainCoinRejectPercentText = value;
                    OnPropertyChanged(nameof(MainCoinRejectPercentText));
                    OnColumnItemChanged(nameof(MainCoinRejectPercentText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币种")]
        public bool DualCoinCode {
            get => _dualCoinCode;
            set {
                if (_dualCoinCode != value) {
                    _dualCoinCode = value;
                    OnPropertyChanged(nameof(DualCoinCode));
                    OnColumnItemChanged(nameof(DualCoinCode));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币算力")]
        public bool DualCoinSpeedText {
            get => _dualCoinSpeedText;
            set {
                if (_dualCoinSpeedText != value) {
                    _dualCoinSpeedText = value;
                    OnPropertyChanged(nameof(DualCoinSpeedText));
                    OnColumnItemChanged(nameof(DualCoinSpeedText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币拒绝")]
        public bool DualCoinRejectPercentText {
            get => _dualCoinRejectPercentText;
            set {
                if (_dualCoinRejectPercentText != value) {
                    _dualCoinRejectPercentText = value;
                    OnPropertyChanged(nameof(DualCoinRejectPercentText));
                    OnColumnItemChanged(nameof(DualCoinRejectPercentText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("功耗")]
        public bool TotalPowerText {
            get => _totalPowerText;
            set {
                if (_totalPowerText != value) {
                    _totalPowerText = value;
                    OnPropertyChanged(nameof(TotalPowerText));
                    OnColumnItemChanged(nameof(TotalPowerText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("最高卡温")]
        public bool MaxTempText {
            get => _maxTempText;
            set {
                if (_maxTempText != value) {
                    _maxTempText = value;
                    OnPropertyChanged(nameof(MaxTempText));
                    OnColumnItemChanged(nameof(MaxTempText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主币预期收益")]
        public bool IncomeMainCoinPerDayText {
            get => _incomeMainCoinPerDayText;
            set {
                if (_incomeMainCoinPerDayText != value) {
                    _incomeMainCoinPerDayText = value;
                    OnPropertyChanged(nameof(IncomeMainCoinPerDayText));
                    OnColumnItemChanged(nameof(IncomeMainCoinPerDayText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币预期收益")]
        public bool IncomeDualCoinPerDayText {
            get => _incomeDualCoinPerDayText;
            set {
                if (_incomeDualCoinPerDayText != value) {
                    _incomeDualCoinPerDayText = value;
                    OnPropertyChanged(nameof(IncomeDualCoinPerDayText));
                    OnColumnItemChanged(nameof(IncomeDualCoinPerDayText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主币钱包")]
        public bool MainCoinWallet {
            get => _mainCoinWallet;
            set {
                if (_mainCoinWallet != value) {
                    _mainCoinWallet = value;
                    OnPropertyChanged(nameof(MainCoinWallet));
                    OnColumnItemChanged(nameof(MainCoinWallet));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("主币矿池")]
        public bool MainCoinPool {
            get => _mainCoinPool;
            set {
                if (_mainCoinPool != value) {
                    _mainCoinPool = value;
                    OnPropertyChanged(nameof(MainCoinPool));
                    OnColumnItemChanged(nameof(MainCoinPool));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("内核")]
        public bool Kernel {
            get => _kernel;
            set {
                if (_kernel != value) {
                    _kernel = value;
                    OnPropertyChanged(nameof(Kernel));
                    OnColumnItemChanged(nameof(Kernel));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币钱包")]
        public bool DualCoinWallet {
            get => _dualCoinWallet;
            set {
                if (_dualCoinWallet != value) {
                    _dualCoinWallet = value;
                    OnPropertyChanged(nameof(DualCoinWallet));
                    OnColumnItemChanged(nameof(DualCoinWallet));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("双挖币矿池")]
        public bool DualCoinPool {
            get => _dualCoinPool;
            set {
                if (_dualCoinPool != value) {
                    _dualCoinPool = value;
                    OnPropertyChanged(nameof(DualCoinPool));
                    OnColumnItemChanged(nameof(DualCoinPool));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("客户端版本")]
        public bool Version {
            get => _version;
            set {
                if (_version != value) {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                    OnColumnItemChanged(nameof(Version));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("操作系统")]
        public bool OSName {
            get => _oSName;
            set {
                if (_oSName != value) {
                    _oSName = value;
                    OnPropertyChanged(nameof(OSName));
                    OnColumnItemChanged(nameof(OSName));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("虚拟内存")]
        public bool OSVirtualMemoryGbText {
            get => _oSVirtualMemoryGbText;
            set {
                if (_oSVirtualMemoryGbText != value) {
                    _oSVirtualMemoryGbText = value;
                    OnPropertyChanged(nameof(OSVirtualMemoryGbText));
                    OnColumnItemChanged(nameof(OSVirtualMemoryGbText));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("剩余磁盘")]
        public bool DiskSpace {
            get => _diskSpace;
            set {
                if (_diskSpace != value) {
                    _diskSpace = value;
                    OnPropertyChanged(nameof(DiskSpace));
                    OnColumnItemChanged(nameof(DiskSpace));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("开机启动")]
        public bool IsAutoBoot {
            get => _isAutoBoot;
            set {
                if (_isAutoBoot != value) {
                    _isAutoBoot = value;
                    OnPropertyChanged(nameof(IsAutoBoot));
                    OnColumnItemChanged(nameof(IsAutoBoot));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("自动挖矿")]
        public bool IsAutoStart {
            get => _isAutoStart;
            set {
                if (_isAutoStart != value) {
                    _isAutoStart = value;
                    OnPropertyChanged(nameof(IsAutoStart));
                    OnColumnItemChanged(nameof(IsAutoStart));
                    UpdateColumnsShowAsync();
                }
            }
        }

        [Description("内核命令行")]
        public bool KernelCommandLine {
            get => _kernelCommandLine;
            set {
                if (_kernelCommandLine != value) {
                    _kernelCommandLine = value;
                    OnPropertyChanged(nameof(KernelCommandLine));
                    OnColumnItemChanged(nameof(KernelCommandLine));
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

        public bool IsNoShareRestartComputer {
            get { return _isNoShareRestartComputer; }
            set {
                if (_isNoShareRestartComputer != value) {
                    _isNoShareRestartComputer = value;
                    OnPropertyChanged(nameof(IsNoShareRestartComputer));
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

        private void UpdateColumnsShowAsync() {
            Server.ControlCenterService.AddOrUpdateColumnsShowAsync(new ColumnsShowData().Update(this), (response, exception) => {
                if (!response.IsSuccess()) {
                    Write.UserFail(response.ReadMessage(exception));
                }
            });
        }
    }
}
