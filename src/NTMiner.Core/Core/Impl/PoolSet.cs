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

                    if (VirtualRoot.IsControlCenter) {
                        Server.ControlCenterService.AddOrUpdatePoolAsync(entity, callback: null);
                    }
                    else {
                        var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
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
                    if (VirtualRoot.IsControlCenter) {
                        var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                        repository.Update(new PoolData().Update(message.Input));
                    }
                    else {
                        Server.ControlCenterService.AddOrUpdatePoolAsync(entity, callback: null);
                    }

                    VirtualRoot.Happened(new PoolUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
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
                    
                    PoolData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    if (VirtualRoot.IsControlCenter) {
                        var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(isUseJson);
                        repository.Remove(message.EntityId);
                    }
                    else {
                        Server.ControlCenterService.RemovePoolAsync(entity.Id, callback: null);
                    }
                    VirtualRoot.Happened(new PoolRemovedEvent(entity));
                    Guid[] toRemoves = root.PoolKernelSet.Where(a => a.PoolId == message.EntityId).Select(a => a.GetId()).ToArray();
                    foreach (Guid poolKernelId in toRemoves) {
                        VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                    }
                }).AddToCollection(root.ContextHandlers);
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
                    IEnumerable<PoolData> list;
                    if (VirtualRoot.IsControlCenter) {
                        var repository = NTMinerRoot.CreateCompositeRepository<PoolData>(_isUseJson);
                        list = repository.GetAll();
                    }
                    else {
                        var response = Server.ControlCenterService.GetPools();
                        if (response != null) {
                            list = response.Data;
                        }
                        else {
                            list = new List<PoolData>();
                        }
                    }
                    foreach (var item in list) {
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
