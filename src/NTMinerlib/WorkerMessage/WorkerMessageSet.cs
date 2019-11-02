using LiteDB;
using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.WorkerMessage {
    public class WorkerMessageSet : IEnumerable<IWorkerMessage> {
        private readonly string _connectionString;
        private readonly LinkedList<WorkerMessageData> _records = new LinkedList<WorkerMessageData>();

        public WorkerMessageSet(string dbFileFullName) {
            if (!string.IsNullOrEmpty(dbFileFullName)) {
                _connectionString = $"filename={dbFileFullName};journal=false";
            }
        }

        public int Count {
            get {
                InitOnece();
                return _records.Count;
            }
        }

        public void Add(string channel, string provider, string messageType, string content) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            InitOnece();
            var data = new WorkerMessageData {
                Id = Guid.NewGuid(),
                Channel = channel,
                Provider = provider,
                MessageType = messageType,
                Content = content,
                Timestamp = DateTime.Now
            };
            // TODO:批量持久化，异步持久化
            List<IWorkerMessage> removes = new List<IWorkerMessage>();
            lock (_locker) {
                _records.AddFirst(data);
                while (_records.Count > NTKeyword.WorkerMessageSetCapacity) {
                    var toRemove = _records.Last;
                    removes.Add(toRemove.Value);
                    _records.RemoveLast();
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<WorkerMessageData>();
                        col.Delete(toRemove.Value.Id);
                    }
                }
            }
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<WorkerMessageData>();
                col.Insert(data);
            }
            VirtualRoot.RaiseEvent(new WorkerMessageAddedEvent(data, removes));
        }

        public List<WorkerMessageData> GetWorkerMessages(DateTime timestamp) {
            InitOnece();
            return _records.Where(a => a.Timestamp >= timestamp).ToList();
        }

        public void Clear() {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                lock (_locker) {
                    _records.Clear();
                }
                db.DropCollection(nameof(WorkerMessageData));
            }
            VirtualRoot.RaiseEvent(new WorkerMessageClearedEvent());
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
                    if (string.IsNullOrEmpty(_connectionString)) {
                        return;
                    }
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<WorkerMessageData>();
                        foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                            if (_records.Count < NTKeyword.WorkerMessageSetCapacity) {
                                _records.AddFirst(item);
                            }
                            else {
                                col.Delete(item.Id);
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerator<IWorkerMessage> GetEnumerator() {
            InitOnece();
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _records.GetEnumerator();
        }
    }
}
