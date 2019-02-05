using System;

namespace NTMiner {
    public interface IMinerProfile : IEntity<Guid> {
        string MinerName { get; }
        bool IsAutoThisPCName { get; }
        bool IsShowInTaskbar { get; }
        Guid CoinId { get; }
        bool IsAutoBoot { get; }
        bool IsAutoStart { get; }

        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }

        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }

        bool IsPeriodicRestartComputer { get; }
        int PeriodicRestartComputerHours { get; }

        bool IsAutoRestartKernel { get; }

        bool IsShowCommandLine { get; }
    }
}
