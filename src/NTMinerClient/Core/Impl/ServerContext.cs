using NTMiner.Bus;
using NTMiner.Core.Kernels;
using NTMiner.Core.Kernels.Impl;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class ServerContext : IServerContext {
        private readonly List<IMessagePathId> _serverContextHandlers = new List<IMessagePathId>();

        public ServerContext() {
            ReInit();
        }

        public void ReInit() {
            foreach (var handler in _serverContextHandlers) {
                VirtualRoot.DeletePath(handler);
            }
            _serverContextHandlers.Clear();
            this.CoinGroupSet = new CoinGroupSet(this);
            this.CoinSet = new CoinSet(this);
            this.FileWriterSet = new FileWriterSet(this);
            this.FragmentWriterSet = new FragmentWriterSet(this);
            this.GroupSet = new GroupSet(this);
            this.PoolSet = new PoolSet(this);
            this.SysDicItemSet = new SysDicItemSet(this);
            this.SysDicSet = new SysDicSet(this);
            this.CoinKernelSet = new CoinKernelSet(this);
            this.KernelInputSet = new KernelInputSet(this);
            this.KernelOutputSet = new KernelOutputSet(this);
            this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this);
            this.KernelSet = new KernelSet(this);
            this.PackageSet = new PackageSet(this);
            this.PoolKernelSet = new PoolKernelSet(this);
        }

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public void BuildCmdPath<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            var messagePathId = VirtualRoot.BuildPath(description, logType, action);
            _serverContextHandlers.Add(messagePathId);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public void BuildEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            var messagePathId = VirtualRoot.BuildPath(description, logType, action);
            _serverContextHandlers.Add(messagePathId);
        }

        public ICoinGroupSet CoinGroupSet { get; private set; }

        public ICoinSet CoinSet { get; private set; }

        public IFileWriterSet FileWriterSet { get; private set; }

        public IFragmentWriterSet FragmentWriterSet { get; private set; }

        public IGroupSet GroupSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public ISysDicItemSet SysDicItemSet { get; private set; }

        public ISysDicSet SysDicSet { get; private set; }

        public ICoinKernelSet CoinKernelSet { get; private set; }

        public IKernelInputSet KernelInputSet { get; private set; }

        public IKernelOutputSet KernelOutputSet { get; private set; }

        public IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; private set; }

        public IKernelSet KernelSet { get; private set; }

        public IPackageSet PackageSet { get; private set; }

        public IPoolKernelSet PoolKernelSet { get; private set; }
    }
}
