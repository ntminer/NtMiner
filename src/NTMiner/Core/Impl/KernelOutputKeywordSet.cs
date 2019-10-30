using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class KernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();

        public KernelOutputKeywordSet(INTMinerRoot root) {
            _root = root;
            _root.ServerContextCmdPath<AddKernelOutputKeywordCommand>("添加内核输出关键字", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.MessageType)) {
                        throw new ValidationException("WorkerMessageType can't be null or empty");
                    }
                    if (string.IsNullOrEmpty(message.Input.Keyword)) {
                        throw new ValidationException("Keyword can't be null or empty");
                    }
                    if (_dicById.Values.Any(a => a.Keyword == message.Input.Keyword && a.Id != message.Input.GetId())) {
                        throw new ValidationException($"关键字{message.Input.Keyword}已存在");
                    }
                    KernelOutputKeywordData entity = new KernelOutputKeywordData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateLocalRepository<KernelOutputKeywordData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputKeywordAddedEvent(entity));
                });
            _root.ServerContextCmdPath<UpdateKernelOutputKeywordCommand>("更新内核输出关键字", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.MessageType)) {
                        throw new ValidationException("WorkerMessageType can't be null or empty");
                    }
                    if (string.IsNullOrEmpty(message.Input.Keyword)) {
                        throw new ValidationException("Keyword can't be null or empty");
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
                    var repository = NTMinerRoot.CreateLocalRepository<KernelOutputKeywordData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputKeywordUpdatedEvent(entity));
                });
            _root.ServerContextCmdPath<RemoveKernelOutputKeywordCommand>("移除内核输出关键字", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateLocalRepository<KernelOutputKeywordData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new KernelOutputKeywordRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateLocalRepository<KernelOutputKeywordData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            InitOnece();
            return _dicById.Values.Where(a => a.KernelOutputId == kernelOutputId);
        }

        public bool Contains(Guid kernelOutputId, string keyword) {
            InitOnece();
            return _dicById.Values.Any(a => a.KernelOutputId == kernelOutputId && a.Keyword == keyword);
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
