using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.ServerMessage {
    public class ServerMessageSet : IServerMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<ServerMessageData> _linkedList = new LinkedList<ServerMessageData>();

        private readonly bool _isServer;
        public ServerMessageSet(string dbFileFullName, bool isServer) {
            if (!string.IsNullOrEmpty(dbFileFullName)) {
                _connectionString = $"filename={dbFileFullName};journal=false";
            }
            _isServer = isServer;
            if (!_isServer) {
                VirtualRoot.BuildEventPath<NewServerMessageLoadedEvent>("加载到新服务器消息后叠入服务器消息栈内存", LogEnum.DevConsole,
                    action: message => {
                        if (message.Data.Count != 0) {
                            lock (_locker) {
                                foreach (var item in message.Data) {
                                    _linkedList.AddFirst(item);
                                }
                            }
                        }
                    });
            }
            VirtualRoot.BuildCmdPath<AddServerMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                var data = new ServerMessageData {
                    Id = message.Input.Id == Guid.Empty ? Guid.NewGuid() : message.Input.Id,
                    Provider = message.Input.Provider,
                    MessageType = message.Input.MessageType,
                    Content = message.Input.Content,
                    Timestamp = message.Input.Timestamp == DateTime.MinValue ? DateTime.Now : message.Input.Timestamp,
                    IsDeleted = false
                };
                if (_isServer) {
                    List<ServerMessageData> toRemoves = new List<ServerMessageData>();
                    lock (_locker) {
                        _linkedList.AddFirst(data);
                        while (_linkedList.Count > NTKeyword.ServerMessageSetCapacity) {
                            var toRemove = _linkedList.Last;
                            _linkedList.RemoveLast();
                            toRemoves.Add(toRemove.Value);
                        }
                    }
                    if (toRemoves.Count != 0) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            foreach (var item in toRemoves) {
                                col.Delete(item.Id);
                            }
                        }
                    }
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<ServerMessageData>();
                        col.Insert(data);
                    }
                }
                else {

                    VirtualRoot.Execute(new LoadNewServerMessageCommand());
                }
            });
            VirtualRoot.BuildCmdPath<UpdateServerMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                if (_isServer) {
                    ServerMessageData exist;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.Input.Id);
                        if (exist != null) {
                            exist.Update(message.Input);
                        }
                    }
                    if (exist != null) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Upsert(exist);
                        }
                    }
                }
                else {

                    VirtualRoot.Execute(new LoadNewServerMessageCommand());
                }
            });
            VirtualRoot.BuildCmdPath<MarkDeleteServerMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                if (_isServer) {
                    ServerMessageData exist = null;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.EntityId);
                        if (exist != null) {
                            _linkedList.Remove(exist);
                        }
                    }
                    if (exist != null) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Delete(message.EntityId);
                        }
                    }
                }
                else {

                    VirtualRoot.Execute(new LoadNewServerMessageCommand());
                }
            });
            VirtualRoot.BuildCmdPath<ClearServerMessages>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                // 服务端不应有清空消息的功能
                if (_isServer) {
                    return;
                }
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    lock (_locker) {
                        _linkedList.Clear();
                    }
                    db.DropCollection(nameof(ServerMessageData));
                }
                VirtualRoot.RaiseEvent(new ServerMessagesClearedEvent());
            });
        }

        public List<IServerMessage> GetServerMessages(DateTime timeStamp) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return new List<IServerMessage>();
            }
            InitOnece();
            return _linkedList.Where(a => a.Timestamp >= timeStamp).Cast<IServerMessage>().ToList();
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
                            if (_linkedList.Count < NTKeyword.ServerMessageSetCapacity) {
                                _linkedList.AddFirst(item);
                            }
                            else {
                                col.Delete(_linkedList.Last.Value.Id);
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerator<IServerMessage> GetEnumerator() {
            InitOnece();
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _linkedList.GetEnumerator();
        }
    }
}
