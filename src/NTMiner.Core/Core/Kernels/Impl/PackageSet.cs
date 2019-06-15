using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class PackageSet : IPackageSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, PackageData> _dicById = new Dictionary<Guid, PackageData>();

        private readonly bool _isUseJson;
        public PackageSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddPackageCommand>("添加包", LogEnum.DevConsole,
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
                    IRepository<PackageData> repository = NTMinerRoot.CreateServerRepository<PackageData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new PackageAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdatePackageCommand>("更新包", LogEnum.DevConsole,
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
                    IRepository<PackageData> repository = NTMinerRoot.CreateServerRepository<PackageData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new PackageUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemovePackageCommand>("移除包", LogEnum.DevConsole,
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
                    IRepository<PackageData> repository = NTMinerRoot.CreateServerRepository<PackageData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new PackageRemovedEvent(entity));
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
                    IRepository<PackageData> repository = NTMinerRoot.CreateServerRepository<PackageData>(_isUseJson);
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
            PackageData pkg;
            var r = _dicById.TryGetValue(packageId, out pkg);
            package = pkg;
            return r;
        }

        public IEnumerator<IPackage> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
