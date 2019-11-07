using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.ServerMessage {
    public class LocalServerMessageSet : IServerMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<ServerMessageData> _records = new LinkedList<ServerMessageData>();

        public LocalServerMessageSet(string dbFileFullName) {
            if (!string.IsNullOrEmpty(dbFileFullName)) {
                _connectionString = $"filename={dbFileFullName};journal=false";
            }
            VirtualRoot.BuildCmdPath<AddServerMessageCommand>(action: message => {

                VirtualRoot.Execute(new LoadNewServerMessageCommand());
            });
            VirtualRoot.BuildCmdPath<UpdateServerMessageCommand>(action: message => {

                VirtualRoot.Execute(new LoadNewServerMessageCommand());
            });
            // 标记删除
            VirtualRoot.BuildCmdPath<DeleteServerMessageCommand>(action: message => {

                VirtualRoot.Execute(new LoadNewServerMessageCommand());
            });
        }

        public List<IServerMessage> GetServerMessages(DateTime timeStamp) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return new List<IServerMessage>();
            }
            InitOnece();
            return _records.Where(a => a.Timestamp >= timeStamp).Cast<IServerMessage>().ToList();
        }

        public void Add(string provider, string messageType, string content) {
            InitOnece();
            Add(Guid.Empty, provider, messageType, content, DateTime.MinValue);
        }

        private void Add(Guid id, string provider, string messageType, string content, DateTime timestamp) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            InitOnece();
            var data = new ServerMessageData {
                Id = id == Guid.Empty ? Guid.NewGuid() : id,
                Provider = provider,
                MessageType = messageType,
                Content = content,
                Timestamp = timestamp == DateTime.MinValue ? DateTime.Now : timestamp
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

        public void AddOrUpdate(IServerMessage entity) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            InitOnece();
            if (entity.Id == Guid.Empty) {
                Add(entity.Id, entity.Provider, entity.MessageType, entity.Content, entity.Timestamp);
                return;
            }
            ServerMessageData exist;
            lock (_locker) {
                exist = _records.FirstOrDefault(a => a.Id == entity.Id);
                if (exist != null) {
                    exist.Update(entity);
                }
            }
            if (exist != null) {
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    var col = db.GetCollection<ServerMessageData>();
                    col.Upsert(exist);
                }
            }
            else {
                Add(entity.Id, entity.Provider, entity.MessageType, entity.Content, entity.Timestamp);
            }
        }

        public void Remove(Guid id) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return;
            }
            InitOnece();
            ServerMessageData exist = null;
            lock (_locker) {
                exist = _records.FirstOrDefault(a => a.Id == id);
                if (exist != null) {
                    _records.Remove(exist);
                }
            }
            if (exist != null) {
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    var col = db.GetCollection<ServerMessageData>();
                    col.Delete(id);
                }
            }
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
                                col.Delete(_records.Last.Value.Id);
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
