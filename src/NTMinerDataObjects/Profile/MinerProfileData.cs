using System;
using System.Text;

namespace NTMiner.Profile {
    public class MinerProfileData : IMinerProfile, IDbEntity<Guid>, IGetSignData {
        public static readonly Guid DefaultId = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0");
        public static MinerProfileData CreateDefaultData(Guid coinId) {
            return new MinerProfileData {
                Id = DefaultId,
                CoinId = coinId,
                MinerName = string.Empty,
                IsAutoRestartKernel = true,
                AutoRestartKernelTimes = 10,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 15,
                IsNoShareRestartComputer = false,
                NoShareRestartComputerMinutes = 30,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 12,
                PeriodicRestartKernelMinutes = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 24,
                PeriodicRestartComputerMinutes = 0,
                IsSpeedDownRestartComputer = false,
                RestartComputerSpeedDownPercent = 0,
                IsEChargeEnabled = true,
                EPrice = 0.3,
                IsPowerAppend = false,
                PowerAppend = 0,
                MaxTemp = 80,
                AutoStartDelaySeconds = 15,
                IsAutoDisableWindowsFirewall = true,
                IsShowInTaskbar = true,
                IsNoUi = false,
                IsAutoNoUi = false,
                AutoNoUiMinutes = 10,
                IsShowNotifyIcon = true,
                IsCloseMeanExit = false,
                IsShowCommandLine = false,
                IsAutoBoot = true,
                IsAutoStart = true,
                IsCreateShortcut = true,
                IsAutoStopByCpu = false,
                IsAutoStartByCpu = false,
                CpuGETemperatureSeconds = 60,
                CpuLETemperatureSeconds = 60,
                CpuStartTemperature = 40,
                CpuStopTemperature = 65,
                IsRaiseHighCpuEvent = true,
                HighCpuPercent = 80,
                HighCpuSeconds = 10
            };
        }

        public static MinerProfileData Create(IMinerProfile data) {
            return new MinerProfileData {
                Id = data.CoinId,
                MinerName = data.MinerName,
                CoinId = data.CoinId,
                IsAutoRestartKernel = data.IsAutoRestartKernel,
                AutoRestartKernelTimes = data.AutoRestartKernelTimes,
                IsNoShareRestartKernel = data.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = data.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = data.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = data.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = data.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = data.PeriodicRestartKernelHours,
                PeriodicRestartKernelMinutes = data.PeriodicRestartKernelMinutes,
                IsPeriodicRestartComputer = data.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = data.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = data.PeriodicRestartComputerMinutes,
                IsSpeedDownRestartComputer = data.IsSpeedDownRestartComputer,
                RestartComputerSpeedDownPercent = data.RestartComputerSpeedDownPercent,
                IsEChargeEnabled = data.IsEChargeEnabled,
                EPrice = data.EPrice,
                IsPowerAppend = data.IsPowerAppend,
                PowerAppend = data.PowerAppend,
                MaxTemp = data.MaxTemp,
                AutoStartDelaySeconds = data.AutoStartDelaySeconds,
                IsAutoDisableWindowsFirewall = data.IsAutoDisableWindowsFirewall,
                IsShowInTaskbar = data.IsShowInTaskbar,
                IsNoUi = data.IsNoUi,
                IsAutoNoUi = data.IsAutoNoUi,
                AutoNoUiMinutes = data.AutoNoUiMinutes,
                IsShowNotifyIcon = data.IsShowNotifyIcon,
                IsCloseMeanExit = data.IsCloseMeanExit,
                IsShowCommandLine = data.IsShowCommandLine,
                IsAutoBoot = data.IsAutoBoot,
                IsAutoStart = data.IsAutoStart,
                IsCreateShortcut = data.IsCreateShortcut,
                IsAutoStopByCpu = data.IsAutoStopByCpu,
                IsAutoStartByCpu = data.IsAutoStartByCpu,
                CpuGETemperatureSeconds = data.CpuGETemperatureSeconds,
                CpuLETemperatureSeconds = data.CpuLETemperatureSeconds,
                CpuStartTemperature = data.CpuStartTemperature,
                CpuStopTemperature = data.CpuStopTemperature,
                IsRaiseHighCpuEvent = data.IsRaiseHighCpuEvent,
                HighCpuPercent = data.HighCpuPercent,
                HighCpuSeconds = data.HighCpuSeconds
            };
        }

        public MinerProfileData() {
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

        public int AutoStartDelaySeconds { get; set; }

        public bool IsAutoDisableWindowsFirewall { get; set; }

        public bool IsShowInTaskbar { get; set; }

        public bool IsNoUi { get; set; }

        public bool IsAutoNoUi { get; set; }

        public int AutoNoUiMinutes { get; set; }

        public bool IsShowNotifyIcon { get; set; }

        public bool IsCloseMeanExit { get; set; }

        public bool IsShowCommandLine { get; set; }

        public bool IsAutoBoot { get; set; }

        public bool IsAutoStart { get; set; }

        public bool IsCreateShortcut { get; set; }

        public bool IsAutoStopByCpu { get; set; }

        public int CpuStopTemperature { get; set; }

        public int CpuGETemperatureSeconds { get; set; }

        public bool IsAutoStartByCpu { get; set; }

        public int CpuStartTemperature { get; set; }

        public int CpuLETemperatureSeconds { get; set; }

        public bool IsRaiseHighCpuEvent { get; set; }

        public int HighCpuPercent { get; set; }

        public int HighCpuSeconds { get; set; }

        public override string ToString() {
            return this.BuildSign().ToString();
        }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
