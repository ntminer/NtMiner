using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Profiles;
using NTMiner.Core.SysDics;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public interface INTMinerRoot {
        List<IDelegateHandler> ContextHandlers { get; }

        event Action OnContextReInited;
        event Action OnReRendContext;

        DateTime CreatedOn { get; }

        IAppSettingSet AppSettingSet { get; }

        void Init(Action callback);

        void Exit();

        void Start();

        void StartMine(Guid workId);

        void StopMineAsync(Action callback = null);

        void RestartMine();

        IPackageDownloader PackageDownloader { get; }

        IMineContext CurrentMineContext { get; }

        bool IsMining { get; }

        IWorkProfile MinerProfile { get; }

        IMineWorkSet MineWorkSet { get; }

        IMinerGroupSet MinerGroupSet { get; }

        IColumnsShowSet ColumnsShowSet { get; }

        IOverClockDataSet OverClockDataSet { get; }

        string QQGroup { get; }

        int SpeedHistoryLengthByMinute { get; }

        string GpuSetInfo { get; }

        IGpuSet GpuSet { get; }

        ISysDicSet SysDicSet { get; }

        ISysDicItemSet SysDicItemSet { get; }

        ICoinSet CoinSet { get; }

        IGroupSet GroupSet { get; }

        ICoinGroupSet CoinGroupSet { get; }

        ICalcConfigSet CalcConfigSet { get; }

        IPoolSet PoolSet { get; }

        ICoinKernelSet CoinKernelSet { get; }

        IPoolKernelSet PoolKernelSet { get; }

        IKernelSet KernelSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelInputSet KernelInputSet { get; }

        IKernelOutputSet KernelOutputSet { get; }

        IKernelOutputFilterSet KernelOutputFilterSet { get; }

        IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; }

        object GetMineWorkProperty(string propertyName);

        void SetMinerProfileProperty(string propertyName, object value);

        void SetCoinProfileProperty(Guid coinId, string propertyName, object value);

        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);

        void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value);

        IMineContext CreateMineContext(
            string minerName,
            ICoin mainCoin,
            IPool mainCoinPool,
            IKernel kernel,
            ICoinKernel coinKernel,
            string mainCoinWallet);

        IDualMineContext CreateDualMineContext(
            IMineContext mineContext,
            ICoin dualCoin,
            IPool dualCoinPool,
            string dualCoinWallet,
            double dualCoinWeight);

        string BuildAssembleArgs();
    }
}
