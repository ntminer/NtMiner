using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class MinerGroupSet : IMinerGroupSet {
        private readonly Dictionary<Guid, MinerGroupData> _dicById = new Dictionary<Guid, MinerGroupData>();
        private readonly INTMinerRoot _root;

        public MinerGroupSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Accept<AddMinerGroupCommand>(
                "添加矿工组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("minerGroup name can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    MinerGroupData entity = new MinerGroupData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    Server.ControlCenterService.AddOrUpdateMinerGroupAsync(entity, response => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Happened(new MinerGroupAddedEvent(entity));
                        }
                        else {
                            Write.UserLine("新建或更新矿工组失败", ConsoleColor.Red);
                        }
                    });

                });
            VirtualRoot.Accept<UpdateMinerGroupCommand>(
                "更新矿工组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("minerGroup name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    MinerGroupData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    Server.ControlCenterService.AddOrUpdateMinerGroupAsync(entity, isSuccess=> {
                        VirtualRoot.Happened(new MinerGroupUpdatedEvent(entity));
                    });
                });
            VirtualRoot.Accept<RemoveMinerGroupCommand>(
                "移除矿工组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    MinerGroupData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    Server.ControlCenterService.RemoveMinerGroupAsync(entity.Id, isSuccess=> {
                        VirtualRoot.Happened(new MinerGroupRemovedEvent(entity));
                    });
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
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        Guid messageId = Guid.NewGuid();
                        var response = Server.ControlCenterService.GetMinerGroups(messageId);
                        if (response != null) {
                            foreach (var item in response.Data) {
                                if (!_dicById.ContainsKey(item.GetId())) {
                                    _dicById.Add(item.GetId(), item);
                                }
                            }
                        }
                        _isInited = true;
                    }
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetMinerGroup(Guid id, out IMinerGroup group) {
            InitOnece();
            MinerGroupData g;
            var r = _dicById.TryGetValue(id, out g);
            group = g;
            return r;
        }

        public IEnumerator<IMinerGroup> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
