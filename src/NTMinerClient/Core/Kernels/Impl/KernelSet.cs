using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelSet : IKernelSet {
        private readonly Dictionary<Guid, KernelData> _dicById = new Dictionary<Guid, KernelData>();

        public KernelSet(IServerContext context) {
            context.AddCmdPath<AddKernelCommand>("添加内核", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateKernelCommand>("更新内核", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveKernelCommand>("移除内核", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelData entity = _dicById[message.EntityId];
                    List<Guid> coinKernelIds = context.CoinKernelSet.AsEnumerable().Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var coinKernelId in coinKernelIds) {
                        VirtualRoot.Execute(new RemoveCoinKernelCommand(coinKernelId));
                    }
                    _dicById.Remove(entity.Id);
                    var repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new KernelRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelData>();
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
            var r = _dicById.TryGetValue(kernelId, out KernelData pkg);
            package = pkg;
            return r;
        }

        public IEnumerable<IKernel> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
