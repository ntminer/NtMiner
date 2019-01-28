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
            Global.Access<RefreshPoolKernelSetCommand>(
                Guid.Parse("241152F1-536C-4773-AF3B-1A2B4E99D3E8"),
                "处理刷新矿池内核数据集命令",
                LogEnum.Console,
                action: message => {
                    var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                    foreach (var item in repository.GetAll()) {
                        if (_dicById.ContainsKey(item.Id)) {
                            Global.Execute(new UpdatePoolKernelCommand(item));
                        }
                        else {
                            Global.Execute(new AddPoolKernelCommand(item));
                        }
                    }
                });
            Global.Access<AddPoolKernelCommand>(
                Guid.Parse("B926F4A0-4318-493D-BCE1-CBE329AC7DD7"),
                "处理添加矿池级内核命令",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = new PoolKernelData().Update(message.Input);
                        _dicById.Add(message.Input.GetId(), entity);
                        var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                        repository.Add(entity);
                        Global.Happened(new PoolKernelAddedEvent(message.Input));
                    }
                });
            Global.Access<RemovePoolKernelCommand>(
                Guid.Parse("151EC414-58E3-4752-8758-4742256D2297"),
                "处理移除矿池级内核命令",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.EntityId)) {
                        var entity = _dicById[message.EntityId];
                        _dicById.Remove(message.EntityId);
                        var repository = NTMinerRoot.CreateServerRepository<PoolKernelData>();
                        repository.Remove(message.EntityId);
                        Global.Happened(new PoolKernelRemovedEvent(entity));
                    }
                });
            Global.Access<UpdatePoolKernelCommand>(
                Guid.Parse("08843B3B-3F82-45D2-8B45-6B24F397A326"),
                "更新矿池内核",
                LogEnum.Console,
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
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
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
