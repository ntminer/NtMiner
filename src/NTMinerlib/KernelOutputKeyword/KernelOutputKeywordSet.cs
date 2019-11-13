using LiteDB;
using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.KernelOutputKeyword {
    public class KernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();
        private readonly string _dbFileFullName;
        private readonly bool _isServer;

        public KernelOutputKeywordSet(string dbFileFullName, bool isServer) {
            _dbFileFullName = dbFileFullName;
            _isServer = isServer;
            VirtualRoot.BuildCmdPath<AddOrUpdateKernelOutputKeywordCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.MessageType)) {
                    throw new ValidationException("MessageType can't be null or empty");
                }
                if (string.IsNullOrEmpty(message.Input.Keyword)) {
                    throw new ValidationException("Keyword can't be null or empty");
                }
                if (_dicById.Values.Any(a => a.KernelOutputId == message.Input.KernelOutputId && a.Keyword == message.Input.Keyword && a.Id != message.Input.GetId())) {
                    throw new ValidationException($"关键字{message.Input.Keyword}已存在");
                }
                if (_dicById.TryGetValue(message.Input.GetId(), out KernelOutputKeywordData exist)) {
                    exist.Update(message.Input);
                    using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                        var col = db.GetCollection<KernelOutputKeywordData>();
                        col.Update(exist);
                    }
                }
                else {
                    KernelOutputKeywordData entity = new KernelOutputKeywordData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                        var col = db.GetCollection<KernelOutputKeywordData>();
                        col.Insert(entity);
                    }
                }
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
                                item.SetDataLevel(DataLevel.Profile);
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
