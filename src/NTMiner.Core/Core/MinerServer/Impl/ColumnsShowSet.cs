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
            VirtualRoot.Access<AddColumnsShowCommand>(
                Guid.Parse("A626192D-6C59-4D80-889A-809CC6D6B19A"),
                "添加列显",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
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
            VirtualRoot.Access<UpdateColumnsShowCommand>(
                Guid.Parse("9134E386-60E0-47FC-944B-8D95B083E45A"),
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
            VirtualRoot.Access<RemoveColumnsShowCommand>(
                Guid.Parse("FC0BDDF0-2308-4862-82A8-205B3404BFBD"),
                "移除列显",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
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
