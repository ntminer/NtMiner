using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class PackageSet : SetBase, IPackageSet {
        private readonly Dictionary<Guid, PackageData> _dicById = new Dictionary<Guid, PackageData>();

        private readonly IServerContext _context;
        public PackageSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddPackageCommand>("添加包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException($"{nameof(message.Input.Name)} can't be null or empty");
                    }
                    if (_dicById.Values.Any(a => string.Equals(message.Input.Name, a.Name, StringComparison.OrdinalIgnoreCase))) {
                        throw new ValidationException("包名重复");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    PackageData entity = new PackageData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = context.CreateServerRepository<PackageData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new PackageAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdatePackageCommand>("更新包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException($"{nameof(message.Input.Name)} can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out PackageData entity)) {
                        return;
                    }
                    if (_dicById.Values.Any(a => a.Id != message.Input.Id && string.Equals(message.Input.Name, a.Name, StringComparison.OrdinalIgnoreCase))) {
                        throw new ValidationException("包名重复");
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<PackageData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new PackageUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemovePackageCommand>("移除包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    PackageData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    var repository = context.CreateServerRepository<PackageData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new PackageRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<PackageData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
            }
        }

        public bool Contains(Guid packageId) {
            InitOnece();
            return _dicById.ContainsKey(packageId);
        }

        public bool TryGetPackage(Guid packageId, out IPackage package) {
            InitOnece();
            var r = _dicById.TryGetValue(packageId, out PackageData pkg);
            package = pkg;
            return r;
        }

        public IEnumerable<IPackage> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
