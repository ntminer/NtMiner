using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class PoolKernelSet : IPoolKernelSet {
        private readonly IServerContext _context;
        private readonly Dictionary<Guid, PoolKernelData> _dicById = new Dictionary<Guid, PoolKernelData>();

        public PoolKernelSet(IServerContext context) {
            _context = context;
            _context.BuildCmdPath<AddPoolKernelCommand>("处理添加矿池级内核命令", LogEnum.DevConsole,
                action: message => {
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = new PoolKernelData().Update(message.Input);
                        _dicById.Add(message.Input.GetId(), entity);
                        var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                        repository.Add(entity);
                        VirtualRoot.RaiseEvent(new PoolKernelAddedEvent(message.Input));
                    }
                });
            _context.BuildCmdPath<RemovePoolKernelCommand>("处理移除矿池级内核命令", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.EntityId)) {
                        var entity = _dicById[message.EntityId];
                        _dicById.Remove(message.EntityId);
                        var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                        repository.Remove(message.EntityId);
                        VirtualRoot.RaiseEvent(new PoolKernelRemovedEvent(entity));
                    }
                });
            _context.BuildCmdPath<UpdatePoolKernelCommand>("更新矿池内核", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_context.PoolSet.Contains(message.Input.PoolId)) {
                        throw new ValidationException("there is no pool with id" + message.Input.PoolId);
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    PoolKernelData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new PoolKernelUpdatedEvent(entity));
                });
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

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
                    foreach (IPool pool in _context.PoolSet) {
                        foreach (ICoinKernel coinKernel in _context.CoinKernelSet.Where(a => a.CoinId == pool.CoinId)) {
                            PoolKernelData poolKernel = list.FirstOrDefault(a => a.PoolId == pool.GetId() && a.KernelId == coinKernel.KernelId);
                            if (poolKernel != null) {
                                _dicById.Add(poolKernel.GetId(), poolKernel);
                            }
                            else {
                                Guid poolKernelId = Guid.NewGuid();
                                _dicById.Add(poolKernelId, new PoolKernelData() {
                                    Id = poolKernelId,
                                    Args = string.Empty,
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
            var r = _dicById.TryGetValue(kernelId, out PoolKernelData k);
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
