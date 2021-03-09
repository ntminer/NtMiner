using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ServerMessageSet : SetBase, IServerMessageSet {
        private readonly string _connectionString;
        // 排着序呢且越新的消息越在前
        private readonly LinkedList<ServerMessageData> _linkedList = new LinkedList<ServerMessageData>();

        public ServerMessageSet(string dbFileFullName) {
            if (string.IsNullOrEmpty(dbFileFullName)) {
                throw new ArgumentNullException(nameof(dbFileFullName));
            }
            _connectionString = $"filename={dbFileFullName}";
            VirtualRoot.BuildCmdPath<AddOrUpdateServerMessageCommand>(path: message => {
                InitOnece();
                #region Server
                ServerMessageData exist;
                List<ServerMessageData> toRemoves = new List<ServerMessageData>();
                ServerMessageData data = null;
                lock (_linkedList) {
                    exist = _linkedList.FirstOrDefault(a => a.Id == message.Input.Id);
                    if (exist != null) {
                        exist.Update(message.Input);
                        exist.Timestamp = DateTime.Now;
                        _linkedList.Remove(exist);
                        _linkedList.AddFirst(exist);
                    }
                    else {
                        data = new ServerMessageData().Update(message.Input);
                        data.Timestamp = DateTime.Now;
                        _linkedList.AddFirst(data);
                        while (_linkedList.Count > NTKeyword.ServerMessageSetCapacity) {
                            toRemoves.Add(_linkedList.Last.Value);
                            _linkedList.RemoveLast();
                        }
                    }
                }
                if (exist != null) {
                    try {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Update(exist);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
                else {
                    try {
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
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
                SetServerMessageTimestamp();
                #endregion
            }, location: this.GetType());
            VirtualRoot.BuildCmdPath<MarkDeleteServerMessageCommand>(path: message => {
                InitOnece();
                #region Server
                ServerMessageData exist = null;
                lock (_linkedList) {
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
                    try {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<ServerMessageData>();
                            col.Update(exist);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
                #endregion
                SetServerMessageTimestamp();
            }, location: this.GetType());
        }

        private void SetServerMessageTimestamp() {
            DateTime timestamp = DateTime.MinValue;
            if (_linkedList.Count != 0) {
                timestamp = _linkedList.First.Value.Timestamp;
            }
            AppRoot.SetServerMessageTimestamp(timestamp);
        }

        public List<ServerMessageData> GetServerMessages(DateTime timeStamp) {
            InitOnece();
            var list = new List<ServerMessageData>();
            foreach (var item in _linkedList) {
                if (item.Timestamp >= timeStamp) {
                    list.Add(item);
                }
                else {
                    // 这是个链表，排着序呢且越新的消息越在前
                    break;
                }
            }
            return list;
        }

        protected override void Init() {
            try {
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
                SetServerMessageTimestamp();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public IEnumerable<IServerMessage> AsEnumerable() {
            InitOnece();
            return _linkedList;
        }
    }
}
