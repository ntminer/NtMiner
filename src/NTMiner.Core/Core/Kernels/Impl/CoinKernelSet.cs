using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class CoinKernelSet : ICoinKernelSet {

        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, CoinKernelData> _dicById = new Dictionary<Guid, CoinKernelData>();

        public CoinKernelSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddCoinKernelCommand>(
                Guid.Parse("6345c411-4860-433b-ad5e-3a743bcebfa8"),
                "添加币种内核",
                LogEnum.Log,
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
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>();
                    repository.Add(entity);

                    Global.Happened(new CoinKernelAddedEvent(entity));
                });
            Global.Access<UpdateCoinKernelCommand>(
                Guid.Parse("b3dfdf09-f732-4b3b-aeeb-25de7b83d30c"),
                "更新币种内核",
                LogEnum.Log,
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
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>();
                    repository.Update(entity);

                    Global.Happened(new CoinKernelUpdatedEvent(entity));
                });
            Global.Access<RemoveCoinKernelCommand>(
                Guid.Parse("ee34113f-e616-421d-adcc-c2e810723035"),
                "移除币种内核",
                LogEnum.Log,
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
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>();
                    repository.Remove(entity.Id);

                    Global.Happened(new CoinKernelRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateServerRepository<CoinKernelData>();
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
