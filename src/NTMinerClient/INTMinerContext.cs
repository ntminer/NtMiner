using NTMiner.Core;
using NTMiner.Core.Profiles;
using NTMiner.Cpus;
using NTMiner.Gpus;
using NTMiner.Mine;
using NTMiner.Report;
using System;

namespace NTMiner {
    public interface INTMinerContext {
        ILocalMessageSet LocalMessageSet { get; }
        void ReInitMinerProfile(WorkType workType);

        string GetServerJsonVersion();

        DateTime CreatedOn { get; }

        void Init(Action callback);

        void StartMine(bool isRestart = false);

        void RestartMine(WorkType workType = WorkType.None, string workerName = null);

        StopMineReason StopReason { get; }
        void StopMineAsync(StopMineReason stopReason, Action callback = null);

        IMineContext CreateMineContext();
        IMineContext CurrentMineContext { get; set; }
        /// <summary>
        /// 开始挖矿时锁定的挖矿上下文
        /// </summary>
        IMineContext LockedMineContext { get; }

        /// <summary>
        /// 等效于LockedMineContext非空
        /// </summary>
        bool IsMining { get; }

        IReportDataProvider ReporterDataProvider { get; }

        IServerContext ServerContext { get; }

        IGpuProfileSet GpuProfileSet { get; }

        IWorkProfile MinerProfile { get; }

        string GpuSetInfo { get; }

        IGpuSet GpuSet { get; }
        IOverClockDataSet OverClockDataSet { get; }

        ICpuPackage CpuPackage { get; }

        ICalcConfigSet CalcConfigSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelOutputKeywordSet KernelOutputKeywordSet { get; }
        IServerMessageSet ServerMessageSet { get; }

        Version MinNvidiaDriverVersion { get; }

        Version MinAmdDriverVersion { get; }

        void ExportServerVersionJson(string jsonFileFullName);
        void ExportWorkJson(MineWorkData mineWorkData, out string localJson, out string serverJson);
    }
}
