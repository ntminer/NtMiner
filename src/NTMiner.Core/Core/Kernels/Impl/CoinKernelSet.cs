using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class CoinKernelSet : ICoinKernelSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, CoinKernelData> _dicById = new Dictionary<Guid, CoinKernelData>();

        private readonly bool _isUseJson;
        public CoinKernelSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            VirtualRoot.Window<AddCoinKernelCommand>("添加币种内核", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id" + message.Input.CoinId);
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicById.Values.Any(a => a.CoinId == message.Input.CoinId && a.KernelId == message.Input.KernelId)) {
                        return;
                    }
                    CoinKernelData entity = new CoinKernelData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new CoinKernelAddedEvent(entity));

                    ICoin coin;
                    if (root.CoinSet.TryGetCoin(message.Input.CoinId, out coin)) {
                        IPool[] pools = root.PoolSet.Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            Guid poolKernelId = Guid.NewGuid();
                            var poolKernel = new PoolKernelData() {
                                Id = poolKernelId,
                                Args = string.Empty,
                                Description = string.Empty,
                                KernelId = message.Input.KernelId,
                                PoolId = pool.GetId()
                            };
                            VirtualRoot.Execute(new AddPoolKernelCommand(poolKernel));
                        }
                    }
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdateCoinKernelCommand>("更新币种内核", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id" + message.Input.CoinId);
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    CoinKernelData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new CoinKernelUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemoveCoinKernelCommand>("移除币种内核", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    CoinKernelData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new CoinKernelRemovedEvent(entity));
                    ICoin coin;
                    if (root.CoinSet.TryGetCoin(entity.CoinId, out coin)) {
                        List<Guid> toRemoves = new List<Guid>();
                        IPool[] pools = root.PoolSet.Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            foreach (PoolKernelData poolKernel in root.PoolKernelSet.Where(a => a.PoolId == pool.GetId() && a.KernelId == entity.KernelId)) {
                                toRemoves.Add(poolKernel.Id);
                            }
                        }
                        foreach (Guid poolKernelId in toRemoves) {
                            VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                        }
                    }
                }).AddToCollection(root.ContextHandlers);
        }

        private bool _isInited = false;
        private object _locker = new object();

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid kernelId) {
            InitOnece();
            return _dicById.ContainsKey(kernelId);
        }

        public bool TryGetCoinKernel(Guid kernelId, out ICoinKernel kernel) {
            InitOnece();
            CoinKernelData k;
            var r = _dicById.TryGetValue(kernelId, out k);
            kernel = k;
            return r;
        }

        public IEnumerator<ICoinKernel> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
