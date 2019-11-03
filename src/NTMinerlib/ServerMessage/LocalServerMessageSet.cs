using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.ServerMessage {
    public class LocalServerMessageSet : IServerMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<IServerMessage> _records = new LinkedList<IServerMessage>();

        public LocalServerMessageSet(string dbFileFullName) {
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

        public void Add(string provider, string messageType, string content) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            InitOnece();
            var data = new ServerMessageData {
                Id = Guid.NewGuid(),
                Provider = provider,
                MessageType = messageType,
                Content = content,
                Timestamp = DateTime.Now
            };
            // TODO:批量持久化，异步持久化
            List<IServerMessage> removes = new List<IServerMessage>();
            lock (_locker) {
                _records.AddFirst(data);
                while (_records.Count > NTKeyword.ServerMessageSetCapacity) {
                    var toRemove = _records.Last;
                    removes.Add(toRemove.Value);
                    _records.RemoveLast();
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<ServerMessageData>();
                        col.Delete(toRemove.Value.Id);
                    }
                }
            }
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<ServerMessageData>();
                col.Insert(data);
            }
            VirtualRoot.RaiseEvent(new ServerMessageAddedEvent(data, removes));
        }

        public List<IServerMessage> GetServerMessages(DateTime timeStamp) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return new List<IServerMessage>();
            }
            InitOnece();
            return _records.Where(a => a.Timestamp >= timeStamp).ToList();
        }

        public void Clear() {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                lock (_locker) {
                    _records.Clear();
                }
                db.DropCollection(nameof(ServerMessageData));
            }
            VirtualRoot.RaiseEvent(new ServerMessageClearedEvent());
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
                        var col = db.GetCollection<ServerMessageData>();
                        foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                            if (_records.Count < NTKeyword.ServerMessageSetCapacity) {
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

        public IEnumerator<IServerMessage> GetEnumerator() {
            InitOnece();
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _records.GetEnumerator();
        }
    }
}
