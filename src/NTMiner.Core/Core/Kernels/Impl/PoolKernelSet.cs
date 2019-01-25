using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class PoolKernelSet : IPoolKernelSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, PoolKernelData> _dicById = new Dictionary<Guid, PoolKernelData>();

        public PoolKernelSet(INTMinerRoot root) {
            _root = root;
            Global.Access<CoinKernelAddedEvent>(
                Guid.Parse("F39019DF-21F7-40EF-9233-4F7E8291FF39"),
                "新添了币种内核后刷新矿池内核内存",
                LogEnum.Log,
                action: (message) => {
                    ICoin coin;
                    if (root.CoinSet.TryGetCoin(message.Source.CoinId, out coin)) {
                        IPool[] pools = root.PoolSet.Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            Guid poolKernelId = Guid.NewGuid();
                            var entity = new PoolKernelData() {
                                Id = poolKernelId,
                                Args = string.Empty,
                                Description = string.Empty,
                                KernelId = message.Source.KernelId,
                                PoolId = pool.GetId()
                            };
                            _dicById.Add(poolKernelId, entity);
                            Global.Happened(new PoolKernelAddedEvent(entity));
                        }
                    }
                });
            Global.Access<CoinKernelRemovedEvent>(
                Guid.Parse("3FD6D2B4-4C8F-4C81-8E15-2FBA4E730AF7"),
                "移除了币种内核后刷新矿池内核内存",
                LogEnum.Log,
                action: (message) => {
                    ICoin coin;
                    if (root.CoinSet.TryGetCoin(message.Source.CoinId, out coin)) {
                        var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                        List<Guid> toRemoves = new List<Guid>();
                        IPool[] pools = root.PoolSet.Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (IPool pool in pools) {
                            foreach (PoolKernelData poolKernelVm in _dicById.Values.Where(a => a.PoolId == pool.GetId() && a.KernelId == message.Source.KernelId)) {
                                toRemoves.Add(poolKernelVm.Id);
                            }
                        }
                        foreach (Guid poolKernelId in toRemoves) {
                            var entity = _dicById[poolKernelId];
                            _dicById.Remove(poolKernelId);
                            repository.Remove(entity.Id);
                            Global.Happened(new PoolKernelRemovedEvent(entity));
                        }
                    }
                });
            Global.Access<PoolRemovedEvent>(
                Guid.Parse("F4B99EAE-2532-4DAC-8D49-2D6A51530722"),
                "移除了矿池后刷新矿池内核内存",
                LogEnum.Log,
                action: (message) => {
                    var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                    Guid[] toRemoves = _dicById.Values.Where(a => a.PoolId == message.Source.GetId()).Select(a => a.Id).ToArray();
                    foreach (Guid poolKernelId in toRemoves) {
                        var entity = _dicById[poolKernelId];
                        _dicById.Remove(poolKernelId);
                        repository.Remove(entity.Id);
                        Global.Happened(new PoolKernelRemovedEvent(entity));
                    }
                });
            Global.Access<UpdatePoolKernelCommand>(
                Guid.Parse("08843B3B-3F82-45D2-8B45-6B24F397A326"),
                "更新矿池内核",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.PoolSet.Contains(message.Input.PoolId)) {
                        throw new ValidationException("there is no pool with id" + message.Input.PoolId);
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    PoolKernelData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                    repository.Update(entity);

                    Global.Happened(new PoolKernelUpdatedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
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
                    var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                    List<PoolKernelData> list = repository.GetAll().ToList();
                    foreach (IPool pool in _root.PoolSet) {
                        foreach (ICoinKernel coinKernel in _root.CoinKernelSet.Where(a => a.CoinId == pool.CoinId)) {
                            PoolKernelData poolKernel = list.FirstOrDefault(a => a.PoolId == pool.GetId() && a.KernelId == coinKernel.KernelId);
                            if (poolKernel != null) {
                                _dicById.Add(poolKernel.GetId(), poolKernel);
                            }
                            else {
                                Guid poolKernelId = Guid.NewGuid();
                                _dicById.Add(poolKernelId, new PoolKernelData() {
                                    Id = poolKernelId,
                                    Args = string.Empty,
                                    Description = string.Empty,
                                    KernelId = coinKernel.KernelId,
                                    PoolId = pool.GetId()
                                });
                            }
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

        public bool TryGetPoolKernel(Guid kernelId, out IPoolKernel kernel) {
            InitOnece();
            PoolKernelData k;
            var r = _dicById.TryGetValue(kernelId, out k);
            kernel = k;
            return r;
        }

        public IEnumerator<IPoolKernel> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
