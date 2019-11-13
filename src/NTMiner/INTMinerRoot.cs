using NTMiner.AppSetting;
using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Profiles;
using NTMiner.KernelOutputKeyword;
using NTMiner.Profile;
using NTMiner.ServerMessage;
using NTMiner.User;
using System;
using System.Collections.Generic;

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

        IMineContext CurrentMineContext { get; }

        bool IsMining { get; }

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

        ICalcConfigSet CalcConfigSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelOutputKeywordSet LocalKernelOutputKeywordSet { get; }
        IKernelOutputKeywordSet ServerKernelOutputKeywordSet { get; }
        IServerMessageSet ServerMessageSet { get; }
        bool TryGetProfileKernel(out IKernel kernel);
        string BuildAssembleArgs(out Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments);
    }
}
