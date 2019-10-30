using LiteDB;
using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.KernelOutputKeyword {
    public class LocalKernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();
        private readonly string _dbFileFullName;

        public LocalKernelOutputKeywordSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.BuildCmdPath<AddKernelOutputKeywordCommand>(action: (message) => {
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
                using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                    var col = db.GetCollection<KernelOutputKeywordData>();
                    col.Upsert(entity);
                }

                VirtualRoot.RaiseEvent(new KernelOutputKeywordAddedEvent(entity));
            });
            VirtualRoot.BuildCmdPath<UpdateKernelOutputKeywordCommand>(action: (message) => {
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
                using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                    var col = db.GetCollection<KernelOutputKeywordData>();
                    col.Update(entity);
                }

                VirtualRoot.RaiseEvent(new KernelOutputKeywordUpdatedEvent(entity));
            });
            VirtualRoot.BuildCmdPath<RemoveKernelOutputKeywordCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                KernelOutputKeywordData entity = _dicById[message.EntityId];
                _dicById.Remove(entity.GetId());
                using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                    var col = db.GetCollection<KernelOutputKeywordData>();
                    col.Delete(message.EntityId);
                }

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
                    using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                        var col = db.GetCollection<KernelOutputKeywordData>();
                        foreach (var item in col.FindAll()) {
                            if (!_dicById.ContainsKey(item.GetId())) {
                                _dicById.Add(item.GetId(), item);
                            }
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
