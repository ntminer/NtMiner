using NTMiner.OverClock;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class OverClockDataSet : IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();
        private readonly INTMinerRoot _root;

        public OverClockDataSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Accept<AddOverClockDataCommand>(
                "添加超频建议",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("OverClockData name can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    OverClockData entity = new OverClockData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    Server.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, response => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Happened(new OverClockDataAddedEvent(entity));
                        }
                        else {
                            Write.UserLine("新建或更新超频建议失败", ConsoleColor.Red);
                        }
                    });
                });
            VirtualRoot.Accept<UpdateOverClockDataCommand>(
                "更新超频建议",
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
                    OverClockData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    Server.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, isSuccess => {
                        VirtualRoot.Happened(new OverClockDataUpdatedEvent(entity));
                    });
                });
            VirtualRoot.Accept<RemoveOverClockDataCommand>(
                "移除超频建议",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    OverClockData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    Server.OverClockDataService.RemoveOverClockDataAsync(entity.Id, isSuccess => {
                        VirtualRoot.Happened(new OverClockDataRemovedEvent(entity));
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
                        var response = Server.OverClockDataService.GetOverClockDatas(messageId);
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

        public bool TryGetOverClockData(Guid id, out IOverClockData group) {
            InitOnece();
            OverClockData g;
            var r = _dicById.TryGetValue(id, out g);
            group = g;
            return r;
        }

        public IEnumerator<IOverClockData> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
