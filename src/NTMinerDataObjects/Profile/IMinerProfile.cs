using System;

namespace NTMiner.Profile {
    public interface IMinerProfile : IEntity<Guid> {
        Guid CoinId { get; }
        string MinerName { get; }

        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }

        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }

        bool IsPeriodicRestartComputer { get; }
        int PeriodicRestartComputerHours { get; }

        bool IsAutoRestartKernel { get; }
        int AutoRestartKernelTimes { get; }

        bool IsSpeedDownRestartComputer { get; }
        int RestartComputerSpeedDownPercent { get; }

        bool IsEChargeEnabled { get; }

        double EPrice { get; }
    }
}
