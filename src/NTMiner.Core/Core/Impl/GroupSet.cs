using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GroupSet : IGroupSet {
        private readonly Dictionary<Guid, GroupData> _dicById = new Dictionary<Guid, GroupData>();

        private readonly INTMinerRoot _root;
        public GroupSet(INTMinerRoot root) {
            _root = root;
            Global.Access<RefreshGroupSetCommand>(
                Guid.Parse("CE28E5E9-8908-433E-BF0A-43BC7E5B60E1"),
                "处理刷新组数据集命令",
                LogEnum.Console,
                action: message => {
                    _isInited = false;
                    Init();
                });
            Global.Access<AddGroupCommand>(
                Guid.Parse("e0c313ff-2550-41f8-9403-8575638c7faf"),
                "添加组",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>();
                    repository.Add(entity);

                    Global.Happened(new GroupAddedEvent(entity));
                });
            Global.Access<UpdateGroupCommand>(
                Guid.Parse("b2d190dd-b60d-41f9-8e93-65902c318a78"),
                "更新组",
                LogEnum.Console,
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
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>();
                    repository.Update(entity);

                    Global.Happened(new GroupUpdatedEvent(entity));
                });
            Global.Access<RemoveGroupCommand>(
                Guid.Parse("7dede0b5-be81-4fc1-bee1-cdeb6afa7b72"),
                "移除组",
                LogEnum.Console,
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
                        Global.Execute(new RemoveCoinGroupCommand(id));
                    }
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>();
                    repository.Remove(message.EntityId);

                    Global.Happened(new GroupRemovedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
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
                    _dicById.Clear();
                    var repository = NTMinerRoot.CreateServerRepository<GroupData>();
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
