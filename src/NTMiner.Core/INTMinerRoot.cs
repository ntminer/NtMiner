using NTMiner.AppSetting;
using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Profiles;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public interface INTMinerRoot {
        DelegateHandler<TCmd> ServerContextWindow<TCmd>(string description, LogEnum logType, Action<TCmd> action) where TCmd : ICmd;
        DelegateHandler<TEvent> ServerContextOn<TEvent>(string description, LogEnum logType, Action<TEvent> action) where TEvent : IEvent;
        
        void ReInitMinerProfile();

        IUserSet UserSet { get; }

        DateTime CreatedOn { get; }

        IAppSettingSet ServerAppSettingSet { get; }

        IAppSettingSet LocalAppSettingSet { get; }

        void Init(Action callback);

        void Exit();

        void StartMine();

        void RestartMine(bool isWork = false);

        void StopMineAsync(Action callback = null);

        IMineContext CurrentMineContext { get; }

        bool IsMining { get; }

        IWorkProfile MinerProfile { get; }

        IMineWorkSet MineWorkSet { get; }

        IMinerGroupSet MinerGroupSet { get; }

        IColumnsShowSet ColumnsShowSet { get; }

        IOverClockDataSet OverClockDataSet { get; }

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

        IFileWriterSet FileWriterSet { get; }

        IFragmentWriterSet FragmentWriterSet { get; }

        IPackageSet PackageSet { get; }

        IKernelProfileSet KernelProfileSet { get; }

        IGpusSpeed GpusSpeed { get; }

        ICoinShareSet CoinShareSet { get; }

        IKernelInputSet KernelInputSet { get; }

        IKernelOutputSet KernelOutputSet { get; }

        IKernelOutputFilterSet KernelOutputFilterSet { get; }

        IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; }

        IWorkerEventSet WorkerEventSet { get; }

        IKernelOutputKeywordSet KernelOutputKeywordSet { get; }
        string BuildAssembleArgs(out Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments);
    }
}
