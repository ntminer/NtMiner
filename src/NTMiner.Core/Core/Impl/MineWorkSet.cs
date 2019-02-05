using NTMiner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MineWorkSet : IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();

        private readonly INTMinerRoot _root;
        public MineWorkSet(INTMinerRoot root) {
            _root = root;
            ICoin coin = root.CoinSet.FirstOrDefault();
            Global.Access<AddMineWorkCommand>(
                Guid.Parse("2ce02224-8ddf-4499-9d1d-7439ba5ca2fc"),
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
                    _dicById.Add(entity.Id, entity);
                    Server.ControlCenterService.AddOrUpdateMineWorkAsync(entity, isSuccess=> {
                        Global.Happened(new MineWorkAddedEvent(entity));
                    });
                });
            Global.Access<UpdateMineWorkCommand>(
                Guid.Parse("21140dbe-c9be-48d6-ae92-4d0ebc666a25"),
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
                    entity.Update(message.Input);
                    Server.ControlCenterService.AddOrUpdateMineWorkAsync(entity, isSuccess=> {
                        Global.Happened(new MineWorkUpdatedEvent(entity));
                    });
                });
            Global.Access<RemoveMineWorkCommand>(
                Guid.Parse("cec3ccf4-9700-4e38-b786-8ceefe5209fb"),
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
                    _dicById.Remove(entity.Id);
                    Server.ControlCenterService.RemoveMineWorkAsync(entity.Id, isSuccess=> {
                        Global.Happened(new MineWorkRemovedEvent(entity));
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
