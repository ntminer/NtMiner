using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.ServerMessage {
    public class ServerMessageSet : IServerMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<ServerMessageData> _linkedList = new LinkedList<ServerMessageData>();

        public ServerMessageSet(string dbFileFullName, bool isServer) {
            if (string.IsNullOrEmpty(dbFileFullName)) {
                throw new ArgumentNullException(nameof(dbFileFullName));
            }
            _connectionString = $"filename={dbFileFullName};journal=false";
            if (!isServer) {
                VirtualRoot.AddCmdPath<LoadNewServerMessageCommand>(action: message => {
                    if (!VirtualRoot.IsServerMessagesVisible) {
                        return;
                    }
                    DateTime localTimestamp = VirtualRoot.LocalServerMessageSetTimestamp;
                    // 如果已知服务器端最新消息的时间戳不比本地已加载的最新消息新就不用加载了
                    if (message.KnowServerMessageTimestamp <= Timestamp.GetTimestamp(localTimestamp)) {
                        return;
                    }
                    OfficialServer.ServerMessageService.GetServerMessagesAsync(localTimestamp, (response, e) => {
                        if (response.IsSuccess() && response.Data.Count > 0) {
                            ReceiveServerMessage(response.Data);
                        }
                    });
                });
                VirtualRoot.AddCmdPath<ReceiveServerMessageCommand>(action: message => {
                    ReceiveServerMessage(message.Data);
                });
            }
            VirtualRoot.AddCmdPath<AddOrUpdateServerMessageCommand>(action: message => {
                InitOnece();
                if (isServer) {
                    #region Server
                    ServerMessageData exist;
                    List<ServerMessageData> toRemoves = new List<ServerMessageData>();
                    ServerMessageData data = null;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.Input.Id);
                        if (exist != null) {
                            exist.Update(message.Input);
                            exist.Timestamp = DateTime.Now;
                            _linkedList.Remove(exist);
                            _linkedList.AddFirst(exist);
                        }
                        else {
                            data = new ServerMessageData(message.Input) {
                                Timestamp = DateTime.Now
                            };
                            _linkedList.AddFirst(data);
                            while (_linkedList.Count > NTKeyword.ServerMessageSetCapacity) {
                                toRemoves.Add(_linkedList.Last.Value);
                                _linkedList.RemoveLast();
                            }
                        }
                    }
                    if (exist != null) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Update(exist);
                        }
                    }
                    else {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            if (toRemoves.Count != 0) {
                                foreach (var item in toRemoves) {
                                    col.Delete(item.Id);
                                }
                            }
                            col.Insert(data);
                        }
                    }
                    #endregion
                }
                else {
                    OfficialServer.ServerMessageService.AddOrUpdateServerMessageAsync(new ServerMessageData(message.Input), (response, ex) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadNewServerMessageCommand());
                        }
                    });
                }
            });
            VirtualRoot.AddCmdPath<MarkDeleteServerMessageCommand>(action: message => {
                InitOnece();
                if (isServer) {
                    #region Server
                    ServerMessageData exist = null;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.EntityId);
                        if (exist != null) {
                            exist.IsDeleted = true;
                            exist.Content = string.Empty;
                            exist.Timestamp = DateTime.Now;
                            _linkedList.Remove(exist);
                            _linkedList.AddFirst(exist);
                        }
                    }
                    if (exist != null) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Update(exist);
                        }
                    }
                    #endregion
                }
                else {
                    OfficialServer.ServerMessageService.MarkDeleteServerMessageAsync(message.EntityId, (response, ex) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadNewServerMessageCommand());
                        }
                    });
                }
            });
            VirtualRoot.AddCmdPath<ClearServerMessages>(action: message => {
                InitOnece();
                // 服务端不应有清空消息的功能
                if (isServer) {
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

        private void ReceiveServerMessage(List<ServerMessageData> data) {
            if (data == null) {
                return;
            }
            InitOnece();
            DateTime localTimestamp = VirtualRoot.LocalServerMessageSetTimestamp;
            LinkedList<ServerMessageData> newDatas = new LinkedList<ServerMessageData>();
            lock (_locker) {
                DateTime maxTime = localTimestamp;
                foreach (var item in data.OrderBy(a => a.Timestamp)) {
                    if (item.Timestamp > maxTime) {
                        maxTime = item.Timestamp;
                    }
                    newDatas.AddLast(item);
                    var exist = _linkedList.FirstOrDefault(a => a.Id == item.Id);
                    if (exist != null) {
                        _linkedList.Remove(exist);
                    }
                    if (!item.IsDeleted) {
                        _linkedList.AddFirst(item);
                    }
                }
                if (maxTime != localTimestamp) {
                    VirtualRoot.LocalServerMessageSetTimestamp = maxTime;
                }
            }
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<ServerMessageData>();
                foreach (var item in newDatas) {
                    if (item.IsDeleted) {
                        col.Delete(item.Id);
                    }
                    else {
                        col.Upsert(item);
                    }
                }
            }
            VirtualRoot.RaiseEvent(new NewServerMessageLoadedEvent(newDatas));
        }

        public List<ServerMessageData> GetServerMessages(DateTime timeStamp) {
            InitOnece();
            var list = new List<ServerMessageData>();
            foreach (var item in _linkedList) {
                if (item.Timestamp >= timeStamp) {
                    list.Add(item);
                }
                else {
                    // 这是个链表，最新的消息在前
                    break;
                }
            }
            return list;
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

        public IEnumerable<IServerMessage> AsEnumerable() {
            InitOnece();
            return _linkedList;
        }
    }
}
