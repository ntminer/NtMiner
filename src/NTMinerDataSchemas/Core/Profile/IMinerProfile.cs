using System;

namespace NTMiner.Core.Profile {
    public interface IMinerProfile : IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }
        string MinerName { get; }

        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }

        bool IsNetUnavailableStopMine { get; }
        int NetUnavailableStopMineMinutes { get; }
        bool IsNetAvailableStartMine { get; }
        int NetAvailableStartMineSeconds { get; }

        bool IsNoShareRestartComputer { get; }
        int NoShareRestartComputerMinutes { get; }

        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }
        int PeriodicRestartKernelMinutes { get; }

        bool IsPeriodicRestartComputer { get; }
        int PeriodicRestartComputerHours { get; }
        int PeriodicRestartComputerMinutes { get; }

        bool IsAutoRestartKernel { get; }
        int AutoRestartKernelTimes { get; }

        bool IsSpeedDownRestartComputer { get; }
        int RestartComputerSpeedDownPercent { get; }

        bool IsEChargeEnabled { get; }

        double EPrice { get; }

        bool IsPowerAppend { get; }
        int PowerAppend { get; }
        int MaxTemp { get; }
        int AutoStartDelaySeconds { get; }
        bool IsAutoDisableWindowsFirewall { get; }
        bool IsShowInTaskbar { get; }
        bool IsNoUi { get; }
        bool IsAutoNoUi { get; }
        int AutoNoUiMinutes { get; }
        bool IsShowNotifyIcon { get; }
        bool IsCloseMeanExit { get; }
        bool IsShowCommandLine { get; }
        bool IsAutoBoot { get; }
        bool IsAutoStart { get; }
        bool IsCreateShortcut { get; }
        bool IsAutoStopByCpu { get; }
        int CpuStopTemperature { get; }
        int CpuGETemperatureSeconds { get; }
        bool IsAutoStartByCpu { get; }
        int CpuStartTemperature { get; }
        int CpuLETemperatureSeconds { get; }
        bool IsRaiseHighCpuEvent { get; }
        int HighCpuBaseline { get; }
        int HighCpuSeconds { get; }
    }
}
