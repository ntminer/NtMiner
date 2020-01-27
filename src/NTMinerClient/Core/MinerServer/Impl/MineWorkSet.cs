using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class MineWorkSet : IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();

        public MineWorkSet() {
            VirtualRoot.AddCmdPath<AddMineWorkCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                MineWorkData entity = new MineWorkData().Update(message.Input);
                var response = RpcRoot.Server.MineWorkService.AddOrUpdateMineWork(entity);
                if (response.IsSuccess()) {
                    _dicById.Add(entity.Id, entity);
                    VirtualRoot.RaiseEvent(new MineWorkAddedEvent(message.Id, entity));
                }
                else {
                    Write.UserFail(response?.Description);
                }
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpdateMineWorkCommand>(action: (message) => {
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
                RpcRoot.Server.MineWorkService.AddOrUpdateMineWorkAsync(entity, (response, exception) => {
                    if (!response.IsSuccess()) {
                        entity.Update(oldValue);
                        VirtualRoot.RaiseEvent(new MineWorkUpdatedEvent(message.Id, entity));
                        Write.UserFail(response.ReadMessage(exception));
                    }
                });
                VirtualRoot.RaiseEvent(new MineWorkUpdatedEvent(message.Id, entity));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<RemoveMineWorkCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                MineWorkData entity = _dicById[message.EntityId];
                RpcRoot.Server.MineWorkService.RemoveMineWorkAsync(entity.Id, (response, exception) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new MineWorkRemovedEvent(message.Id, entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(exception));
                    }
                });
            }, location: this.GetType());
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var result = RpcRoot.Server.MineWorkService.GetMineWorks();
                    foreach (var item in result) {
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
            var r = _dicById.TryGetValue(mineWorkId, out MineWorkData w);
            mineWork = w;
            return r;
        }

        public bool Contains(Guid mineWorkId) {
            InitOnece();
            return _dicById.ContainsKey(mineWorkId);
        }

        public IEnumerable<IMineWork> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
