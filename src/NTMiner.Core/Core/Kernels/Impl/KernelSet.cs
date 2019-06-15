using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelSet : IKernelSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, KernelData> _dicById = new Dictionary<Guid, KernelData>();

        private readonly bool _isUseJson;
        public KernelSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddKernelCommand>("添加内核", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException($"{nameof(message.Input.Code)} can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = new KernelData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new KernelAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdateKernelCommand>("更新内核", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException($"{nameof(message.Input.Code)} can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemoveKernelCommand>("移除内核", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelData entity = _dicById[message.EntityId];
                    List<Guid> coinKernelIds = root.CoinKernelSet.Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var coinKernelId in coinKernelIds) {
                        VirtualRoot.Execute(new RemoveCoinKernelCommand(coinKernelId));
                    }
                    _dicById.Remove(entity.Id);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new KernelRemovedEvent(entity));
                });
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
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>(_isUseJson);
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

        public bool TryGetKernel(Guid kernelId, out IKernel package) {
            InitOnece();
            KernelData pkg;
            var r = _dicById.TryGetValue(kernelId, out pkg);
            package = pkg;
            return r;
        }

        public IEnumerator<IKernel> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
