using NTMiner.Repositories;
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
            VirtualRoot.Window<AddPoolCommand>("添加矿池", LogEnum.DevConsole,
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

                    if (VirtualRoot.IsMinerStudio) {
                        Server.ControlCenterService.AddOrUpdatePoolAsync(entity, callback: null);
                    }
                    else {
                        var repository = CreateCompositeRepository<PoolData>(isUseJson);
                        repository.Add(entity);
                    }

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
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdatePoolCommand>("更新矿池", LogEnum.DevConsole,
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
                    if (VirtualRoot.IsMinerStudio) {
                        Server.ControlCenterService.AddOrUpdatePoolAsync(entity, callback: null);
                    }
                    else {
                        var repository = CreateCompositeRepository<PoolData>(isUseJson);
                        repository.Update(new PoolData().Update(message.Input));
                    }

                    VirtualRoot.Happened(new PoolUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemovePoolCommand>("移除矿池", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    
                    PoolData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    if (VirtualRoot.IsMinerStudio) {
                        Server.ControlCenterService.RemovePoolAsync(entity.Id, callback: null);
                    }
                    else {
                        var repository = CreateCompositeRepository<PoolData>(isUseJson);
                        repository.Remove(message.EntityId);
                    }
                    VirtualRoot.Happened(new PoolRemovedEvent(entity));
                    Guid[] toRemoves = root.PoolKernelSet.Where(a => a.PoolId == message.EntityId).Select(a => a.GetId()).ToArray();
                    foreach (Guid poolKernelId in toRemoves) {
                        VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                    }
                }).AddToCollection(root.ContextHandlers);
        }

        /// <summary>
        /// 创建组合仓储，组合仓储由ServerDb和ProfileDb层序组成。
        /// 如果是开发者则访问ServerDb且只访问GlobalDb，否则将ServerDb和ProfileDb并起来访问且不能修改删除GlobalDb。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IRepository<T> CreateCompositeRepository<T>(bool isUseJson) where T : class, ILevelEntity<Guid> {
            return new CompositeRepository<T>(NTMinerRoot.CreateServerRepository<T>(isUseJson), NTMinerRoot.CreateLocalRepository<T>(isUseJson));
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
                    var repository = CreateCompositeRepository<PoolData>(_isUseJson);
                    List<PoolData> data = repository.GetAll().ToList();
                    if (VirtualRoot.IsMinerStudio) {
                        foreach (var item in Server.ControlCenterService.GetPools()) {
                            data.Add(item);
                        }
                    }
                    foreach (var item in data) {
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
