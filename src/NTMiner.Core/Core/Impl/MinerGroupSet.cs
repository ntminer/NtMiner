using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class MinerGroupSet : IMinerGroupSet {
        private readonly Dictionary<Guid, MinerGroupData> _dicById = new Dictionary<Guid, MinerGroupData>();
        private readonly INTMinerRoot _root;

        public MinerGroupSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddMinerGroupCommand>(
                Guid.Parse("051DE144-1C91-4633-B826-EDFBE951B450"),
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
                            Global.Happened(new MinerGroupAddedEvent(entity));
                        }
                    });

                });
            Global.Access<UpdateMinerGroupCommand>(
                Guid.Parse("BC6ADC0E-E57C-4313-8C85-D866E2068913"),
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
                        Global.Happened(new MinerGroupUpdatedEvent(entity));
                    });
                });
            Global.Access<RemoveMinerGroupCommand>(
                Guid.Parse("3083F1E6-0932-484E-AD2F-BDEA2790FD44"),
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
                        Global.Happened(new MinerGroupRemovedEvent(entity));
                    });
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

        public bool TryGetCoin(Guid id, out IMinerGroup group) {
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
