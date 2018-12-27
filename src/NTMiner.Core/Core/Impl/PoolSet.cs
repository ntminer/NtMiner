using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    internal class PoolSet : IPoolSet {

        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, PoolData> _dicById = new Dictionary<Guid, PoolData>();

        public PoolSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddPoolCommand>(
                Guid.Parse("5ee1b14b-4b9e-445f-b6fe-433f6fe44b18"),
                "添加矿池",
                LogEnum.Log,
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
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>();
                    repository.Add(entity);

                    Global.Happened(new PoolAddedEvent(entity));
                });
            Global.Access<UpdatePoolCommand>(
                Guid.Parse("62d847f6-2b1f-4891-990b-3beb4c1dc5b0"),
                "更新矿池",
                LogEnum.Log,
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
                    // 组合GlobalDb和ProfileDb，Profile用户无权修改GlobalDb中的数据，否则抛出异常终端流程从而确保GlobalDb中的数据不会被后续的流程修改
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>();
                    repository.Update(new PoolData().Update(message.Input));
                    PoolData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);

                    Global.Happened(new PoolUpdatedEvent(entity));
                });
            Global.Access<RemovePoolCommand>(
                Guid.Parse("c5ce3c6c-78c4-4e76-81e3-2feeac5d5ced"),
                "移除矿池",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    // 组合GlobalDb和ProfileDb，Profile用户无权删除GlobalDb中的数据，否则抛出异常终端流程从而确保GlobalDb中的数据不会被后续的流程修改
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>();
                    repository.Remove(message.EntityId);
                    PoolData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());

                    Global.Happened(new PoolRemovedEvent(entity));
                });
            BootLog.Log(this.GetType().FullName + "接入总线");
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
                    var repository = NTMinerRoot.CreateCompositeRepository<PoolData>();
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
