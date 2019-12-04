using NTMiner.Hub;
using NTMiner.Core;
using NTMiner.Core.Kernels;
using System;

namespace NTMiner {
    public interface IServerContext {
        void ReInit();
        void AddCmdPath<TCmd>(string description, LogEnum logType, Action<TCmd> action, Type location) where TCmd : ICmd;
        void AddEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action, Type location) where TEvent : IEvent;
        ICoinGroupSet CoinGroupSet { get; }
        ICoinSet CoinSet { get; }
        IFileWriterSet FileWriterSet { get; }
        IFragmentWriterSet FragmentWriterSet { get; }
        IGroupSet GroupSet { get; }
        IPoolSet PoolSet { get; }
        ISysDicItemSet SysDicItemSet { get; }
        ISysDicSet SysDicSet { get; }
        ICoinKernelSet CoinKernelSet { get; }
        IKernelInputSet KernelInputSet { get; }
        IKernelOutputSet KernelOutputSet { get; }
        IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; }
        IKernelSet KernelSet { get; }
        IPackageSet PackageSet { get; }
        IPoolKernelSet PoolKernelSet { get; }
    }
}
