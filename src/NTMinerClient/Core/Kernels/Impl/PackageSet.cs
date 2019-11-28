using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class PackageSet : IPackageSet {
        private readonly Dictionary<Guid, PackageData> _dicById = new Dictionary<Guid, PackageData>();

        public PackageSet(IServerContext context) {
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
                    var repository = NTMinerRoot.CreateServerRepository<PackageData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new PackageAddedEvent(message.Id, entity));
                });
            context.AddCmdPath<UpdatePackageCommand>("更新包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException($"{nameof(message.Input.Name)} can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicById.Values.Any(a => a.Id != message.Input.Id && string.Equals(message.Input.Name, a.Name, StringComparison.OrdinalIgnoreCase))) {
                        throw new ValidationException("包名重复");
                    }
                    PackageData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<PackageData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new PackageUpdatedEvent(message.Id, entity));
                });
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
                    var repository = NTMinerRoot.CreateServerRepository<PackageData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new PackageRemovedEvent(message.Id, entity));
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
                    var repository = NTMinerRoot.CreateServerRepository<PackageData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
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
