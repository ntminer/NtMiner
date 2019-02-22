using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class ColumnsShowSet : IColumnsShowSet {
        private readonly Dictionary<Guid, ColumnsShowData> _dicById = new Dictionary<Guid, ColumnsShowData>();

        private readonly INTMinerRoot _root;
        public ColumnsShowSet(INTMinerRoot root) {
            _root = root;
            ICoin coin = root.CoinSet.FirstOrDefault();
            VirtualRoot.Accept<AddColumnsShowCommand>(
                "添加列显",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty || message.Input.GetId() == ColumnsShowData.PleaseSelectId) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    ColumnsShowData entity = new ColumnsShowData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    Server.ControlCenterService.AddOrUpdateColumnsShowAsync(entity, isSuccess => {
                        VirtualRoot.Happened(new ColumnsShowAddedEvent(entity));
                    });
                });
            VirtualRoot.Accept<UpdateColumnsShowCommand>(
                "更新列显",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    ColumnsShowData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    Server.ControlCenterService.AddOrUpdateColumnsShowAsync(entity, isSuccess => {
                        VirtualRoot.Happened(new ColumnsShowUpdatedEvent(entity));
                    });
                });
            VirtualRoot.Accept<RemoveColumnsShowCommand>(
                "移除列显",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty || message.EntityId == ColumnsShowData.PleaseSelectId) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    ColumnsShowData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    Server.ControlCenterService.RemoveColumnsShowAsync(entity.Id, isSuccess => {
                        VirtualRoot.Happened(new ColumnsShowRemovedEvent(entity));
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
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        Guid messageId = Guid.NewGuid();
                        var response = Server.ControlCenterService.GetColumnsShows(messageId);
                        if (response != null) {
                            foreach (var item in response.Data) {
                                if (!_dicById.ContainsKey(item.GetId())) {
                                    _dicById.Add(item.GetId(), item);
                                }
                            }
                        }
                        if (!_dicById.ContainsKey(ColumnsShowData.PleaseSelectId)) {
                            var entity = new ColumnsShowData {
                                ColumnsShowName = "请选择",
                                Id = ColumnsShowData.PleaseSelectId
                            };
                            _dicById.Add(ColumnsShowData.PleaseSelectId, entity);
                            Server.ControlCenterService.AddOrUpdateColumnsShowAsync(entity, respon => {
                                if (!respon.IsSuccess()) {
                                    Logger.ErrorDebugLine("AddOrUpdateColumnsShowAsync " + respon.Description);
                                }
                            });
                        }
                        _isInited = true;
                    }
                }
            }
        }

        public bool Contains(Guid columnsShowId) {
            InitOnece();
            return _dicById.ContainsKey(columnsShowId);
        }

        public bool TryGetColumnsShow(Guid columnsShowId, out IColumnsShow columnsShow) {
            InitOnece();
            ColumnsShowData g;
            var r = _dicById.TryGetValue(columnsShowId, out g);
            columnsShow = g;
            return r;
        }

        public IEnumerator<IColumnsShow> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
