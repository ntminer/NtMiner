using NTMiner.AppSetting;
using NTMiner.Core;
using NTMiner.Core.Cpus;
using NTMiner.Core.Gpus;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Profiles;
using NTMiner.KernelOutputKeyword;
using NTMiner.ServerMessage;
using NTMiner.User;
using System;

namespace NTMiner {
    public interface INTMinerRoot {
        void ReInitMinerProfile();

        string GetServerJsonVersion();

        IUserSet UserSet { get; }

        DateTime CreatedOn { get; }

        IAppSettingSet ServerAppSettingSet { get; }

        void Init(Action callback);

        void Exit();

        void StartMine(bool isRestart = false);

        void RestartMine(bool isWork = false);

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

        IReporter Reporter { get; }

        IServerContext ServerContext { get; }

        IGpuProfileSet GpuProfileSet { get; }

        IWorkProfile MinerProfile { get; }

        IMineWorkSet MineWorkSet { get; }

        IMinerGroupSet MinerGroupSet { get; }

        INTMinerWalletSet NTMinerWalletSet { get; }

        IColumnsShowSet ColumnsShowSet { get; }

        IOverClockDataSet OverClockDataSet { get; }

        string GpuSetInfo { get; }

        IGpuSet GpuSet { get; }

        ICpuPackage CpuPackage { get; }

        ICalcConfigSet CalcConfigSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelOutputKeywordSet KernelOutputKeywordSet { get; }
        IServerMessageSet ServerMessageSet { get; }
    }
}
