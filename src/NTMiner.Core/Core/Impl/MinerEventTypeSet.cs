using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MinerEventTypeSet : IMinerEventTypeSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, MinerEventTypeData> _dicById = new Dictionary<Guid, MinerEventTypeData>();

        private readonly bool _isUseJson;

        public MinerEventTypeSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddMinerEventTypeCommand>("添加事件类型", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("EventType name can't be null or empty");
                    }
                    MinerEventTypeData entity = new MinerEventTypeData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateCompositeRepository<MinerEventTypeData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new MinerEventTypeAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdateMinerEventTypeCommand>("更新事件类型", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("EventType name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    MinerEventTypeData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateCompositeRepository<MinerEventTypeData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new MinerEventTypeUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemoveMinerEventTypeCommand>("移除事件类型", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    MinerEventTypeData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateCompositeRepository<MinerEventTypeData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new MinerEventTypeRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateCompositeRepository<MinerEventTypeData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(string name) {
            InitOnece();
            return _dicById.Values.Any(a => a.Name == name);
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetEventType(Guid id, out IMinerEventType eventType) {
            InitOnece();
            var result = _dicById.TryGetValue(id, out MinerEventTypeData data);
            eventType = data;
            return result;
        }

        public IEnumerator<IMinerEventType> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
