using System;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Profile {
    public class MinerProfileData : IMinerProfile, IProfile, IDbEntity<Guid> {
        public static readonly Guid DefaultId = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0");
        public static MinerProfileData CreateDefaultData(Guid coinId) {
            return new MinerProfileData {
                Id = DefaultId,
                CoinId = coinId
            };
        }

        private static PropertyInfo[] _workIgnoreProperties;
        private static object _locker = new object();
        private static PropertyInfo[] WorkIgnoreProperties {
            get {
                if (_workIgnoreProperties == null) {
                    lock (_locker) {
                        if (_workIgnoreProperties == null) {
                            Type attributeType = typeof(WorkIgnoreAttribute);
                            _workIgnoreProperties = typeof(MinerProfileData).GetProperties().Where(a => a.GetCustomAttributes(attributeType, inherit: false).Length != 0).ToArray();
                        }
                    }
                }
                return _workIgnoreProperties;
            }
        }

        public static void CopyWorkIgnoreValues(MinerProfileData from, MinerProfileData to) {
            foreach (var propertyInfo in WorkIgnoreProperties) {
                propertyInfo.SetValue(to, propertyInfo.GetValue(from, null), null);
            }
        }

        public MinerProfileData() {
            // 这些赋值的意义是为模型新增的属性提供默认值，因为新增的属性在数据库中没有值
            MinerName = string.Empty;
            IsAutoRestartKernel = true;
            AutoRestartKernelTimes = 10;
            IsNoShareRestartKernel = false;
            NoShareRestartKernelMinutes = 15;
            IsNetUnavailableStopMine = false;
            IsNetAvailableStartMine = false;
            NetUnavailableStopMineMinutes = 1;
            NetAvailableStartMineSeconds = 10;
            IsNoShareRestartComputer = false;
            NoShareRestartComputerMinutes = 30;
            IsPeriodicRestartKernel = false;
            PeriodicRestartKernelHours = 12;
            PeriodicRestartKernelMinutes = 0;
            IsPeriodicRestartComputer = false;
            PeriodicRestartComputerHours = 24;
            PeriodicRestartComputerMinutes = 0;
            IsSpeedDownRestartComputer = false;
            RestartComputerSpeedDownPercent = 0;
            IsEChargeEnabled = true;
            EPrice = 0.3;
            IsPowerAppend = false;
            PowerAppend = 0;
            MaxTemp = 80;
            AutoStartDelaySeconds = 15;
            IsAutoDisableWindowsFirewall = true;
            IsDisableAntiSpyware = true;
            IsDisableUAC = true;
            IsDisableWAU = true;
            IsShowInTaskbar = true;
            IsNoUi = false;
            IsAutoNoUi = false;
            AutoNoUiMinutes = 10;
            IsShowNotifyIcon = true;
            IsCloseMeanExit = false;
            IsShowCommandLine = false;
            IsAutoBoot = false;
            IsAutoStart = false;
            IsCreateShortcut = true;
            IsAutoStopByCpu = false;
            IsAutoStartByCpu = false;
            CpuGETemperatureSeconds = 60;
            CpuLETemperatureSeconds = 60;
            CpuStartTemperature = 40;
            CpuStopTemperature = 65;
            IsRaiseHighCpuEvent = true;
            HighCpuBaseline = 80;
            HighCpuSeconds = 10;
            IsOuterUserEnabled = false;
            OuterUserId = string.Empty;
        }

        public Guid GetId() {
            return this.Id;
        }
        public Guid Id { get; set; }
        public string MinerName { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public int AutoRestartKernelTimes { get; set; }
        public Guid CoinId { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsNetUnavailableStopMine { get; set; }
        public int NetUnavailableStopMineMinutes { get; set; }
        public bool IsNetAvailableStartMine { get; set; }
        public int NetAvailableStartMineSeconds { get; set; }
        public bool IsNoShareRestartComputer { get; set; }
        public int NoShareRestartComputerMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }

        public bool IsSpeedDownRestartComputer { get; set; }

        public int RestartComputerSpeedDownPercent { get; set; }

        public bool IsEChargeEnabled { get; set; }

        public double EPrice { get; set; }

        public bool IsPowerAppend { get; set; }

        public int PowerAppend { get; set; }

        public int PeriodicRestartKernelMinutes { get; set; }

        public int PeriodicRestartComputerMinutes { get; set; }

        public int MaxTemp { get; set; }

        [WorkIgnore]
        public int AutoStartDelaySeconds { get; set; }

        [WorkIgnore]
        public bool IsAutoDisableWindowsFirewall { get; set; }

        [WorkIgnore]
        public bool IsDisableUAC { get; set; }

        [WorkIgnore]
        public bool IsDisableWAU { get; set; }

        [WorkIgnore]
        public bool IsDisableAntiSpyware { get; set; }

        [WorkIgnore]
        public bool IsShowInTaskbar { get; set; }

        [WorkIgnore]
        public bool IsNoUi { get; set; }

        [WorkIgnore]
        public bool IsAutoNoUi { get; set; }

        [WorkIgnore]
        public int AutoNoUiMinutes { get; set; }

        [WorkIgnore]
        public bool IsShowNotifyIcon { get; set; }

        [WorkIgnore]
        public bool IsCloseMeanExit { get; set; }

        [WorkIgnore]
        public bool IsShowCommandLine { get; set; }

        [WorkIgnore]
        public bool IsAutoBoot { get; set; }
        [WorkIgnore]
        public bool IsAutoStart { get; set; }

        [WorkIgnore]
        public bool IsCreateShortcut { get; set; }

        public bool IsAutoStopByCpu { get; set; }

        public int CpuStopTemperature { get; set; }

        public int CpuGETemperatureSeconds { get; set; }

        public bool IsAutoStartByCpu { get; set; }

        public int CpuStartTemperature { get; set; }

        public int CpuLETemperatureSeconds { get; set; }

        public bool IsRaiseHighCpuEvent { get; set; }

        public int HighCpuBaseline { get; set; }

        public int HighCpuSeconds { get; set; }

        [WorkIgnore]
        public bool IsOuterUserEnabled { get; set; }

        [WorkIgnore]
        public string OuterUserId { get; set; }

        // 检测内存状态是否变更时使用
        public override string ToString() {
            return this.BuildSign().ToString();
        }
    }
}
