using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class KernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();

        private readonly bool _isUseJson;

        public KernelOutputKeywordSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddKernelOutputKeywordCommand>("添加内核输出关键字", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.Keyword)) {
                        throw new ValidationException("EventType name can't be null or empty");
                    }
                    if (_dicById.Values.Any(a => a.Keyword == message.Input.Keyword && a.Id != message.Input.GetId())) {
                        throw new ValidationException($"关键字{message.Input.Keyword}已存在");
                    }
                    KernelOutputKeywordData entity = new KernelOutputKeywordData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateCompositeRepository<KernelOutputKeywordData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new KernelOutputKeywordAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdateKernelOutputKeywordCommand>("更新内核输出关键字", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Keyword)) {
                        throw new ValidationException("EventType name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicById.Values.Any(a => a.Keyword == message.Input.Keyword && a.Id != message.Input.GetId())) {
                        throw new ValidationException($"关键字{message.Input.Keyword}已存在");
                    }
                    KernelOutputKeywordData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateCompositeRepository<KernelOutputKeywordData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelOutputKeywordUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemoveKernelOutputKeywordCommand>("移除内核输出关键字", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelOutputKeywordData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateCompositeRepository<KernelOutputKeywordData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new KernelOutputKeywordRemovedEvent(entity));
                });
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
                    var repository = NTMinerRoot.CreateCompositeRepository<KernelOutputKeywordData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(string keyword) {
            InitOnece();
            return _dicById.Values.Any(a => a.Keyword == keyword);
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword) {
            InitOnece();
            var result = _dicById.TryGetValue(id, out KernelOutputKeywordData data);
            keyword = data;
            return result;
        }

        public IEnumerator<IKernelOutputKeyword> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
