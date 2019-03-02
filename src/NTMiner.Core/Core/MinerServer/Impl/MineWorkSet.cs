using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class MineWorkSet : IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();
        
        private readonly INTMinerRoot _root;
        public MineWorkSet(INTMinerRoot root) {
            _root = root;
            ICoin coin = root.CoinSet.FirstOrDefault();
            VirtualRoot.Accept<AddMineWorkCommand>(
                "添加工作",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    MineWorkData entity = new MineWorkData().Update(message.Input);
                    Server.ControlCenterService.AddOrUpdateMineWorkAsync(entity, response=> {
                        if (response.IsSuccess()) {
                            _dicById.Add(entity.Id, entity);
                            VirtualRoot.Happened(new MineWorkAddedEvent(entity));
                        }
                        else if (response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                        }
                    });
                });
            VirtualRoot.Accept<UpdateMineWorkCommand>(
                "更新工作",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    MineWorkData entity = _dicById[message.Input.GetId()];
                    MineWorkData oldValue = new MineWorkData().Update(entity);
                    entity.Update(message.Input);
                    Server.ControlCenterService.AddOrUpdateMineWorkAsync(entity, response => {
                        if (!response.IsSuccess()) {
                            entity.Update(oldValue);
                            VirtualRoot.Happened(new MineWorkUpdatedEvent(entity));
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                            }
                        }
                    });
                    VirtualRoot.Happened(new MineWorkUpdatedEvent(entity));
                });
            VirtualRoot.Accept<RemoveMineWorkCommand>(
                "移除工作",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    MineWorkData entity = _dicById[message.EntityId];
                    Server.ControlCenterService.RemoveMineWorkAsync(entity.Id, response => {
                        if (response.IsSuccess()) {
                            _dicById.Remove(entity.Id);
                            VirtualRoot.Happened(new MineWorkRemovedEvent(entity));
                        }
                        else if (response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                        }
                    });
                });
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
                    foreach (var item in Server.ProfileService.GetMineWorks()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                        else {
                            _dicById[item.GetId()].Update(item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetMineWork(Guid mineWorkId, out IMineWork mineWork) {
            InitOnece();
            MineWorkData w;
            var r = _dicById.TryGetValue(mineWorkId, out w);
            mineWork = w;
            return r;
        }

        public bool Contains(Guid mineWorkId) {
            InitOnece();
            return _dicById.ContainsKey(mineWorkId);
        }

        public IEnumerator<IMineWork> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
