using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class OverClockDataSet : IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();
        private readonly INTMinerRoot _root;

        public OverClockDataSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Window<AddOverClockDataCommand>("添加超频建议", LogEnum.DevConsole,
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
                    OfficialServer.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, (response, e) => {
                        if (response.IsSuccess()) {
                            _dicById.Add(entity.Id, entity);
                            VirtualRoot.Happened(new OverClockDataAddedEvent(entity));
                        }
                        else if(response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                        }
                    });
                });
            VirtualRoot.Window<UpdateOverClockDataCommand>("更新超频建议", LogEnum.DevConsole,
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
                    OverClockData oldValue = new OverClockData().Update(entity);
                    entity.Update(message.Input);
                    OfficialServer.OverClockDataService.AddOrUpdateOverClockDataAsync(entity, (response, e) => {
                        if (!response.IsSuccess()) {
                            entity.Update(oldValue);
                            VirtualRoot.Happened(new OverClockDataUpdatedEvent(entity));
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                            }
                        }
                    });
                    VirtualRoot.Happened(new OverClockDataUpdatedEvent(entity));
                });
            VirtualRoot.Window<RemoveOverClockDataCommand>("移除超频建议", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    OverClockData entity = _dicById[message.EntityId];
                    OfficialServer.OverClockDataService.RemoveOverClockDataAsync(entity.Id, (response, e) => {
                        if (response.IsSuccess()) {
                            _dicById.Remove(entity.Id);
                            VirtualRoot.Happened(new OverClockDataRemovedEvent(entity));
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
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        Guid messageId = Guid.NewGuid();
                        var result = OfficialServer.OverClockDataService.GetOverClockDatas(messageId);
                        foreach (var item in result) {
                            if (!_dicById.ContainsKey(item.GetId())) {
                                _dicById.Add(item.GetId(), item);
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
