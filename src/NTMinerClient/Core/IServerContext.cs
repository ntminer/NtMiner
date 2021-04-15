using NTMiner.Core.Kernels;
using NTMiner.Hub;
using System;

namespace NTMiner.Core {
    /// <summary>
    /// 这个是server.litedb或server.json的对应
    /// </summary>
    public interface IServerContext {
        /// <summary>
        /// 有两种情况会ReInit：
        /// 1，因为用户的活动触发了从服务器加载到新版本的server.json时；
        /// 2，开始挖矿时作业模式变更了(切换了作业或由作业模式转入非作业模式或由非作业模式转入作业模式都属于作业模式变更)。
        /// </summary>
        void ReInit();
        void AddCmdPath<TCmd>(LogEnum logType, Type location, Action<TCmd> action) where TCmd : ICmd;
        void AddEventPath<TEvent>(string description, LogEnum logType, Type location, PathPriority priority, Action<TEvent> action) where TEvent : IEvent;
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
