using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GroupSet : IGroupSet {
        private readonly Dictionary<Guid, GroupData> _dicById = new Dictionary<Guid, GroupData>();

        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;
        public GroupSet(INTMinerRoot root, bool isUseJson) {
            _isUseJson = isUseJson;
            _root = root;
            VirtualRoot.Window<AddGroupCommand>("添加组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    GroupData entity = new GroupData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new GroupAddedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdateGroupCommand>("更新组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("Group name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    GroupData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new GroupUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemoveGroupCommand>("移除组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    GroupData entity = _dicById[message.EntityId];
                    Guid[] toRemoves = root.CoinGroupSet.GetGroupCoinIds(entity.Id).ToArray();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemoveCoinGroupCommand(id));
                    }
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new GroupRemovedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
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
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>(_isUseJson);
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

        public bool Contains(Guid groupId) {
            InitOnece();
            return _dicById.ContainsKey(groupId);
        }

        public bool TryGetGroup(Guid groupId, out IGroup group) {
            InitOnece();
            GroupData g;
            bool r = _dicById.TryGetValue(groupId, out g);
            group = g;
            return r;
        }

        public IEnumerator<IGroup> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
