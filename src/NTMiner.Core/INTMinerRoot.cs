using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Core.Profiles;
using NTMiner.Core.SysDics;
using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner {
    public interface INTMinerRoot {
        void Exit();

        void Start();

        void StartMine(Guid workId);

        void StopMine(bool wait = true);

        void RestartMine();

        IPackageDownloader PackageDownloader { get; }

        IMineContext CurrentMineContext { get; }

        bool IsMining { get; }

        IMinerProfile MinerProfile { get; }

        IMineWorkSet MineWorkSet { get; }

        IMinerGroupSet MinerGroupSet { get; }

        string QQGroup { get; }

        int GpuAllId { get; }

        int SpeedHistoryLengthByMinute { get; }

        IGpuSet GpuSet { get; }

        ISysDicSet SysDicSet { get; }

        ISysDicItemSet SysDicItemSet { get; }

        ICoinSet CoinSet { get; }

        IGroupSet GroupSet { get; }

        ICoinGroupSet CoinGroupSet { get; }

        ICalcConfigSet CalcConfigSet { get; }

        IWalletSet WalletSet { get; }

        IPoolSet PoolSet { get; }

        ICoinKernelSet CoinKernelSet { get; }

        IKernelSet KernelSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelOutputFilterSet KernelOutputFilterSet { get; }

        IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; }

        ICoinProfileSet CoinProfileSet { get; }

        ICoinKernelProfileSet CoinKernelProfileSet { get; }

        void SetMinerProfileProperty(string propertyName, object value);

        object GetMineWorkProperty(string propertyName);

        void SetCoinProfileProperty(Guid coinId, string propertyName, object value);

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
