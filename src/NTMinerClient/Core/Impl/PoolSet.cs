using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    internal class PoolSet : IPoolSet {
        private class PoolDelay {
            public string MainCoinPoolDelayText;
            public string DualCoinPoolDelayText;
        }

        private readonly Dictionary<Guid, PoolData> _dicById = new Dictionary<Guid, PoolData>();
        private readonly Dictionary<Guid, PoolDelay> _poolDelayById = new Dictionary<Guid, PoolDelay>();

        private readonly IServerContext _context;
        public PoolSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddPoolCommand>("添加矿池", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!context.CoinSet.Contains(message.Input.CoinId)) {
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

                    var repository = context.CreateCompositeRepository<PoolData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new PoolAddedEvent(message.MessageId, entity));

                    if (context.CoinSet.TryGetCoin(message.Input.CoinId, out ICoin coin)) {
                        ICoinKernel[] coinKernels = context.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == coin.GetId()).ToArray();
                        foreach (ICoinKernel coinKernel in coinKernels) {
                            Guid poolKernelId = Guid.NewGuid();
                            var poolKernel = new PoolKernelData() {
                                Id = poolKernelId,
                                Args = string.Empty,
                                KernelId = coinKernel.KernelId,
                                PoolId = message.Input.GetId()
                            };
                            VirtualRoot.Execute(new AddPoolKernelCommand(poolKernel));
                        }
                    }
                }, location: this.GetType());
            context.AddCmdPath<UpdatePoolCommand>("更新矿池", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!context.CoinSet.Contains(message.Input.CoinId)) {
                        throw new ValidationException("there is no coin with id " + message.Input.CoinId);
                    }
                    if (string.IsNullOrEmpty(message.Input.Server)) {
                        throw new ValidationException("pool poolurl can't be null or empty");
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("pool name can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out PoolData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateCompositeRepository<PoolData>();
                    repository.Update(new PoolData().Update(message.Input));

                    VirtualRoot.RaiseEvent(new PoolUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemovePoolCommand>("移除矿池", LogEnum.DevConsole,
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
                    var repository = context.CreateCompositeRepository<PoolData>();
                    repository.Remove(message.EntityId);
                    VirtualRoot.RaiseEvent(new PoolRemovedEvent(message.MessageId, entity));
                    Guid[] toRemoves = context.PoolKernelSet.AsEnumerable().Where(a => a.PoolId == message.EntityId).Select(a => a.GetId()).ToArray();
                    foreach (Guid poolKernelId in toRemoves) {
                        VirtualRoot.Execute(new RemovePoolKernelCommand(poolKernelId));
                    }
                }, location: this.GetType());
            VirtualRoot.AddEventPath<PoolDelayPickedEvent>("提取了矿池延时后记录进内存", LogEnum.DevConsole,
                action: message => {
                    if (message.IsDual) {
                        if (_poolDelayById.TryGetValue(message.PoolId, out PoolDelay poolDelay)) {
                            poolDelay.DualCoinPoolDelayText = message.PoolDelayText;
                        }
                        else {
                            _poolDelayById.Add(message.PoolId, new PoolDelay {
                                MainCoinPoolDelayText = string.Empty,
                                DualCoinPoolDelayText = message.PoolDelayText
                            });
                        }
                    }
                    else {
                        if (_poolDelayById.TryGetValue(message.PoolId, out PoolDelay poolDelay)) {
                            poolDelay.MainCoinPoolDelayText = message.PoolDelayText;
                        }
                        else {
                            _poolDelayById.Add(message.PoolId, new PoolDelay {
                                MainCoinPoolDelayText = message.PoolDelayText,
                                DualCoinPoolDelayText = string.Empty
                            });
                        }
                    }
                }, location: this.GetType());
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = _context.CreateCompositeRepository<PoolData>();
                    List<PoolData> data = repository.GetAll().ToList();
                    foreach (var item in data) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid poolId) {
            InitOnece();
            return _dicById.ContainsKey(poolId);
        }

        public bool TryGetPool(Guid poolId, out IPool pool) {
            InitOnece();
            var r = _dicById.TryGetValue(poolId, out PoolData p);
            pool = p;
            return r;
        }

        public string GetPoolDelayText(Guid poolId, bool isDual) {
            InitOnece();
            if (_poolDelayById.TryGetValue(poolId, out PoolDelay poolDelay)) {
                if (isDual) {
                    return poolDelay.DualCoinPoolDelayText;
                }
                return poolDelay.MainCoinPoolDelayText;
            }
            return string.Empty;
        }

        public IEnumerable<IPool> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
