using System;

namespace NTMiner.Profile {
    public interface IMinerProfile : IEntity<Guid> {
        Guid CoinId { get; }
        string MinerName { get; }

        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }

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
    }
}
