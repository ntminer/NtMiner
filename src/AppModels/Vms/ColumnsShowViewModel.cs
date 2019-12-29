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
        private bool _localIp;
        private bool _minerGroup;
        private bool _mainCoinCode;
        private bool _mainCoinSpeedText;
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
        private bool _autoStartDelaySeconds;
        private bool _oSName;
        private bool _oSVirtualMemoryGbText;
        private bool _gpuType;
        private bool _gpuDriver;
        private bool _totalPowerText;
        private bool _maxTempText;
        private bool _kernelCommandLine;
        private bool _diskSpace;
        private bool _isAutoRestartKernel;
        private bool _autoRestartKernelTimes;
        private bool _isNoShareRestartKernel;
        private bool _noShareRestartKernelMinutes;
        private bool _isNoShareRestartComputer;
        private bool _noShareRestartComputerMinutes;
        private bool _isPeriodicRestartKernel;
        private bool _periodicRestartKernelHours;
        private bool _periodicRestartKernelMinutes;
        private bool _isPeriodicRestartComputer;
        private bool _periodicRestartComputerHours;
        private bool _periodicRestartComputerMinutes;
        private bool _mainCoinPoolDelay;
        private bool _dualCoinPoolDelay;
        private bool _isAutoStopByCpu;
        private bool _isAutoStartByCpu;
        private bool _cpuGETemperatureSeconds;
        private bool _cpuStopTemperature;
        private bool _cpuLETemperatureSeconds;
        private bool _cpuStartTemperature;

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
            OnPropertyChanged(propertyName);
            ColumnItem item = this.ColumnItems.FirstOrDefault(a => a.PropertyInfo.Name == propertyName);
            if (item != null) {
                item.OnPropertyChanged(nameof(item.IsChecked));
            }
            RpcRoot.Server.ColumnsShowService.AddOrUpdateColumnsShowAsync(new ColumnsShowData().Update(this), (response, exception) => {
                if (!response.IsSuccess()) {
                    Write.UserFail(response.ReadMessage(exception));
                }
            });
        }

        public ICommand Hide { get; private set; }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public ColumnsShowViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
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
                    VirtualRoot.Out.ShowSuccess($"保存成功");
                }
                else {
                    VirtualRoot.Execute(new AddColumnsShowCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new ColumnsShowEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: "该项不能删除", title: "警告", icon: "Icon_Error"));
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.ColumnsShowName}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveColumnsShowCommand(this.Id));
                }));
            });
        }

        public ColumnsShowViewModel(IColumnsShow data) : this(data.GetId()) {
            _columnsShowName = data.ColumnsShowName;
            _work = data.Work;
            _minerName = data.MinerName;
            _clientName = data.ClientName;
            _minerIp = data.MinerIp;
            _localIp = data.LocalIp;
            _minerGroup = data.MinerGroup;
            _mainCoinCode = data.MainCoinCode;
            _mainCoinSpeedText = data.MainCoinSpeedText;
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
            _autoStartDelaySeconds = data.AutoStartDelaySeconds;
            _oSName = data.OSName;
            _oSVirtualMemoryGbText = data.OSVirtualMemoryGbText;
            _gpuType = data.GpuType;
            _gpuDriver = data.GpuDriver;
            _totalPowerText = data.TotalPowerText;
            _maxTempText = data.MaxTempText;
            _kernelCommandLine = data.KernelCommandLine;
            _diskSpace = data.DiskSpace;
            _isAutoRestartKernel = data.IsAutoRestartKernel;
            _autoRestartKernelTimes = data.AutoRestartKernelTimes;
            _isNoShareRestartKernel = data.IsNoShareRestartKernel;
            _noShareRestartKernelMinutes = data.NoShareRestartKernelMinutes;
            _isNoShareRestartComputer = data.IsNoShareRestartComputer;
            _noShareRestartComputerMinutes = data.NoShareRestartComputerMinutes;
            _isPeriodicRestartKernel = data.IsPeriodicRestartKernel;
            _periodicRestartKernelHours = data.PeriodicRestartKernelHours;
            _periodicRestartKernelMinutes = data.PeriodicRestartKernelMinutes;
            _isPeriodicRestartComputer = data.IsPeriodicRestartComputer;
            _periodicRestartComputerHours = data.PeriodicRestartComputerHours;
            _periodicRestartComputerMinutes = data.PeriodicRestartComputerMinutes;
            _mainCoinPoolDelay = data.MainCoinPoolDelay;
            _dualCoinPoolDelay = data.DualCoinPoolDelay;
            _isAutoStopByCpu = data.IsAutoStopByCpu;
            _isAutoStartByCpu = data.IsAutoStartByCpu;
            _cpuGETemperatureSeconds = data.CpuGETemperatureSeconds;
            _cpuStopTemperature = data.CpuStopTemperature;
            _cpuLETemperatureSeconds = data.CpuLETemperatureSeconds;
            _cpuStartTemperature = data.CpuStartTemperature;
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
                    OnColumnItemChanged(nameof(ColumnsShowName));
                }
            }
        }

        public const string LAST_ACTIVED_ON_TEXT = "最后更新";
        [Description(LAST_ACTIVED_ON_TEXT)]
        public bool LastActivedOnText {
            get => _lastActivedOnText;
            set {
                if (_lastActivedOnText != value) {
                    _lastActivedOnText = value;
                    OnColumnItemChanged(nameof(LastActivedOnText));
                }
            }
        }

        public const string BOOT_TIME_SPAN_TEXT = "运行时长";
        [Description(BOOT_TIME_SPAN_TEXT)]
        public bool BootTimeSpanText {
            get { return _bootTimeSpanText; }
            set {
                if (_bootTimeSpanText != value) {
                    _bootTimeSpanText = value;
                    OnColumnItemChanged(nameof(BootTimeSpanText));
                }
            }
        }

        public const string MINE_TIME_SPAN_TEXT = "挖矿时长";
        [Description(MINE_TIME_SPAN_TEXT)]
        public bool MineTimeSpanText {
            get { return _mineTimeSpanText; }
            set {
                if (_mineTimeSpanText != value) {
                    _mineTimeSpanText = value;
                    OnColumnItemChanged(nameof(MineTimeSpanText));
                }
            }
        }

        public const string WORK = "作业";
        [Description(WORK)]
        public bool Work {
            get => _work;
            set {
                if (_work != value) {
                    _work = value;
                    OnColumnItemChanged(nameof(Work));
                }
            }
        }

        public const string MINER_GROUP = "分组";
        [Description(MINER_GROUP)]
        public bool MinerGroup {
            get { return _minerGroup; }
            set {
                if (_minerGroup != value) {
                    _minerGroup = value;
                    OnColumnItemChanged(nameof(MinerGroup));
                }
            }
        }

        public const string CLIENT_NAME = "矿机名";
        [Description(CLIENT_NAME)]
        public bool ClientName {
            get { return _clientName; }
            set {
                if (_clientName != value) {
                    _clientName = value;
                    OnColumnItemChanged(nameof(ClientName));
                }
            }
        }

        public const string MINER_NAME = "群控名";
        [Description(MINER_NAME)]
        public bool MinerName {
            get { return _minerName; }
            set {
                if (_minerName != value) {
                    _minerName = value;
                    OnColumnItemChanged(nameof(MinerName));
                }
            }
        }

        public const string MINER_IP = "IP";
        [Description(MINER_IP)]
        public bool MinerIp {
            get { return _minerIp; }
            set {
                if (_minerIp != value) {
                    _minerIp = value;
                    OnColumnItemChanged(nameof(MinerIp));
                }
            }
        }

        public const string LOCAL_IP = "内网IP?";
        public const string LOCAL_IP_TOOLTIP = "挖矿端从2.6.6.6开始上报内网IP";
        [Description(LOCAL_IP)]
        public bool LocalIp {
            get { return _localIp; }
            set {
                if (_localIp != value) {
                    _localIp = value;
                    OnColumnItemChanged(nameof(LocalIp));
                }
            }
        }

        public const string WINDOWS_LOGIN_NAME_AND_PASSWORD = "远程桌面";
        [Description(WINDOWS_LOGIN_NAME_AND_PASSWORD)]
        public bool WindowsLoginNameAndPassword {
            get => _windowsLoginNameAndPassword;
            set {
                if (_windowsLoginNameAndPassword != value) {
                    _windowsLoginNameAndPassword = value;
                    OnColumnItemChanged(nameof(WindowsLoginNameAndPassword));
                }
            }
        }

        public const string GPU_TYPE = "显卡类型";
        [Description(GPU_TYPE)]
        public bool GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnColumnItemChanged(nameof(GpuType));
                }
            }
        }

        public const string GPU_INFO = "显卡";
        [Description(GPU_INFO)]
        public bool GpuInfo {
            get => _gpuInfo;
            set {
                if (_gpuInfo != value) {
                    _gpuInfo = value;
                    OnColumnItemChanged(nameof(GpuInfo));
                }
            }
        }

        public const string GPU_DRIVER = "显卡驱动";
        [Description(GPU_DRIVER)]
        public bool GpuDriver {
            get => _gpuDriver;
            set {
                if (_gpuDriver != value) {
                    _gpuDriver = value;
                    OnColumnItemChanged(nameof(GpuDriver));
                }
            }
        }

        public const string MAIN_COIN_CODE = "主挖币种";
        [Description(MAIN_COIN_CODE)]
        public bool MainCoinCode {
            get {
                return _mainCoinCode;
            }
            set {
                if (_mainCoinCode != value) {
                    _mainCoinCode = value;
                    OnColumnItemChanged(nameof(MainCoinCode));
                }
            }
        }

        public const string MAIN_COIN_SPEED_TEXT = "主币算力";
        [Description(MAIN_COIN_SPEED_TEXT)]
        public bool MainCoinSpeedText {
            get { return _mainCoinSpeedText; }
            set {
                if (_mainCoinSpeedText != value) {
                    _mainCoinSpeedText = value;
                    OnColumnItemChanged(nameof(MainCoinSpeedText));
                }
            }
        }

        public const string MAIN_COIN_REJECT_PERCENT_TEXT = "主币拒绝";
        [Description(MAIN_COIN_REJECT_PERCENT_TEXT)]
        public bool MainCoinRejectPercentText {
            get => _mainCoinRejectPercentText;
            set {
                if (_mainCoinRejectPercentText != value) {
                    _mainCoinRejectPercentText = value;
                    OnColumnItemChanged(nameof(MainCoinRejectPercentText));
                }
            }
        }

        public const string DUAL_COIN_CODE = "双挖币种";
        [Description(DUAL_COIN_CODE)]
        public bool DualCoinCode {
            get => _dualCoinCode;
            set {
                if (_dualCoinCode != value) {
                    _dualCoinCode = value;
                    OnColumnItemChanged(nameof(DualCoinCode));
                }
            }
        }

        public const string DUAL_COIN_SPEED_TEXT = "双挖币算力";
        [Description(DUAL_COIN_SPEED_TEXT)]
        public bool DualCoinSpeedText {
            get => _dualCoinSpeedText;
            set {
                if (_dualCoinSpeedText != value) {
                    _dualCoinSpeedText = value;
                    OnColumnItemChanged(nameof(DualCoinSpeedText));
                }
            }
        }

        public const string DUAL_COIN_REJECT_PERCENT_TEXT = "双挖币拒绝";
        [Description(DUAL_COIN_REJECT_PERCENT_TEXT)]
        public bool DualCoinRejectPercentText {
            get => _dualCoinRejectPercentText;
            set {
                if (_dualCoinRejectPercentText != value) {
                    _dualCoinRejectPercentText = value;
                    OnColumnItemChanged(nameof(DualCoinRejectPercentText));
                }
            }
        }

        public const string TOTAL_POWER_TEXT = "功耗";
        [Description(TOTAL_POWER_TEXT)]
        public bool TotalPowerText {
            get => _totalPowerText;
            set {
                if (_totalPowerText != value) {
                    _totalPowerText = value;
                    OnColumnItemChanged(nameof(TotalPowerText));
                }
            }
        }

        public const string MAX_TEMP_TEXT = "最高卡温";
        [Description(MAX_TEMP_TEXT)]
        public bool MaxTempText {
            get => _maxTempText;
            set {
                if (_maxTempText != value) {
                    _maxTempText = value;
                    OnColumnItemChanged(nameof(MaxTempText));
                }
            }
        }

        public const string INCOME_MAIN_COIN_PER_DAY_TEXT = "主币预期收益";
        [Description(INCOME_MAIN_COIN_PER_DAY_TEXT)]
        public bool IncomeMainCoinPerDayText {
            get => _incomeMainCoinPerDayText;
            set {
                if (_incomeMainCoinPerDayText != value) {
                    _incomeMainCoinPerDayText = value;
                    OnColumnItemChanged(nameof(IncomeMainCoinPerDayText));
                }
            }
        }

        public const string INCOME_DUAL_COIN_PER_DAY_TEXT = "双挖币预期收益";
        [Description(INCOME_DUAL_COIN_PER_DAY_TEXT)]
        public bool IncomeDualCoinPerDayText {
            get => _incomeDualCoinPerDayText;
            set {
                if (_incomeDualCoinPerDayText != value) {
                    _incomeDualCoinPerDayText = value;
                    OnColumnItemChanged(nameof(IncomeDualCoinPerDayText));
                }
            }
        }

        public const string MAIN_COIN_WALLET = "主币钱包";
        [Description(MAIN_COIN_WALLET)]
        public bool MainCoinWallet {
            get => _mainCoinWallet;
            set {
                if (_mainCoinWallet != value) {
                    _mainCoinWallet = value;
                    OnColumnItemChanged(nameof(MainCoinWallet));
                }
            }
        }

        public const string MAIN_COIN_POOL = "主币矿池";
        [Description(MAIN_COIN_POOL)]
        public bool MainCoinPool {
            get => _mainCoinPool;
            set {
                if (_mainCoinPool != value) {
                    _mainCoinPool = value;
                    OnColumnItemChanged(nameof(MainCoinPool));
                }
            }
        }

        public const string KERNEL = "内核";
        [Description(KERNEL)]
        public bool Kernel {
            get => _kernel;
            set {
                if (_kernel != value) {
                    _kernel = value;
                    OnColumnItemChanged(nameof(Kernel));
                }
            }
        }

        public const string DUAL_COIN_WALLET = "双挖币钱包";
        [Description(DUAL_COIN_WALLET)]
        public bool DualCoinWallet {
            get => _dualCoinWallet;
            set {
                if (_dualCoinWallet != value) {
                    _dualCoinWallet = value;
                    OnColumnItemChanged(nameof(DualCoinWallet));
                }
            }
        }

        public const string DUAL_COIN_POOL = "双挖币矿池";
        [Description(DUAL_COIN_POOL)]
        public bool DualCoinPool {
            get => _dualCoinPool;
            set {
                if (_dualCoinPool != value) {
                    _dualCoinPool = value;
                    OnColumnItemChanged(nameof(DualCoinPool));
                }
            }
        }

        public const string VERSION = "开源版本";
        [Description(VERSION)]
        public bool Version {
            get => _version;
            set {
                if (_version != value) {
                    _version = value;
                    OnColumnItemChanged(nameof(Version));
                }
            }
        }

        public const string OS_NAME = "操作系统";
        [Description(OS_NAME)]
        public bool OSName {
            get => _oSName;
            set {
                if (_oSName != value) {
                    _oSName = value;
                    OnColumnItemChanged(nameof(OSName));
                }
            }
        }

        public const string OS_VIRTUAL_MEMORY_GB_TEXT = "虚拟内存";
        [Description(OS_VIRTUAL_MEMORY_GB_TEXT)]
        public bool OSVirtualMemoryGbText {
            get => _oSVirtualMemoryGbText;
            set {
                if (_oSVirtualMemoryGbText != value) {
                    _oSVirtualMemoryGbText = value;
                    OnColumnItemChanged(nameof(OSVirtualMemoryGbText));
                }
            }
        }

        public const string DISK_SPACE = "剩余磁盘";
        [Description(DISK_SPACE)]
        public bool DiskSpace {
            get => _diskSpace;
            set {
                if (_diskSpace != value) {
                    _diskSpace = value;
                    OnColumnItemChanged(nameof(DiskSpace));
                }
            }
        }

        public const string IS_AUTO_BOOT = "开机启动";
        [Description(IS_AUTO_BOOT)]
        public bool IsAutoBoot {
            get => _isAutoBoot;
            set {
                if (_isAutoBoot != value) {
                    _isAutoBoot = value;
                    OnColumnItemChanged(nameof(IsAutoBoot));
                }
            }
        }

        public const string IS_AUTO_START = "自动挖矿";
        [Description(IS_AUTO_START)]
        public bool IsAutoStart {
            get => _isAutoStart;
            set {
                if (_isAutoStart != value) {
                    _isAutoStart = value;
                    OnColumnItemChanged(nameof(IsAutoStart));
                }
            }
        }

        public const string AUTO_START_DELAY_SECONDS = "延时秒数";
        [Description(AUTO_START_DELAY_SECONDS)]
        public bool AutoStartDelaySeconds {
            get => _autoStartDelaySeconds;
            set {
                if (_autoStartDelaySeconds != value) {
                    _autoStartDelaySeconds = value;
                    OnColumnItemChanged(nameof(AutoStartDelaySeconds));
                }
            }
        }

        public const string KERNEL_COMMAND_LINE = "内核命令行";
        [Description(KERNEL_COMMAND_LINE)]
        public bool KernelCommandLine {
            get => _kernelCommandLine;
            set {
                if (_kernelCommandLine != value) {
                    _kernelCommandLine = value;
                    OnColumnItemChanged(nameof(KernelCommandLine));
                }
            }
        }

        public const string IS_AUTO_RESTART_KERNEL = "自动重启内核";
        [Description(IS_AUTO_RESTART_KERNEL)]
        public bool IsAutoRestartKernel {
            get => _isAutoRestartKernel;
            set {
                if (_isAutoRestartKernel != value) {
                    _isAutoRestartKernel = value;
                    OnColumnItemChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public const string AUTO_RESTART_KERNEL_TIMES = "重启次数上限";
        [Description(AUTO_RESTART_KERNEL_TIMES)]
        public bool AutoRestartKernelTimes {
            get => _autoRestartKernelTimes;
            set {
                if (_autoRestartKernelTimes != value) {
                    _autoRestartKernelTimes = value;
                    OnColumnItemChanged(nameof(AutoRestartKernelTimes));
                }
            }
        }

        public const string IS_NO_SHARE_RESTART_KERNEL = "无份额重启内核";
        [Description(IS_NO_SHARE_RESTART_KERNEL)]
        public bool IsNoShareRestartKernel {
            get => _isNoShareRestartKernel;
            set {
                if (_isNoShareRestartKernel != value) {
                    _isNoShareRestartKernel = value;
                    OnColumnItemChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public const string NO_SHARE_RESTART_KERNEL_MINUTES = "无份额重启内核分钟";
        [Description(NO_SHARE_RESTART_KERNEL_MINUTES)]
        public bool NoShareRestartKernelMinutes {
            get => _noShareRestartKernelMinutes;
            set {
                if (_noShareRestartKernelMinutes != value) {
                    _noShareRestartKernelMinutes = value;
                    OnColumnItemChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public const string IS_NO_SHARE_RESTART_COMPUTER = "无份额重启电脑";
        [Description(IS_NO_SHARE_RESTART_COMPUTER)]
        public bool IsNoShareRestartComputer {
            get { return _isNoShareRestartComputer; }
            set {
                if (_isNoShareRestartComputer != value) {
                    _isNoShareRestartComputer = value;
                    OnColumnItemChanged(nameof(IsNoShareRestartComputer));
                }
            }
        }

        public const string NO_SHARE_RESTART_COMPUTER_MINUTES = "无份额重启电脑分钟";
        [Description(NO_SHARE_RESTART_COMPUTER_MINUTES)]
        public bool NoShareRestartComputerMinutes {
            get { return _noShareRestartComputerMinutes; }
            set {
                if (_noShareRestartComputerMinutes != value) {
                    _noShareRestartComputerMinutes = value;
                    OnColumnItemChanged(nameof(NoShareRestartComputerMinutes));
                }
            }
        }

        public const string IS_PERIODIC_RESTART_KERNEL = "周期重启内核";
        [Description(IS_PERIODIC_RESTART_KERNEL)]
        public bool IsPeriodicRestartKernel {
            get => _isPeriodicRestartKernel;
            set {
                if (_isPeriodicRestartKernel != value) {
                    _isPeriodicRestartKernel = value;
                    OnColumnItemChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public const string IS_PERIODIC_RESTART_COMPUTER = "周期重启电脑";
        [Description(IS_PERIODIC_RESTART_COMPUTER)]
        public bool IsPeriodicRestartComputer {
            get => _isPeriodicRestartComputer;
            set {
                if (_isPeriodicRestartComputer != value) {
                    _isPeriodicRestartComputer = value;
                    OnColumnItemChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public const string PERIODIC_RESTART_KERNEL_HOURS = "周期重启内核小时";
        [Description(PERIODIC_RESTART_KERNEL_HOURS)]
        public bool PeriodicRestartKernelHours {
            get => _periodicRestartKernelHours;
            set {
                if (_periodicRestartKernelHours != value) {
                    _periodicRestartKernelHours = value;
                    OnColumnItemChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }
        public const string PERIODIC_RESTART_KERNEL_MINUTES = "周期重启内核分钟";
        [Description(PERIODIC_RESTART_KERNEL_MINUTES)]
        public bool PeriodicRestartKernelMinutes {
            get => _periodicRestartKernelMinutes;
            set {
                if (_periodicRestartKernelMinutes != value) {
                    _periodicRestartKernelMinutes = value;
                    OnColumnItemChanged(nameof(PeriodicRestartKernelMinutes));
                }
            }
        }
        public const string PERIODIC_RESTART_COMPUTER_HOURS = "周期重启电脑小时";
        [Description(PERIODIC_RESTART_COMPUTER_HOURS)]
        public bool PeriodicRestartComputerHours {
            get => _periodicRestartComputerHours;
            set {
                if (_periodicRestartComputerHours != value) {
                    _periodicRestartComputerHours = value;
                    OnColumnItemChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }
        public const string PERIODIC_RESTART_COMPUTER_MINUTES = "周期重启电脑分钟";
        [Description(PERIODIC_RESTART_COMPUTER_MINUTES)]
        public bool PeriodicRestartComputerMinutes {
            get => _periodicRestartComputerMinutes;
            set {
                if (_periodicRestartComputerMinutes != value) {
                    _periodicRestartComputerMinutes = value;
                    OnColumnItemChanged(nameof(PeriodicRestartComputerMinutes));
                }
            }
        }

        public const string MAIN_COIN_POOL_DELAY = "矿池延时";
        [Description(MAIN_COIN_POOL_DELAY)]
        public bool MainCoinPoolDelay {
            get { return _mainCoinPoolDelay; }
            set {
                if (_mainCoinPoolDelay != value) {
                    _mainCoinPoolDelay = value;
                    OnColumnItemChanged(nameof(MainCoinPoolDelay));
                }
            }
        }

        public const string DUAL_COIN_POOL_DELAY = "双挖矿池延时";
        [Description(DUAL_COIN_POOL_DELAY)]
        public bool DualCoinPoolDelay {
            get { return _dualCoinPoolDelay; }
            set {
                if (_dualCoinPoolDelay != value) {
                    _dualCoinPoolDelay = value;
                    OnColumnItemChanged(nameof(DualCoinPoolDelay));
                }
            }
        }

        public const string IS_AUTO_STOP_BY_CPU = "CPU高温自动停止挖矿";
        [Description(IS_AUTO_STOP_BY_CPU)]
        public bool IsAutoStopByCpu {
            get => _isAutoStopByCpu;
            set {
                if (_isAutoStopByCpu != value) {
                    _isAutoStopByCpu = value;
                    OnPropertyChanged(nameof(IsAutoStopByCpu));
                }
            }
        }

        public const string IS_AUTO_START_BY_CPU = "CPU低温自动开始挖矿";
        [Description(IS_AUTO_START_BY_CPU)]
        public bool IsAutoStartByCpu {
            get => _isAutoStartByCpu;
            set {
                if (_isAutoStartByCpu != value) {
                    _isAutoStartByCpu = value;
                    OnPropertyChanged(nameof(IsAutoStartByCpu));
                }
            }
        }

        public const string CPU_GE_TEMPERATURE_SECONDS = "CPU高温秒数";
        [Description(CPU_GE_TEMPERATURE_SECONDS)]
        public bool CpuGETemperatureSeconds {
            get => _cpuGETemperatureSeconds;
            set {
                if (_cpuGETemperatureSeconds != value) {
                    _cpuGETemperatureSeconds = value;
                    OnPropertyChanged(nameof(CpuGETemperatureSeconds));
                }
            }
        }

        public const string CPU_STOP_TEMPERATURE = "CPU高温";
        [Description(CPU_STOP_TEMPERATURE)]
        public bool CpuStopTemperature {
            get => _cpuStopTemperature;
            set {
                if (_cpuStopTemperature != value) {
                    _cpuStopTemperature = value;
                    OnPropertyChanged(nameof(CpuStopTemperature));
                }
            }
        }

        public const string CPU_LE_TEMPERATURE_SECONDS = "CPU低温秒数";
        [Description(CPU_LE_TEMPERATURE_SECONDS)]
        public bool CpuLETemperatureSeconds {
            get => _cpuLETemperatureSeconds;
            set {
                if (_cpuLETemperatureSeconds != value) {
                    _cpuLETemperatureSeconds = value;
                    OnPropertyChanged(nameof(CpuLETemperatureSeconds));
                }
            }
        }

        public const string CPU_START_TEMPERATURE = "CPU低温";
        [Description(CPU_START_TEMPERATURE)]
        public bool CpuStartTemperature {
            get => _cpuStartTemperature;
            set {
                if (_cpuStartTemperature != value) {
                    _cpuStartTemperature = value;
                    OnPropertyChanged(nameof(CpuStartTemperature));
                }
            }
        }
    }
}
