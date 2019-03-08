using NTMiner.Core.Kernels;
using NTMiner.Core.Kernels.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    internal class PoolSet : IPoolSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, PoolData> _dicById = new Dictionary<Guid, PoolData>();

        private readonly bool _isUseJson;
        public PoolSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            VirtualRoot.Accept<RefreshPoolSetCommand>(
                "处理刷新矿池数据集命令",
                LogEnum.Console,
                action: message => {
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (_dicById.ContainsKey(item.Id)) {
                            VirtualRoot.Execute(new UpdatePoolCommand(item));
                        }
                        else {
                            VirtualRoot.Execute(new AddPoolCommand(item));
                        }
                    }
                });
            VirtualRoot.Accept<AddPoolCommand>(
                "添加矿池",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id " + message.Input.CoinId);
                    }
                    if (string.IsNullOrEmpty(message.Input.Server)) {
                        throw new ValidationException("pool poolurl can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    PoolData entity = new PoolData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new PoolAddedEvent(entity));

                    ICoin coin;
                    if (root.CoinSet.TryGetCoin(message.Input.CoinId, out coin)) {
                        ICoinKernel[] coinKernels = root.CoinKernelSet.Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (ICoinKernel coinKernel in coinKernels) {
                            Guid poolKernelId = Guid.NewGuid();
                            var poolKernel = new PoolKernelData() {
                                Id = poolKernelId,
                                Args = string.Empty,
                                Description = string.Empty,
                                KernelId = coinKernel.KernelId,
                                PoolId = message.Input.GetId()
                            };
                            VirtualRoot.Execute(new AddPoolKernelCommand(poolKernel));
                        }
                    }
                });
            VirtualRoot.Accept<UpdatePoolCommand>(
                "更新矿池",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id " + message.Input.CoinId);
                    }
                    if (string.IsNullOrEmpty(message.Input.Server)) {
                        throw new ValidationException("pool poolurl can't be null or empty");
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("pool name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    PoolData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                    repository.Update(new PoolData().Update(message.Input));

                    VirtualRoot.Happened(new PoolUpdatedEvent(entity));
                });
            VirtualRoot.Accept<RemovePoolCommand>(
                "移除矿池",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    // 组合ServerDb和ProfileDb，Profile用户无权删除GlobalDb中的数据，否则抛出异常终端流程从而确保ServerDb中的数据不会被后续的流程修改
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                    repository.Remove(message.EntityId);
                    PoolData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());

                    VirtualRoot.Happened(new PoolRemovedEvent(entity));
                    Guid[] toRemoves = root.PoolKernelSet.Where(a => a.PoolId == message.EntityId).Select(a => a.GetId()).ToArray();
                    foreach (Guid poolKernelId in toRemoves) {
                        VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                    }
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        public bool Contains(Guid poolId) {
            InitOnece();
            return _dicById.ContainsKey(poolId);
        }

        public bool TryGetPool(Guid poolId, out IPool pool) {
            InitOnece();
            PoolData p;
            var r = _dicById.TryGetValue(poolId, out p);
            pool = p;
            return r;
        }

        public IEnumerator<IPool> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
