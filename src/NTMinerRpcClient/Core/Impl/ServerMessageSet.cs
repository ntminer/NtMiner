using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ServerMessageSet : SetBase, IServerMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<ServerMessageData> _linkedList = new LinkedList<ServerMessageData>();

        public ServerMessageSet(string dbFileFullName) {
            if (string.IsNullOrEmpty(dbFileFullName)) {
                throw new ArgumentNullException(nameof(dbFileFullName));
            }
            _connectionString = $"filename={dbFileFullName}";
            VirtualRoot.BuildCmdPath<LoadNewServerMessageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                if (!RpcRoot.IsServerMessagesVisible) {
                    return;
                }
                DateTime localTimestamp = VirtualRoot.LocalServerMessageSetTimestamp;
                // 如果已知服务器端最新消息的时间戳不比本地已加载的最新消息新就不用加载了
                if (message.KnowServerMessageTimestamp <= Timestamp.GetTimestamp(localTimestamp)) {
                    return;
                }
                RpcRoot.OfficialServer.ServerMessageBinaryService.GetServerMessagesAsync(localTimestamp, (response, e) => {
                    if (response.IsSuccess()) {
                        if (response.Data.Count > 0) {
                            VirtualRoot.Execute(new ReceiveServerMessageCommand(response.Data));
                        }
                    }
                    else {
                        if (e != null) {
                            Logger.ErrorDebugLine(e);
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    }
                });
            });
            VirtualRoot.BuildCmdPath<ReceiveServerMessageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                ReceiveServerMessage(message.Data);
            });
            VirtualRoot.BuildCmdPath<AddOrUpdateServerMessageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                RpcRoot.OfficialServer.ServerMessageService.AddOrUpdateServerMessageAsync(new ServerMessageData().Update(message.Input), (response, ex) => {
                    if (response.IsSuccess()) {
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(ex), autoHideSeconds: 4);
                    }
                });
            });
            VirtualRoot.BuildCmdPath<MarkDeleteServerMessageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                RpcRoot.OfficialServer.ServerMessageService.MarkDeleteServerMessageAsync(message.EntityId, (response, ex) => {
                    if (response.IsSuccess()) {
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(ex), autoHideSeconds: 4);
                    }
                });
            });
            VirtualRoot.BuildCmdPath<ClearServerMessagesCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                try {
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        lock (_linkedList) {
                            _linkedList.Clear();
                        }
                        db.DropCollection(nameof(ServerMessageData));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
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
            lock (_linkedList) {
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
            try {
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
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
