using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelSet : SetBase, IKernelSet {
        private readonly Dictionary<Guid, KernelData> _dicById = new Dictionary<Guid, KernelData>();

        private readonly IServerContext _context;
        public KernelSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddKernelCommand>(LogEnum.DevConsole,
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
                    var repository = context.CreateServerRepository<KernelData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateKernelCommand>(LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException($"{nameof(message.Input.Code)} can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out KernelData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<KernelData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveKernelCommand>(LogEnum.DevConsole,
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
                    var repository = context.CreateServerRepository<KernelData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new KernelRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<KernelData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
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
