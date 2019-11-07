using LiteDB;
using NTMiner.Core;
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
                VirtualRoot.BuildCmdPath<LoadNewServerMessageCommand>(action: message => {
                    DateTime dateTime = VirtualRoot.LocalServerMessageSetTimestamp;
                    // 如果已知服务器端最新消息的时间戳不比本地已加载的最新消息新就不用加载了
                    if (message.KnowServerMessageTimestamp <= dateTime) {
                        return;
                    }
                    OfficialServer.ServerMessageService.GetServerMessagesAsync(dateTime, (response, e) => {
                        if (response.IsSuccess() && response.Data.Count > 0) {
                            LinkedList<ServerMessageData> data = new LinkedList<ServerMessageData>();
                            lock (_locker) {
                                DateTime maxTime = dateTime;
                                foreach (var item in response.Data.OrderBy(a => a.Timestamp)) {
                                    if (item.Timestamp > maxTime) {
                                        maxTime = item.Timestamp;
                                    }
                                    data.AddLast(item);
                                    _linkedList.AddFirst(item);
                                }
                                if (maxTime != dateTime) {
                                    VirtualRoot.LocalServerMessageSetTimestamp = maxTime;
                                }
                            }
                            VirtualRoot.RaiseEvent(new NewServerMessageLoadedEvent(data));
                        }
                    });
                });
            }
            VirtualRoot.BuildCmdPath<AddOrUpdateServerMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                if (_isServer) {
                    #region Server
                    ServerMessageData exist;
                    List<ServerMessageData> toRemoves = new List<ServerMessageData>();
                    ServerMessageData data = null;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.Input.Id);
                        if (exist != null) {
                            DateTime timestamp = exist.Timestamp;
                            exist.Update(message.Input);
                            // 如果更新前后时间戳没有变化则自动变更时间戳
                            if (timestamp == exist.Timestamp) {
                                exist.Timestamp = DateTime.Now;
                            }
                        }
                        else {
                            data = new ServerMessageData(message.Input);
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
                            VirtualRoot.Execute(new LoadNewServerMessageCommand(DateTime.MaxValue));
                        }
                    });
                }
            });
            VirtualRoot.BuildCmdPath<MarkDeleteServerMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                if (_isServer) {
                    #region Server
                    ServerMessageData exist = null;
                    lock (_locker) {
                        exist = _linkedList.FirstOrDefault(a => a.Id == message.EntityId);
                        if (exist != null) {
                            exist.IsDeleted = true;
                            exist.Timestamp = DateTime.Now;
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
                        VirtualRoot.Execute(new LoadNewServerMessageCommand(DateTime.MaxValue));
                    });
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

        public List<ServerMessageData> GetServerMessages(DateTime timeStamp) {
            if (string.IsNullOrEmpty(_connectionString)) {
                return new List<ServerMessageData>();
            }
            InitOnece();
            return _linkedList.Where(a => a.Timestamp >= timeStamp).ToList();
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
