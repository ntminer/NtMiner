using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class OverClockDataSet : IOverClockDataSet {
        private readonly Dictionary<Guid, OverClockData> _dicById = new Dictionary<Guid, OverClockData>();
        private readonly INTMinerRoot _root;

        public OverClockDataSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Window<AddOverClockDataCommand>("添加超频建议", LogEnum.DevConsole,
                action: (message) => {
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
                        else {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                });
            VirtualRoot.Window<UpdateOverClockDataCommand>("更新超频建议", LogEnum.DevConsole,
                action: (message) => {
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
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                    VirtualRoot.Happened(new OverClockDataUpdatedEvent(entity));
                });
            VirtualRoot.Window<RemoveOverClockDataCommand>("移除超频建议", LogEnum.DevConsole,
                action: (message) => {
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
                        else {
                            Write.UserFail(response.ReadMessage(e));
                        }
                    });
                });
        }

        private bool _isInited = false;
        private void Init() {
            if (_isInited) {
                return;
            }
            _isInited = true;
            OfficialServer.OverClockDataService.GetOverClockDatasAsync((response, e) => {
                if (response.IsSuccess()) {
                    IEnumerable<OverClockData> query;
                    if (_root.GpuSet.GpuType == GpuType.Empty) {
                        query = response.Data;
                    }
                    else {
                        query = response.Data.Where(a => a.GpuType == _root.GpuSet.GpuType);
                    }
                    foreach (var item in query) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                }
                VirtualRoot.Happened(new OverClockDataSetInitedEvent());
            });
        }

        public bool TryGetOverClockData(Guid id, out IOverClockData data) {
            Init();
            var r = _dicById.TryGetValue(id, out OverClockData temp);
            data = temp;
            return r;
        }

        public IEnumerator<IOverClockData> GetEnumerator() {
            Init();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            Init();
            return _dicById.Values.GetEnumerator();
        }
    }
}
